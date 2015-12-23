import Model from require "lapis.db.model"

class Mod extends Model
	@primary_key: "id"
	@__table_name: "mods"
	@relations: {
		{"versions", has_many: "ModVersion", key: "mod_id" }
	}
