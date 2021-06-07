using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.ServiceModel.Dispatcher;
using log4net;
using System.Reflection;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Net;

namespace GPS.Website.ExceptionHandler
{
    public class JsonGlobalExceptionHandler : BehaviorExtensionElement, IErrorHandler, IEndpointBehavior
    {
        protected override object CreateBehavior()
        {
            return new JsonGlobalExceptionHandler();
        }

        public override Type BehaviorType
        {
            get { return GetType(); }
        }

        public bool HandleError(Exception error)
        {
            //leave some handled exception send to client
            if (error is MyException)
            {
                return false;
            }
            try
            {
                var logger = LogManager.GetLogger(GetType());
                logger.Error("WCF Unhandle Error ", error);
            }
            catch { }
            return true;
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            string errorMessage = ConfigurationManager.AppSettings["UnhandleExceptionMessage"];
            var faultException = new FaultException<string>(errorMessage, new FaultReason(errorMessage));
            var messageFault = faultException.CreateMessageFault();
            fault = Message.CreateMessage(version, messageFault, null);
            // tell WCF to use JSON encoding rather than default XML
            WebBodyFormatMessageProperty wbf = new WebBodyFormatMessageProperty(WebContentFormat.Json);

            // Add the formatter to the fault
            fault.Properties.Add(WebBodyFormatMessageProperty.Name, wbf);

            //Modify response
            HttpResponseMessageProperty rmp = new HttpResponseMessageProperty();

            // return custom error code, 400.
            rmp.StatusCode = System.Net.HttpStatusCode.BadRequest;
            rmp.StatusDescription = "Bad request";

            //Mark the jsonerror and json content
            rmp.Headers[HttpResponseHeader.ContentType] = "application/json";
            rmp.Headers["jsonerror"] = "true";

            //Add to fault
            fault.Properties.Add(HttpResponseMessageProperty.Name, rmp);

        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            return;   
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            return;
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(this);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            return;
        }
    }
}