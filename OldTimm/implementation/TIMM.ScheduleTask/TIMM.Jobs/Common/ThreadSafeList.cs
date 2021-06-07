using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMM.Jobs.Common
{
    public class ThreadSafeList<T>
    {
        private List<T> m_List = new List<T>();
        private object m_Locker = new object();

        public void Add(T value)
        {
            lock (m_Locker)
            {
                m_List.Add(value);
            }
        }

        public bool TryGet(int index, out T value)
        {
            lock (m_Locker)
            {
                if (index < m_List.Count)
                {
                    value = m_List[index];
                    return true;
                }
                value = default(T);
                return false;
            }
        }

        public List<T> CopyList()
        {
            lock (m_Locker)
            {
                T[] result = new T[m_List.Count];
                m_List.CopyTo(result);
                return new List<T>(result);
            }
        }
    }
}
