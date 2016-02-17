using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accursed.Models;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Network.Default;
using AngleSharp.Network;
using Microsoft.Extensions.Logging;

namespace Accursed.Services
{
    public class CurseModFetcher : IModFetcher
    {
        private readonly ILogger logger;
        private readonly IConfiguration config;
        private readonly IBrowsingContext context;

        public CurseModFetcher(ILoggerFactory loggerFactory)
        {
            config = Configuration.Default
                .WithDefaultLoader(requesters: new IRequester[] { new HttpClientRequester(), new DataRequester() });
            context = BrowsingContext.New(config);
            this.logger = loggerFactory.CreateLogger<CurseModFetcher>();
        }

        public string VersionName(string name, string version)
        {
            return version
                .Trim()
                .StripEnd(".jar").StripEnd(".zip") // File extensions
                .StripStart(name) // Most prefix with name
                .Trim(' ', '-'); // Most have space or hyphen
        }

        public string FileName(ModVersion version, string file)
        {
            string suffix = file
                .Trim()
                .StripStart(version.Mod.Name)
                .Trim(' ', '-')
                .StripStart(version.FancyName)
                .Trim(' ', '-');

            string prefix = version.Mod.Name + "-" + version.FancyName;

            if (suffix.Length == 0 || !Char.IsLetterOrDigit(suffix[0]))
            {
                return prefix + suffix;
            }
            else
            {
                return prefix + "-" + suffix;
            }
        }

        public uint ExtractId(string url)
        {
            string sub = url.Substring(url.LastIndexOf('/') + 1);
            return UInt32.Parse(sub);
        }

        public async Task<Mod> FetchMod(string slug)
        {
            slug = slug.ToLowerInvariant();
            logger.LogInformation($"Fetching mod {slug}");

            var dom = await context.OpenAsync($"http://minecraft.curseforge.com/projects/{slug}/files/");
            Mod mod = new Mod()
            {
                Slug = slug,
                Name = dom.QuerySelector("h1").TextContent.Trim(),
                Versions = new List<ModVersion>(),
            };

            logger.LogInformation($"Found mod {mod.Name} for {slug}");
            await RefreshVersions(mod, dom);

            return mod;
        }

        public async Task PopulateVersionFiles(ModVersion version)
        {
            logger.LogInformation($"Populating files for {version.Name}");
            var dom = await context.OpenAsync($"http://minecraft.curseforge.com/projects/{version.Mod.Slug}/files/{version.DownloadId}");

            {
                var name = dom
                    .QuerySelectorAll("li div.info-label")
                    .First(x => x.TextContent.Contains("Filename"))
                    .ParentElement
                    .QuerySelector("div.info-data")
                    .TextContent
                    .Trim();

                version.Files.Add(new File()
                {
                    DownloadId = version.DownloadId,
                    Name = name,
                    NormalisedName = FileName(version, name),
                    Version = version,
                });
            }

            foreach (IElement element in dom.QuerySelectorAll("section.details-additional-files tr.project-file-list-item"))
            {
                var node = element.QuerySelector("div.project-file-name-container a.overflow-tip");
                var id = ExtractId(node.Attributes["href"].Value);
                var name = node.TextContent.Trim();

                version.Files.Add(new File()
                {
                    DownloadId = id,
                    Name = name,
                    NormalisedName = FileName(version, name),
                    Version = version,
                });
            }
        }

        public async Task RefreshVersions(Mod mod)
        {
            logger.LogInformation($"Updating versions for {mod.Name}");
            var dom = await context.OpenAsync($"http://minecraft.curseforge.com/projects/{mod.Slug}/files/");
            await RefreshVersions(mod, dom);
        }

        public Task RefreshVersions(Mod mod, IDocument dom)
        {
            mod.VersionRefresh = DateTime.Now;

            var versions = mod.Versions.ToDictionary(x => x.DownloadId, x => x);

            return Task.WhenAll(dom.QuerySelectorAll("tr.project-file-list-item").Select(async element =>
            {
                var node = element.QuerySelector("div.project-file-name-container a.overflow-tip");
                var id = ExtractId(node.Attributes["href"].Value);

                if (!versions.ContainsKey(id))
                {
                    var name = node.TextContent.Trim();
                    var mcVersion = element.QuerySelector("td.project-file-game-version span.version-label").TextContent.Trim();

                    ModVersion version = new ModVersion()
                    {
                        Name = name,
                        FancyName = VersionName(mod.Name, name),
                        DownloadId = id,
                        MCVersion = mcVersion,

                        Mod = mod,
                        Files = new List<File>(),
                    };
                    mod.Versions.Add(version);

                    logger.LogInformation($"Created version {version.Name} : {version.FancyName}");
                    await PopulateVersionFiles(version);
                }

                return Task.FromResult(0);
            }));
        }
    }
}
