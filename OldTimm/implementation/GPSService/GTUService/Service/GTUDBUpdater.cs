using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.QueueUpdater;
using TIMM.GTUService;
namespace GTUService.TIMM
{
    public class GTUDBUpdater
    {
        static readonly GTUDBUpdater s_Instance = new GTUDBUpdater();
        private QueueUpdater<GTU> _GTUQUpdater;

        GTUDBUpdater()
        {
            _GTUQUpdater = new QueueUpdater<GTU>();
            _GTUQUpdater.Init(new QueueUpdater<GTU>.ProcessQHandler(this.UpdateDB), new QueueUpdater<GTU>.ProcessQHandler(this.ProcessFailure));

        }
        /// <summary>
        /// Singleton class
        /// </summary>
        public static GTUDBUpdater Instance
        {
            get
            {
                return s_Instance;
            }
        }

        public void AddQ(GTU oGTU)
        {
            _GTUQUpdater.AddQ(oGTU);
        }

        /// <summary>
        /// This method will be called by QueueUpdater to process any oGTU which is
        /// failed to be updated in the database. 
        /// </summary>
        /// <param name="oGTU"></param>
        /// <returns></returns>
        bool ProcessFailure(GTU oGTU)
        {
            //TODO: save to local files. 
            return true;
        }

        /// <summary>
        ///  This is to implemet the ProcessQHandler delegate in QueueUpdater.
        ///  This method is called in the thread pool of ProcessQHandler to process the queued pending GTU 
        ///     
        /// </summary>
        /// <param name="oGTU"></param>
        /// <returns></returns>
        bool UpdateDB(GTU oGTU)
        {
            bool bRet = false;

            System.Diagnostics.Trace.TraceInformation("Begin to Operate DB \n");
            //TODO: Add a new class for updating database, and update the database at here 
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            bRet = dao.UpdateGTU(oGTU);
            bRet &= dao.UpdateGTUList(oGTU.Code);
            if (!bRet)
                System.Diagnostics.Trace.TraceError("\n Failed to update GTU{0} to the database. \n", oGTU.Code);
            else
                System.Diagnostics.Trace.TraceInformation("End Operate DB \n");
            return bRet;            

        }
    }
}
