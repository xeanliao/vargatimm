using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using GPS.Tool.Mapping;
using System.Threading;

namespace GPS.Tool.FixData
{
    public abstract class FixerBase
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

    public abstract class Fixer<T> : FixerBase
    {

        protected Thread thread;
        int _count = 0;
        int _current = 0;
        int _innerCount = 0;

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
        protected abstract List<T> GetItems(int skip, int count);
        protected abstract void FixItems(List<T> items);
        protected abstract Dictionary<int, List<ICoordinate>> GetShapes(T t);
        protected abstract string GetCode(T t);
        protected List<int> GetInnerShapeIds(T t)
        {
            Dictionary<int, List<ICoordinate>> shapes = GetShapes(t);
            List<int> ids = new List<int>();

            foreach(int i in shapes.Keys)
            {
                foreach (int j in shapes.Keys)
                {
                    if (i != j && InnerShape(shapes[j], shapes[i]))
                    {
                        ids.Add(i);
                        break;
                    }
                }
            }
            _current++;
            if (ids.Count > 0)
            {
                _innerCount++;
            }
            SendMessage(GetCode(t), ids.Count > 0);
            return ids;
        }

        private bool InnerShape(List<ICoordinate> masterCoordinates, List<ICoordinate> innerCoordinates)
        {
            bool inner = true;
            foreach (ICoordinate coordinate in innerCoordinates)
            {
                if (!ShapeMethods.PointInPolygon(masterCoordinates, coordinate.Latitude, coordinate.Longitude))
                {
                    inner = false;
                    break;
                }
            }
            return inner;
        }

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
