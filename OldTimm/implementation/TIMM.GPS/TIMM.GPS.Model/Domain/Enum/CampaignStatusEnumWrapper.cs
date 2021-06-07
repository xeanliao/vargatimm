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
    public class CampaignStatusEnumWrapper : EnumWrapper<CampaignStatusEnum>
    {
        public int CampaignId { get; set; }

        public static implicit operator CampaignStatusEnumWrapper(CampaignStatusEnum e)
        {
            return new CampaignStatusEnumWrapper() { Enum = e };
        }

    }
}
