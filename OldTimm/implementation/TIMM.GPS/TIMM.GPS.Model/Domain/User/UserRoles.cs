using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TIMM.GPS.Model
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
