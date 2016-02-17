using System;
using System.Linq;
using System.Threading.Tasks;
using Accursed.Models;
using Accursed.Services;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ModVersion = Accursed.Models.ModVersion;

namespace Accursed.Controllers
{
    public class ModController : Controller
    {
        private readonly IModFetcher fetcher;
        private readonly AccursedDbContext context;
        private readonly TimeSpan versionRefresh;
        private readonly IMemoryCache cache;
        private readonly ILogger logger;

        public ModController(
            IModFetcher fetcher,
            IMemoryCache cache,
            AccursedDbContext context,
            IConfiguration configuration,
            ILoggerFactory loggerFactory
        )
        {
            this.fetcher = fetcher;
            this.cache = cache; // TODO: Implement caching
            this.context = context;
            this.logger = loggerFactory.CreateLogger<ModController>();

            this.versionRefresh = TimeSpan.Parse(configuration["VersionRefreshTime"] ?? "00:30");
        }

        public async Task<IActionResult> ViewMod(string slug)
        {
            Mod mod = await FindMod(slug);
            mod.Versions = await context.Versions
                .Where(x => x.ModId == mod.Id)
                .OrderByDescending(x => x.DownloadId)
                .ToListAsync();

            return View(mod);
        }

        public async Task<IActionResult> ViewVersion(string modSlug, string versionName)
        {
            Mod mod = await FindMod(modSlug, versionName);
            ModVersion version = await context.Versions
                .Where(x => x.ModId == mod.Id)
                .Include(x => x.Mod)
                .Include(x => x.Files)
                .WhereNameAsync(versionName);

            return View(version);
        }

        public async Task<IActionResult> GetVersion(string modSlug, string versionName)
        {
            Mod mod = await FindMod(modSlug, versionName);
            ModVersion version = await context.Versions
                .Where(x => x.ModId == mod.Id)
                .Include(x => x.Mod)
                .WhereNameAsync(versionName);

            return new RedirectResult(FormatDownload(mod, version.DownloadId));
        }

        public async Task<IActionResult> GetFile(string modSlug, string versionName, string fileName)
        {
            Mod mod = await FindMod(modSlug, versionName);
            ModVersion version = await context.Versions
                .Where(x => x.ModId == mod.Id)
                .WhereNameAsync(versionName);

            File file = await context.Files.FirstAsync(x => x.VersionId == version.Id);
            return new RedirectResult(FormatDownload(mod, file.DownloadId));
        }

        private async Task<Mod> FindMod(string slug, string requiredVersion = null)
        {
            Mod mod = await context.Mods.FirstOrDefaultAsync(x => x.Slug == slug);

            if (mod == null)
            {
                mod = await fetcher.FetchMod(slug);
                context.Mods.Add(mod);
                await context.SaveChangesAsync();
            }
            else if (mod.VersionRefresh.Add(versionRefresh) > DateTime.Now)
            {
                logger.LogInformation("Version refresh time has expired: Refreshing");

                var versions = await context.Versions
                    .Where(x => x.ModId == mod.Id)
                    .ToListAsync();
                mod.Versions = versions;

                // Ideally we could fetch versions on a separate thread, but I'm going to
                // have to look into away to do this.
                await fetcher.RefreshVersions(mod);
                await context.SaveChangesAsync();
            }

            return mod;
        }

        public async void FetchVersions(Mod mod)
        {
            try
            {
                await fetcher.RefreshVersions(mod);
                await context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                logger.LogError("Fetching version " + mod, e);
            }
        }

        public static string FormatDownload(Mod mod, uint id)
        {
            return "http://minecraft.curseforge.com/projects/" + mod.Slug + "/files/" + id + "/download";
        }

        public static string FormatVersion(ModVersion version)
        {
            return "http://minecraft.curseforge.com/projects/" + version.Mod.Slug + "/files/" + version.DownloadId;
        }

        public static string FormatMod(Mod mod)
        {
            return "http://minecraft.curseforge.com/projects/" + mod.Slug;
        }
    }
}
