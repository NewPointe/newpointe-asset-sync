using System;
using System.Linq;
using System.Threading.Tasks;

using NewPointe.Util;
using NewPointe.Rock.Api;
using NewPointe.ProfileManager;
using NewPointe.JitBit;
using System.Collections.Generic;

using JitBitRest = NewPointe.JitBit.Structures;
using ProfileManagerRest = NewPointe.ProfileManager.Structures;

namespace NewPointe.AssetSync
{
    class Program
    {

        static async Task Main(string[] args)
        {

            // Collect info from JitBit
            var JitBitClient = new JitBitClient("https://newpointe.JitBit.com/helpdesk/");

            Console.Write("JitBit Username: ");
            var JitBitUsername = Console.ReadLine();

            Console.Write("JitBit Password: ");
            var JitBitPassword = ConsoleUtil.ReadPassword();

            Console.WriteLine("");

            await JitBitClient.Login(JitBitUsername, JitBitPassword);

            var JitBitAssets = await JitBitClient.GetAllAssets();
            var JitBitAssetsById = JitBitAssets.ToDictionary(a => a.ItemId);
            var JitBitAssetsBySerialNumber = JitBitAssets.Where(a => a.SerialNumber != null).ToDictionary(a => a.SerialNumber.ToLowerInvariant());

            Console.WriteLine("Found {0} Assets in JitBit", JitBitAssets.Length);

            // Collect info from Profile Manager
            var ProfileManagerClient = new ProfileManagerClient("https://npmdm.newpointe.org/");

            Console.Write("Profile Manager Username: ");
            var ProfileManagerUsername = Console.ReadLine();

            Console.Write("Profile Manager Password: ");
            var ProfileManagerPassword = ConsoleUtil.ReadPassword();

            Console.WriteLine("");

            await ProfileManagerClient.Login(ProfileManagerUsername, ProfileManagerPassword);

            var ProfileManagerDeviceIds = await ProfileManagerClient.GetDeviceIds();
            var ProfileManagerDevices = await ProfileManagerClient.GetDevices(ProfileManagerDeviceIds.Take(2).ToArray());
            var ProfileManagerDevicesById = ProfileManagerDevices.ToDictionary(d => d.Id);
            var ProfileManagerDevicesBySerialNumber = ProfileManagerDevices.Where(d => d.SerialNumber != null).ToDictionary(d => d.SerialNumber.ToLowerInvariant());

            Console.WriteLine("Found {0} Devices in Profile Manager", ProfileManagerDevices.Length);

            Console.WriteLine("Syncing Devices from Profile Manager to JitBit...", JitBitAssets.Length);

            foreach (var pmDevice in ProfileManagerDevices)
            {
                JitBitRest.Asset jbAsset = null;

                // Try matching by serial number
                if(pmDevice.SerialNumber != null) {
                    jbAsset = JitBitAssetsBySerialNumber.GetValueOrDefault(pmDevice.SerialNumber.ToLowerInvariant());

                // If we got a match, update the JitBit asset
                if(jbAsset != null) {

                    Console.WriteLine("Syncing '{0}'@JitBit with '{1}'@ProfileManager", jbAsset.ModelName, pmDevice.DeviceName);

                    var assetUpdate = new JitBitRest.UpdateAssetParameters {
                        Id = jbAsset.ItemId,
                        ModelName = string.Format("{0} ({1})", pmDevice.ModelName, pmDevice.DeviceName),
                        Manufacturer = "Apple",
                        Quantity = 1,
                        SerialNumber = pmDevice.SerialNumber
                    };

                    await JitBitClient.UpdateAsset(assetUpdate);

                }
                // Otherwise, create an new asset fot it
                else {

                }

            }

        }

    }
}
