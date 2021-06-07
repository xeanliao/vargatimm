using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPS.Website.DAL
{
    public enum GtuStatusEum:int
    {
        No_GTU_Signal = 10,
        Inside_DND_CustomArea = 20,
        Inside_DND_nAddress = 21,
        Outside_map_boundary = 30,
        No_movement = 40,
        Duplicate_path = 50
    }
}