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
    public class EnumWrapper<TEnum> where TEnum : struct, IConvertible
    {
        public EnumWrapper()
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("Not an enum");
        }

        public TEnum Enum { get; set; }

        public int Value
        {
            get { return Convert.ToInt32(Enum); }
            set { Enum = (TEnum)(object)value; }
        }

        public static implicit operator TEnum(EnumWrapper<TEnum> w)
        {
            if (w == null) return default(TEnum);
            else return w.Enum;
        }

        public static implicit operator EnumWrapper<TEnum>(TEnum e)
        {
            return new EnumWrapper<TEnum>() { Enum = e };
        }

        public static implicit operator int(EnumWrapper<TEnum> w)
        {
            if (w == null)
                return Convert.ToInt32(default(TEnum));
            else
                return w.Value;
        }

    }
}
