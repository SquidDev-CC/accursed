config = require "lapis.config"

config "development", ->
	mysql ->
		host "127.0.0.1"
		user "root"
		password "password"
		database "curse"
	show_queries false
	port 8080

config "production", ->
	port 80
	num_workers 4
	code_cache "on"