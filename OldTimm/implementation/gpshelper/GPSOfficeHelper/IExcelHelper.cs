using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace GPSOfficeHelper
{
    [ServiceContract]
    public interface IExcelHelper
    {
        [OperationContract]

        GtuRecord[] ReadGtuRecord(string fileName);

        [OperationContract]
        string TestHello(string text);

        [OperationContract]
        [ServiceKnownType(typeof(AreaRecord))]
        bool WriteRecords(string fileName, string templateFile, object[] records);

        [OperationContract]
        AreaRecord[] ReadAreaRecords(string fileName);
        //[ServiceKnownType(typeof(AreaRecord))]
        //object[] ReadRecords<T>(string fileName);
    }
}