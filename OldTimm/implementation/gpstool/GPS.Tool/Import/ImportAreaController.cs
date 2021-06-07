using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Data;
using GPS.Tool.Data;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace GPS.Tool.Import
{
    public abstract class ImportAreaController
    {
        public delegate void ImportController(bool isSuccess, int total,  string message,string file);
        public event ImportController Messaging;

        public delegate void StartImportContraller(string message);
        public event StartImportContraller StartMessaging;

        public delegate void ShowShapeContraller(string filePath);
        public event ShowShapeContraller ShapeShowing;

        public Thread thread;

        public Dictionary<string, int> resultDict = new Dictionary<string, int>
            {
                {ImportResultType.successful.ToString(), 0},
                {ImportResultType.failed.ToString(), 0}
            };

        public Dictionary<string, string> errorDict = new Dictionary<string, string>();

        public bool importResult = false;

        public void StartThread(Dictionary<FileInfo, FileInfo> fileDictionary)
        {
            thread = new Thread(new ParameterizedThreadStart(Importting));
            thread.Start(fileDictionary);
        }

        protected abstract void Importting(Object data);

        protected void SendMessage(bool success, int total, string message,string file)
        {
            if (Messaging != null)
            {
                Messaging(success, total, message, file);
            }
        }

        protected void SendStartMessage(string message)
        {
            if (StartMessaging != null)
            {
                StartMessaging(message);
            }
        }

        protected void ShowShape(string filePath)
        {
            if (ShapeShowing != null)
            {
                ShapeShowing(filePath);
            }
        }

        protected void SendResultMessage()
        {
            StringBuilder strMsg = new StringBuilder();
            strMsg.Append(ImportResultType.successful.ToString());
            strMsg.Append(":");
            strMsg.Append(resultDict[ImportResultType.successful.ToString()].ToString());
            strMsg.Append(" ");
            strMsg.Append(ImportResultType.failed.ToString());
            strMsg.Append(":");
            strMsg.Append(resultDict[ImportResultType.failed.ToString()].ToString());
            strMsg.Append(". ");
            if (resultDict[ImportResultType.failed.ToString()] > 0)
            {
                strMsg.Append("Please look at the log file.");
            }
            SendMessage(true, -1, strMsg.ToString(), String.Empty);
        }

        protected void WriteErrorLog()
        {
            StringBuilder strError = new StringBuilder();
            foreach (KeyValuePair<string, string> error in errorDict)
            {
                strError.Append(error.Key);
                strError.Append(":");
                strError.Append(error.Value);
                strError.Append("\r\n");
            }
            Logger.Write(strError.ToString());
        }

        protected void SetSuccessMessage(
            Dictionary<string, int> resultDict,
            ref bool importResult)
        {
            resultDict[ImportResultType.successful.ToString()]++;
            importResult = true;
        }

        protected void SetFailedMessage(
            Dictionary<string, int> resultDict,
            Dictionary<string, string> errorDict,
            ref bool importResult,
            string errorKey,
            string errorMsg)
        {
            resultDict[ImportResultType.failed.ToString()]++;
            importResult = false;
            errorDict[errorKey] = errorMsg;   
        }
    }
}
