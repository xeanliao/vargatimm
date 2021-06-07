using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using log4net;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace GPS.Website.ExceptionHandler
{
    public class ServiceGlobalExceptionHandler : BehaviorExtensionElement, IErrorHandler, IServiceBehavior
    {
        protected override object CreateBehavior()
        {
            return new ServiceGlobalExceptionHandler();
        }

        public override Type BehaviorType
        {
            get { return GetType(); }
        }

        public bool HandleError(Exception error)
        {
            try
            {
                var logger = LogManager.GetLogger(GetType());
                logger.Error("WCF Unhandle Error ", error);
            }
            catch{}
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            //hide exception details to clients
            string errorMessage = ConfigurationManager.AppSettings["UnhandleExceptionMessage"];            
            var faultException = new FaultException<string>(errorMessage, new FaultReason(errorMessage));
            var messageFault = faultException.CreateMessageFault();
            fault = Message.CreateMessage(version, messageFault, null);
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channDisp in serviceHostBase.ChannelDispatchers)
            {
                channDisp.ErrorHandlers.Add(this);
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> serviceEndPoints, BindingParameterCollection bindingParameters)
        {
            return;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //do nothing.
        }

    }
}