using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;

namespace GTUService.TIMM
{

    [ServiceContract]
    public interface IPolicy
    {
        [OperationContract, WebGet(UriTemplate = "clientaccesspolicy.xml")]
        Stream GetSilverlightPolicy();
      //  [OperationContract, WebGet(UriTemplate = "/crossdomain.xml")]
      //  Stream GetFlashPolicy();
    }

}