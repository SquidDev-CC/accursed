import default_environment from require "lapis.cmd.util"
import execute from require "lapis.cmd.actions"

exec = (cmd) ->
	f = io.popen cmd
	with f\read("*all")\gsub "%s*$", ""
		f\close!

{
	name: "run"
	usage: "run [environment]"
	help: "annotate a model with schema"

	(...) ->
		assert os.execute("moonc .") == 0, "Error running moonc"
		assert os.execute("gulp") == 0, "Error running gulp"
		execute({"server", ...})
}