
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vargainc.Timm.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserRoles : int
    {
        Walker = 1,
        Sales = 46,
        CampaignSupervisor = 47,
        Driver = 48,
        Client = 49,
        Auditor = 50,
        DistributionManager = 51,
        DistributionSupervisor = 52,
        Administrator = 53
    }    
}
