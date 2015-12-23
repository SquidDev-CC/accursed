import Model from require "lapis.db.model"

class ModVersionFile extends Model
	@primary_key: "id"
	@__table_name: "mod_version_files"
	@relations: {
		{ "mod_versions", belongs_to: "ModVersion", key: "version_id" }
	}
