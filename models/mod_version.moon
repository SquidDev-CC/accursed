import Model from require "lapis.db.model"

class ModVersion extends Model
	@primary_key: "id"
	@__table_name: "mod_versions"
	@relations: {
		{ "mod", belongs_to: "Mod", key: "mod_id" }
		{ "files", has_many: "ModVersionFile", key: "version_id" }
		{ "downloads", has_many: "ModVersionDownload", key: "version_id" }
	}
