using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;

namespace TIMM.QueueUpdater
{
    public class QueueUpdater<V> where V:class
    {
        //Define the defaul length of the Q for both workingQ and backupQ
        const long DefaultQCount = 1024 * 10;
        Queue<V> _WorkingQueue = new Queue<V>();
        Queue<V> _FailedQueue = new Queue<V>();
        private ManualResetEvent _hasNewWorkingItemEvent = new ManualResetEvent(false);
        private ManualResetEvent _hasNewBackupItemEvent = new ManualResetEvent(false);
        Thread[] _OperatorThread;
        private int _OperatorThreadCount = 2; 
        Thread _BackupThread;
        private static bool _bWorking = true;

        private long nProcessQLength = DefaultQCount;//Defualt value, also minimum value
        private long nBackupListLength = DefaultQCount;//Defualt value, also minimum value

        public delegate bool ProcessQHandler(V Value);
        private ProcessQHandler _ProcessWorkingQ = null;
        private ProcessQHandler _ProcessFailedQ = null;

        private int threadJoinSeconds = 10 * 1000; //default value

        public long ProcessQLength
        {
            get { return nProcessQLength; }
            set { nProcessQLength = value > DefaultQCount ? value : DefaultQCount; }
        }

        public long BackupQLength
        {
            get { return nBackupListLength; }
            set { nBackupListLength = value > DefaultQCount ? value : DefaultQCount; }
        }

        public void Init(ProcessQHandler ProcessWorkingQ, ProcessQHandler ProcessFailedQ)
        {
            _ProcessWorkingQ = ProcessWorkingQ;
            _ProcessFailedQ = ProcessFailedQ;
            Init();
        }

       
        public void Init(ProcessQHandler ProcessWorkingQ, int ThreadCount)
        {
            _ProcessWorkingQ = ProcessWorkingQ;
            _OperatorThreadCount = ThreadCount;
            Init();
        }

        public void Init()
        {
            _OperatorThread = new Thread[_OperatorThreadCount];
            for (int i = 0; i < _OperatorThreadCount; i++)
            {
                _OperatorThread[i] = new Thread(new ThreadStart(this.OperatorProc));
                _OperatorThread[i].Start();
            }
            _BackupThread = new Thread(new ThreadStart(this.FailedQProc));
            _BackupThread.Start();
            try
            {
                threadJoinSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["ThreadJoinSeconds"].ToString());
            }
            catch
            {
                System.Diagnostics.Trace.TraceError("\n Cannot get the setting for ThreadJoinSeconds, using default valu\n");
            }
        } 



        public void AddQ(V Value)
        {
            System.Diagnostics.Trace.TraceInformation("Begin to Update \n");
            lock (_WorkingQueue)
            {
                _WorkingQueue.Enqueue(Value);
                _hasNewWorkingItemEvent.Set();
            }
            System.Diagnostics.Trace.TraceInformation("End UpdateGTU \n");
        }
        private void AddFailedQ(V Value)
        {
            System.Diagnostics.Trace.TraceInformation("Begin to add FailedQ \n");
            lock (_FailedQueue)
            {
                _FailedQueue.Enqueue(Value);
                _hasNewBackupItemEvent.Set();
            }
            System.Diagnostics.Trace.TraceInformation("End to add FailedQ \n");
        }
        private void FailedQProc()
        {
            while (_bWorking)
            {
                _hasNewBackupItemEvent.Reset();
                if (_FailedQueue.Count > 0)
                {
                    V Value = null;
                    lock (_FailedQueue)
                    {
                        Value = _FailedQueue.Dequeue();
                    }
                    if (_ProcessFailedQ != null && Value != null)
                        if (!_ProcessFailedQ(Value))
                        {
                            System.Diagnostics.Trace.TraceInformation("_ProcessFailedQ handler return false \n");
                        }
                }
                _hasNewBackupItemEvent.WaitOne();
            }
        }
        public void Stop()
        {
            _bWorking = false;
            // Signal the Listening main thread to Stop.
            _hasNewWorkingItemEvent.Set();
            _hasNewBackupItemEvent.Set();

            //
            for (int i = 0; i < _OperatorThreadCount; i++)
            {
                if (!_OperatorThread[i].Join(threadJoinSeconds))  // Wait for n seconds, TODO: change to be configurable
                {
                    _OperatorThread[i].Abort(); // kill the thread
                    _OperatorThread[i].Join();
                }
            }

            if (_BackupThread != null)
            {
                if (!_BackupThread.Join(threadJoinSeconds))  // Wait for n seconds, TODO: change to be configurable
                {
                    _BackupThread.Abort(); // kill the thread
                    _BackupThread.Join();
                }
            }

        }

        private void OperatorProc()
        {
            while (_bWorking)
            {
                _hasNewWorkingItemEvent.Reset();

                while (_WorkingQueue.Count > 0 && _bWorking)
                {
                    V Value = null;
                    lock (_WorkingQueue)
                    {
                        if (_WorkingQueue.Count > 0) //double check after locking
                        {
                            Value = _WorkingQueue.Dequeue();
                        }
                    }
                    if (_ProcessWorkingQ != null && Value != null)
                    {
                        if (!_ProcessWorkingQ(Value))
                        {
                            AddFailedQ(Value);
                        }
                    }
                }
                _hasNewWorkingItemEvent.WaitOne();
            }
        }

    }


}
