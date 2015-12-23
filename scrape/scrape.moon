ffi = require "ffi"
import attribute, contents, loadHTML, xpath from require "scrape.xml"
import Mod, ModVersion, ModVersionDownload, ModVersionFile from require "models"
import format_date from require "lapis.db"
import escape_pattern, trim from require "lapis.util"

config = require("lapis.config").get!

http = require(if ngx then "lapis.nginx.http" else "socket.http")

version_name = (name, version) ->
	version = trim version
	version = version\gsub("%.jar$", "")\gsub("%.zip$", "") -- -- Remove some basic file extensions
	version = version\gsub("^" .. escape_pattern(name) .. "[%s-]*", "") -- -- Remove name and possible whitespace/dashes

	version

request = (host, sub) ->
	local res, status, headers
	url = host .. sub
	for i = 1, config.redirects or 10 do
		res, status, headers = http.request url
		if status == 301 or status == 302 and headers.Location then
			location = headers.Location
			if not location\match("^https?://") then
				location = host .. location
			print "Redirecting from #{url} to #{location}"
			url = location
		else
			break
	res, status, headers


scrape_version = (mod, version) ->
	res, status = request "http://minecraft.curseforge.com", "/projects/#{mod.slug}/files/#{version.download_id}?cookieTest=1"
	assert status == 200, "#{mod.slug}/#{version.download_id} failed with #{status}"

	doc = loadHTML(res)
	query = xpath(doc)

	files = {}
	do -- Initial file
		files[#files + 1] = {
			download_id: version.download_id,
			version_id: version.id,
			name: trim(contents(query("//li[div[contains(@class,'info-label')]='Filename']/div[contains(@class,'info-data')]")[1]))
		}

	-- Other files
	for download in *query "//section[contains(@class, 'details-additional-files')]//tr[contains(@class, 'project-file-list-item')]" do
		node = query(".//div[contains(@class, 'project-file-name-container')]/a[contains(@class, 'overflow-tip')]", download)[1]
		download_id = attribute(node, "href")\match("/([^/]+)$")\gsub("%D", "")
		print "Getting "
		print contents(node)

		files[#files + 1] = {
			download_id: tonumber(download_id)
			version_id: version.id,
			name: trim(contents(node))
		}
	files


scrape_mod = (mod, downloads = false) ->
	res, status, headers = request "http://minecraft.curseforge.com", "/projects/#{mod.slug}/files/?cookieTest=1"
	if status ~= 200 then
		return false, "Cannot find #{mod.slug}: Curse responded with #{status}", status: 404

	doc = loadHTML(res)
	query = xpath(doc)

	mod.name = trim(contents query("//h1")[1]) or mod.name
	if not mod.id then
		-- Force downloads if creating
		downloads = true

	-- Update timestamps
	time = format_date!
	mod.version_refresh = time
	if downloads then
		mod.download_refresh = time

	-- Save the model: either save or update it
	if mod.id then
		mod\update!
	else
		mod = Mod\create mod

	versions = { version.download_id, version for version in *mod\get_versions! }
	threads = {}

	for download in *query "//tr[contains(@class, 'project-file-list-item')]" do
		node = query(".//div[contains(@class, 'project-file-name-container')]/a[contains(@class, 'overflow-tip')]", download)[1]

		download_id = attribute(node, "href")\match("/([^/]+)$")\gsub("%D", "")
		download_id = tonumber(download_id)

		if not versions[download_id] then
			name = trim contents(node)
			mc_version = trim contents(query(".//td[contains(@class, 'project-file-game-version')]/span[@class='version-label']", download)[1])

			version = ModVersion\create {
				mod_id: mod.id
				:download_id,
				:mc_version,
				:name,
				fancy_name: version_name(mod.name, name)
			}

			version_id = version.id
			versions[download_id] = version

			tg, err = ngx.thread.spawn(scrape_version, mod, version)
			if not tg then
				ngx.log(ngx.ERROR, "failed to spawn file scraper thread " .. err)
			else
				threads[#threads] = tg

		if downloads then
			count = contents(query(".//td[contains(@class, 'project-file-downloads')]", download)[1])
			count = count\gsub("%D", "")

			ModVersionDownload\create {
				version_id: versions[download_id].id,
				count: tonumber(count)
				created_at: time
			}

	for thread in *threads do
		ok, res = ngx.thread.wait(thread)
		if not ok then
			ngx.log(ngx.ERROR, "Error fetching: " .. res)
		else
			for file in *res do
				ModVersionFile\create file
	return true, mod

scrape_mod
