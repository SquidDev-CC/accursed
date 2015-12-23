import Model from require "lapis.db.model"

class ModVersionDownload extends Model
	@primary_key: "id"
	@__table_name: "mod_version_downloads"
	@relations: {
		{ "mod_versions", belongs_to: "ModVersion", key: "version_id" }
	}
