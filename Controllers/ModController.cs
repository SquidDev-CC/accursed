using System;
using System.Linq;
using System.Threading.Tasks;
using Accursed.Models;
using Accursed.Services;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using ModVersion = Accursed.Models.ModVersion;

namespace Accursed.Controllers
{
    public class ModController : Controller
    {
        private readonly IModFetcher fetcher;
        private readonly AccursedDbContext context;
        private readonly TimeSpan versionRefresh;

        public ModController(
            IModFetcher fetcher,
            AccursedDbContext context,
            IConfiguration configuration
        )
        {
            this.fetcher = fetcher;
            this.context = context;

            this.versionRefresh = TimeSpan.Parse(configuration["VersionRefreshTime"] ?? "00:30");
        }

        public async Task<IActionResult> ViewMod(string slug)
        {
            Mod mod = await FindMod(slug);
            if (mod.Versions == null)
            {
                mod.Versions = await context.Versions
                    .Where(x => x.ModId == mod.Id)
                    .OrderBy(x => x.DownloadId)
                    .ToListAsync();
            }

            return View(mod);
        }

        public async Task<IActionResult> ViewVersion(string modSlug, string versionName)
        {
            Mod mod = await FindMod(modSlug, true);
            ModVersion version = await context.Versions
                .Where(x => x.ModId == mod.Id)
                .Include(x => x.Mod)
                .Include(x => x.Files)
                .WhereNameAsync(versionName);

            return View(version);
        }

        public async Task<IActionResult> GetVersion(string modSlug, string versionName)
        {
            Mod mod = await FindMod(modSlug);
            ModVersion version = await context.Versions
                .Where(x => x.ModId == mod.Id)
                .Include(x => x.Mod)
                .WhereNameAsync(versionName);

            return new RedirectResult(FormatDownload(mod, version.DownloadId));
        }

        public async Task<IActionResult> GetFile(string modSlug, string versionName, string fileName)
        {
            Mod mod = await FindMod(modSlug);
            ModVersion version = await context.Versions
                .Where(x => x.ModId == mod.Id)
                .WhereNameAsync(versionName);

            File file = await context.Files.FirstAsync(x => x.VersionId == version.Id);
            return new RedirectResult(FormatDownload(mod, file.DownloadId));
        }

        private async Task<Mod> FindMod(string slug, bool files = false)
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
                var versions = await context.Versions
                    .Where(x => x.ModId == mod.Id)
                    .OrderBy(x => x.DownloadId)
                    .ToListAsync();
                Console.WriteLine(Object.ReferenceEquals(mod, versions[0].Mod));

                mod.Versions = versions;
                await fetcher.RefreshVersions(mod);
                await context.SaveChangesAsync();
            }

            return mod;
        }

        public static string FormatDownload(Mod mod, uint id)
        {
            return "http://minecraft.curseforge.com/projects/" + mod.Slug + "/files/" + id + "/download";
        }
    }
}
