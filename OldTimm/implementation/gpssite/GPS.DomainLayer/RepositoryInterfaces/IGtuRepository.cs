using System;
using System.Collections.Generic;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public interface IGtuRepository
    {
        IList<Gtu> GetAllGtus();
        IList<Gtu> GetGtus(int[] ids);
        Gtu GetGtu(string uniqueId);
        Gtu GetGtu(int id);
        Gtu AddGtu(Gtu gtu);
        void AddGtus(System.Collections.Generic.IEnumerable<Gtu> gtus);
        void DeleteGtu(string uniqueId);
        System.Collections.Generic.IEnumerable<Gtu> LoadGtuFromExcel(string filename);
        Gtu UpdateGtu(Gtu gtu);
    }
}
