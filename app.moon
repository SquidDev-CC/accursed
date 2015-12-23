lapis = require "lapis"
import Mod, ModVersionFile, ModVersionDownload from require "models"
scrape = require "scrape.scrape"

require "lapis.features.etlua"

class App extends lapis.Application
	@enable "etlua"
	layout: require "views.layout"
	[index: "/"]: =>
		-- Render HTML inline for simplicity
		@title = "SquidDev"
		render: "home"
	[mod: "/mods/:mod"]: =>
		mod = Mod\find slug: @params.mod
		if not mod then
			success, mod, status = scrape slug: @params.mod
			if not success then
				return mod, status

		-- Preload mods
		ModVersionFile\include_in mod\get_versions!, "version_id", as: "files", flip: true, many: true
		ModVersionDownload\include_in mod\get_versions!, "version_id", as: "downloads", flip: true, many: true, order: "created_at"
		@title = mod.name .. " | Mods | SquidDev"
		@footer = [[<script src="https://cdn.plot.ly/plotly-latest.min.js"></script>]]
		@mod = mod

		render: "mod"lapis = require "lapis"
import Mod, ModVersionFile, ModVersionDownload from require "models"
scrape = require "scrape.scrape"

require "lapis.features.etlua"

class App extends lapis.Application
	@enable "etlua"
	layout: require "views.layout"
	[index: "/"]: =>
		-- Render HTML inline for simplicity
		@title = "SquidDev"
		render: "home"
	[mod: "/mods/:mod"]: =>
		mod = Mod\find slug: @params.mod
		if not mod then
			success, mod, status = scrape slug: @params.mod
			if not success then
				return mod, status

		-- Preload mods
		ModVersionFile\include_in mod\get_versions!, "version_id", as: "files", flip: true, many: true
		ModVersionDownload\include_in mod\get_versions!, "version_id", as: "downloads", flip: true, many: true, order: "created_at"
		@title = mod.name .. " | Mods | SquidDev"
		@footer = [[<script src="https://cdn.plot.ly/plotly-latest.min.js"></script>]]
		@mod = mod

		render: "mod"
	[food: "/food/:name"]: =>
		food_description = favorite_foods[@params.name]
		unless food_description
			return "Not found", status: 404

		@html ->
			h1 @params.name
			h2 "My thoughts on this food"
			p food_description