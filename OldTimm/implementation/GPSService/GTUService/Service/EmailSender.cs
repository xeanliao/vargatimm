using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.QueueUpdater;
using GTUService.TIMM;

namespace TIMM.GTUService.Service
{
    class EmailSender
    {
        static readonly EmailSender e_Instance = new EmailSender();        
        private QueueUpdater<GTU> _EmailSender;

        EmailSender()
        {
            _EmailSender = new QueueUpdater<GTU>();
            _EmailSender.Init(new QueueUpdater<GTU>.ProcessQHandler(this.sendEmail), new QueueUpdater<GTU>.ProcessQHandler(this.ProcessFailure));

        }
        /// <summary>
        /// Singleton class
        /// </summary>
        public static EmailSender Instance
        {
            get
            {
                return e_Instance;
            }
        }

        public void AddQ(GTU oGTU)
        {
            _EmailSender.AddQ(oGTU);
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
        bool sendEmail(GTU oGTU)
        {
            TaskStore _oTaskStore = new TaskStore();
            Email _oEmail = new Email();
            bool bRet = false;
            try
            {
                string message = string.Empty;
                bool isOut=true;
                System.Diagnostics.Trace.TraceInformation("Begin to sendEmail \n");
                //TODO: Add a new class for updating database, and update the database at here 
                string emailAddresses = _oTaskStore.getEmailAddresses(oGTU.Code);
                IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
                string userName = dao.getUserForEmail(oGTU.Code);
                if (oGTU.Status == Status.InDNDArea)
                {
                    isOut = false;
                    message = oGTU.dndInfo;
                }
                else
                {
                    message = _oTaskStore.getDMName(oGTU.Code);
                }
                oGTU.SendTime = oGTU.SendTime.ToUniversalTime().AddHours(-7);
                _oEmail.CDOEmailSend(isOut,message, userName, emailAddresses, oGTU.Code, oGTU.SendTime.ToString());
                bRet = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("\n Failure sending mail. \n" + e.InnerException, oGTU.Code);
                bRet = false;
            }
            return bRet;            

        }
    }
}
