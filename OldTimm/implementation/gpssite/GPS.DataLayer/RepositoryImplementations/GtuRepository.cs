using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Utilities.OfficeHelpers;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;
using NHibernate.Criterion;

namespace GPS.DataLayer
{
    public class GtuRepository : RepositoryBase, GPS.DataLayer.IGtuRepository
    {
        public GtuRepository() { }

        public GtuRepository(ISession session) : base(session) { }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public Gtu GetGtu(string uniqueId)
        {
            return InternalSession.Linq<Gtu>().Where(g => g.UniqueID == uniqueId).FirstOrDefault();
        }

        /// <summary>
        /// Get By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Gtu GetGtu(int id)
        {
            return InternalSession.Linq<Gtu>().Where(g => g.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Return a list of all existing Gtus.
        /// </summary>
        /// <returns>
        /// An <see cref="IList"/> containing all <see cref="Gtus"/>s.
        /// </returns>
        public IList<Gtu> GetGtus(int[] ids)
        {
            return InternalSession.CreateCriteria(typeof(Gtu)).Add(Expression.In("Id", ids)).List<Gtu>();
        }

        /// <summary>
        /// Return a list of all existing Gtus.
        /// </summary>
        /// <returns>
        /// An <see cref="IList"/> containing all <see cref="Gtus"/>s.
        /// </returns>
        public IList<Gtu> GetAllGtus()
        {
            return InternalSession.Linq<Gtu>().ToList();
        }

        

        /// <summary>
        /// Delete a GTU specified by unique id.
        /// </summary>
        /// <param name="uniqueId">The unique id of the GTU to be deleted.</param>
        public void DeleteGtu(string uniqueId)
        {
            Gtu gtu = this.GetGtu(uniqueId);
            base.Delete(gtu);
        }

        /// <summary>
        /// Update GTU.
        /// </summary>
        /// <param name="gtu">The gtu to be updated.</param>
        public Gtu UpdateGtu(Gtu gtu)
        {
            Gtu g = this.GetGtu(gtu.UniqueID);
            if (null != g)
            {
                g.IsEnabled = gtu.IsEnabled;
                g.Model = gtu.Model;
                g.User = gtu.User;
            }
            base.Update(g);

           
            return g;
        }

        /// <summary>
        /// Add a Gtu to the database.
        /// </summary>
        /// <param name="gtu">A <see cref="Gtus"/> object.</param>
        /// <returns>The <see cref="Gtus"/> just added successfully.</returns>
        public Gtu AddGtu(Gtu gtu)
        {
            
            base.Insert(gtu);
            return gtu;
        }

        public void AddGtus(IEnumerable<Gtu> gtus)
        {
            if (null != gtus)
            {
                foreach (Gtu g in gtus)
                {
                    InternalSession.Save(g);
                }
                InternalSession.Flush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public IEnumerable<Gtu> LoadGtuFromExcel(string filename)
        {
            //ExcelHelperClient proxy = new ExcelHelperClient();
            //GtuRecord[] gtuRecord = proxy.ReadGtuRecord(filename);

            List<Gtu> gtusList = new List<Gtu>();
            //try
            //{
            //    if (gtuRecord.Length > 0)
            //    {
            //        //IList<Gtu> gtus = new List<Gtu>();

            //        for (int i = 0; i < gtuRecord.Length; i++)
            //        {
            //            Gtu gtu = this.GetGtu(gtuRecord[i].UniqueId);
            //            if (null == gtu)
            //            {
            //                Gtu g = new Gtu();
            //                g.Id = System.Guid.NewGuid().GetHashCode();
            //                g.UniqueID = gtuRecord[i].UniqueId;
            //                g.Model = gtuRecord[i].Model;
            //                g.IsEnabled = gtuRecord[i].Enabled == "1" ? true : false;
            //                g.User = null;
            //                //g.User.Id = 0;
            //                gtusList.Add(g);
            //            }
            //        }

            //        this.AddGtus(gtusList);
            //    }
                return gtusList;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }
    }
}
