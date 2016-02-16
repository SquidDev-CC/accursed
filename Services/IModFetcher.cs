using System.Threading.Tasks;
using Accursed.Models;

namespace Accursed.Services
{
    public interface IModFetcher
    {
        Task<Mod> FetchMod(string slug);

        Task RefreshVersions(Mod mod);

        Task PopulateVersionFiles(ModVersion version);
    }
}
