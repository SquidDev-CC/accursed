import create_table, create_index, types from require "lapis.db.schema"
import Mod, ModVersionDownload from require "models"
import to_json, from_json from require "lapis.util"
import format_date from require "lapis.db"

{
	[1450548685]: =>
		-- List of mods
		create_table "mods", {
			{ "id", types.id unsigned: true }
			{ "slug", types.varchar }
			{ "name", types.varchar }

			{ "version_refresh", types.timestamp }
			{ "download_refresh", types.timestamp }
		}

		create_index "mods", "slug", unique: true

		-- Lookup table for mod versions
		create_table "mod_versions", {
			{ "id", types.id unsigned: true }
			{ "mod_id", types.integer unsigned: true }
			{ "download_id", types.integer unsigned: true }
			{ "mc_version", types.varchar length: 10 }
			{ "name", types.varchar }
			{ "fancy_name", types.varchar }
		}

		create_index "mod_versions", "mod_id"
		create_index "mod_versions", "name"
		create_index "mod_versions", "fancy_name"

		-- Tracks files belonging to mod versions
		create_table "mod_version_files", {
			{ "id", types.id unsigned: true }
			{ "version_id", types.integer unsigned: true }
			{ "download_id", types.integer unsigned: true }
			{ "name", types.varchar }
		}

		create_index "mod_version_files", "version_id"
		create_index "mod_version_files", "name"

		-- Tracks downloads over time
		create_table "mod_version_downloads", {
			{ "id", types.id unsigned: true }
			{ "version_id", types.integer unsigned: true }
			{ "count", types.integer unsigned: true }
			{ "created_at", types.timestamp }
		}

		create_index "mod_version_downloads", "version_id"
	[1450813370]: =>
		require("scrape.scrape")(Mod\find(slug: "cctweaks") or slug: "cctweaks")

		versions = { version.name, version for version in * Mod\find(slug: "cctweaks")\get_versions! }
		local contents
		with file = io.open "db.json", "r" do
			contents = from_json(file\read "*a").downloads
			file\close

		for fetch in *contents do
			if fetch.success then
				for version in *fetch.items do
					ModVersionDownload\create {
						version_id: versions[version.name].id
						count: version.count\gsub("%D", ""),
						created_at: format_date(fetch.date / 1000)
					}

}
