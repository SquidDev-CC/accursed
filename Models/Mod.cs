using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace Accursed.Models
{
    public class Mod
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }

        public DateTime VersionRefresh { get; set; }

        public List<ModVersion> Versions { get; set; }
    }

    public class ModVersion
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string FancyName { get; set; }

        public int ModId { get; set; }
        public Mod Mod { get; set; }

        public uint DownloadId { get; set; }
        public string MCVersion { get; set; }

        public List<File> Files { get; set; }
    }

    public class File
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public uint DownloadId { get; set; }

        public int VersionId { get; set; }
        public ModVersion Version { get; set; }
    }

    public static class Extensions
    {
        public static Task<ModVersion> WhereNameAsync(this IQueryable<ModVersion> versions, string versionName)
        {
            return versions.FirstAsync(x => x.Name == versionName || x.FancyName == versionName);
        }

        public static ModVersion WhereName(this IEnumerable<ModVersion> versions, string versionName)
        {
            return versions.First(x => x.Name == versionName || x.FancyName == versionName);
        }
    }
}
