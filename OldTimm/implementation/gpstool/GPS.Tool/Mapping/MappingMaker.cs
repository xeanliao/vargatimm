using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Threading;

namespace GPS.Tool.Mapping
{
    abstract class MappingMaker
    {
        public delegate void MessageHandler(bool success, int total, int current, string message);
        public event MessageHandler Messaging;

        public delegate void StatueHandler(bool isProgress, bool success);
        public event StatueHandler Progressing;

        protected bool _stopEnabled = false;
        protected Thread thread;

        protected AreaDataContext dataContext;

        public MappingMaker()
        {
            dataContext = new AreaDataContext();
        }
        public MappingMaker(bool newData)
        {
        }
        public void StartMake()
        {
            _stopEnabled = false;
            thread = new Thread(new ThreadStart(MakeMapping));
            thread.Start();
        }
        public void StopMake()
        {
            _stopEnabled = true;
        }

        protected abstract void MakeMapping();

        protected void SendMessage(bool success, int total, int current, string message)
        {
            if (Messaging != null)
            {
                Messaging(success, total, current, message);
            }
        }

        protected void SendStatus(bool isProgress, bool success)
        {
            if (Progressing != null)
                Progressing(isProgress, success);
        }
    }
}
