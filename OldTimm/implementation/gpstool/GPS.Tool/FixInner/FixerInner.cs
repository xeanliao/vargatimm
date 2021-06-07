using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GPS.Tool.Data;

namespace GPS.Tool.FixInner
{
    public abstract class FixerInnerBase
    {
        public delegate void MessageHandler(int total, int current, int innerCount, string code, bool inner, bool completed);
        public event MessageHandler Messaging;
        public abstract void StartFix();
        protected void SendMessage(int total, int current, int innerCount, string code, bool inner, bool completed)
        {
            if (Messaging != null)
            {
                Messaging(total, current, innerCount, code, inner, completed);
            }
        }
    }

    public abstract class FixerInner<T> : FixerInnerBase
    {

        protected Thread thread;
        protected int _count = 0;
        protected int _current = 0;
        protected int _innerCount = 0;

        public override void StartFix()
        {
            thread = new Thread(new ThreadStart(Fix));
            thread.Start();
        }

        void Fix()
        {
            _count = GetItemCount();
            int number = 100;
            int numberCount = _count % number > 0 ? _count / number + 1 : _count / number;
            SendMessage("start", false);
            for (int i = 0; i < numberCount; i++)
            {
                FixItems(GetItems(i * number, number));
            }
            SendMessage("End", false, true);
        }

        protected abstract int GetItemCount();
        protected abstract List<int> GetItems(int skip, int count);
        protected abstract void FixItems(List<int> items);
        
        

        protected void SendMessage(string code, bool inner)
        {
            SendMessage(code, inner, false);
        }

        protected void SendMessage(string code, bool inner, bool completed)
        {
            SendMessage(_count, _current, _innerCount, code, inner, completed);
        }
    }
}
