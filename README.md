# Accursed

Helper for mods on Curse written with ASP.NET.

[Try it live](http://46.101.4.196/)

## Setup

- Install [ASP.NET](https://docs.asp.net/en/latest/getting-started/index.html). This has been tested under Mono and .Net core.
- Install [Node](https://nodejs.org)
- Install Bower and Gulp `sudo npm install -g gulp bower`
- Restore packages: `dnu restore && npm install && bower install`
- Build `dnu build --quiet && gulp`
- Run `dnx web`

## Usage
Add this to your `build.gradle`.
```gradle
repositories {
	ivy {
		pattern 'http://46.101.4.196/mods/download/[module]/[revision]/[artifact](-[classifier])(.[ext])'
	}
}

dependencies {
	compile group: 'Accursed', name: 'ComputerCraft', version: "1.78", artifact: 'ComputerCraft1.78'
}
```
