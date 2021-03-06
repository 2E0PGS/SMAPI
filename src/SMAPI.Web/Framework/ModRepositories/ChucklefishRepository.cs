using System;
using System.Threading.Tasks;
using StardewModdingAPI.Common.Models;
using StardewModdingAPI.Web.Framework.Clients.Chucklefish;

namespace StardewModdingAPI.Web.Framework.ModRepositories
{
    /// <summary>An HTTP client for fetching mod metadata from the Chucklefish mod site.</summary>
    internal class ChucklefishRepository : RepositoryBase
    {
        /*********
        ** Properties
        *********/
        /// <summary>The underlying HTTP client.</summary>
        private readonly IChucklefishClient Client;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="vendorKey">The unique key for this vendor.</param>
        /// <param name="client">The underlying HTTP client.</param>
        public ChucklefishRepository(string vendorKey, IChucklefishClient client)
            : base(vendorKey)
        {
            this.Client = client;
        }

        /// <summary>Get metadata about a mod in the repository.</summary>
        /// <param name="id">The mod ID in this repository.</param>
        public override async Task<ModInfoModel> GetModInfoAsync(string id)
        {
            // validate ID format
            if (!uint.TryParse(id, out uint realID))
                return new ModInfoModel($"The value '{id}' isn't a valid Chucklefish mod ID, must be an integer ID.");

            // fetch info
            try
            {
                var mod = await this.Client.GetModAsync(realID);
                if (mod == null)
                    return new ModInfoModel("Found no mod with this ID.");

                // create model
                return new ModInfoModel(mod.Name, this.NormaliseVersion(mod.Version), mod.Url);
            }
            catch (Exception ex)
            {
                return new ModInfoModel(ex.ToString());
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public override void Dispose()
        {
            this.Client.Dispose();
        }
    }
}
