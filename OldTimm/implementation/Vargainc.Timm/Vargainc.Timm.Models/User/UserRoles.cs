
namespace Vargainc.Timm.Models
{
    public enum UserRoles
    {
        Admin = 0x00000001,
        Customer = 0x00000010,
        StandardAdmin = 0x00000100,
        Driver = 0x00001000,
        Auditor = 0x00010000,
        Sales = 0x00100000,
        Walker = 0x01000000,
        None = 0x00000000
    }
}
