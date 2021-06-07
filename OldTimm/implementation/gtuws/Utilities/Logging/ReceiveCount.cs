using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.Utilities.Logging
{
    public class ReceiveCount
    {
        public static object syncRoot = new object();
        private int _currentNumber;

        public int CurrentNumber
        {
            get { return _currentNumber; }
            set { _currentNumber = value; }
        }

        public void IncreaseCount()
        {
            lock(ReceiveCount.syncRoot)
            {
                _currentNumber++;
            }
        }
         
    }
}
