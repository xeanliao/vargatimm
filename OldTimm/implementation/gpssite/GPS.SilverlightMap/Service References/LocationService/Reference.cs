﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 5.0.61118.0
// 
namespace GPS.SilverlightMap.LocationService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="TIMM.Website.SilverlightServices", ConfigurationName="LocationService.LocationService")]
    public interface LocationService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="TIMM.Website.SilverlightServices/LocationService/PointInPolygon", ReplyAction="TIMM.Website.SilverlightServices/LocationService/PointInPolygonResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.ObjectModel.ObservableCollection<double>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.ObjectModel.ObservableCollection<object>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<System.Collections.ObjectModel.ObservableCollection<object>, System.Collections.ObjectModel.ObservableCollection<double>>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<object>>))]
        System.IAsyncResult BeginPointInPolygon(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates, System.Collections.ObjectModel.ObservableCollection<double> point, System.Collections.ObjectModel.ObservableCollection<object> gti, System.AsyncCallback callback, object asyncState);
        
        System.Collections.ObjectModel.ObservableCollection<object> EndPointInPolygon(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="TIMM.Website.SilverlightServices/LocationService/PointsInPolygon", ReplyAction="TIMM.Website.SilverlightServices/LocationService/PointsInPolygonResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.ObjectModel.ObservableCollection<double>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.ObjectModel.ObservableCollection<object>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<System.Collections.ObjectModel.ObservableCollection<object>, System.Collections.ObjectModel.ObservableCollection<double>>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<object>>))]
        System.IAsyncResult BeginPointsInPolygon(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates, System.Collections.Generic.Dictionary<System.Collections.ObjectModel.ObservableCollection<object>, System.Collections.ObjectModel.ObservableCollection<double>> points, System.AsyncCallback callback, object asyncState);
        
        System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<object>> EndPointsInPolygon(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface LocationServiceChannel : GPS.SilverlightMap.LocationService.LocationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PointInPolygonCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public PointInPolygonCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.ObjectModel.ObservableCollection<object> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.ObjectModel.ObservableCollection<object>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PointsInPolygonCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public PointsInPolygonCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<object>> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<object>>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LocationServiceClient : System.ServiceModel.ClientBase<GPS.SilverlightMap.LocationService.LocationService>, GPS.SilverlightMap.LocationService.LocationService {
        
        private BeginOperationDelegate onBeginPointInPolygonDelegate;
        
        private EndOperationDelegate onEndPointInPolygonDelegate;
        
        private System.Threading.SendOrPostCallback onPointInPolygonCompletedDelegate;
        
        private BeginOperationDelegate onBeginPointsInPolygonDelegate;
        
        private EndOperationDelegate onEndPointsInPolygonDelegate;
        
        private System.Threading.SendOrPostCallback onPointsInPolygonCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public LocationServiceClient() {
        }
        
        public LocationServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LocationServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LocationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LocationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<PointInPolygonCompletedEventArgs> PointInPolygonCompleted;
        
        public event System.EventHandler<PointsInPolygonCompletedEventArgs> PointsInPolygonCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GPS.SilverlightMap.LocationService.LocationService.BeginPointInPolygon(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates, System.Collections.ObjectModel.ObservableCollection<double> point, System.Collections.ObjectModel.ObservableCollection<object> gti, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginPointInPolygon(coordinates, point, gti, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.ObjectModel.ObservableCollection<object> GPS.SilverlightMap.LocationService.LocationService.EndPointInPolygon(System.IAsyncResult result) {
            return base.Channel.EndPointInPolygon(result);
        }
        
        private System.IAsyncResult OnBeginPointInPolygon(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates = ((System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>>)(inValues[0]));
            System.Collections.ObjectModel.ObservableCollection<double> point = ((System.Collections.ObjectModel.ObservableCollection<double>)(inValues[1]));
            System.Collections.ObjectModel.ObservableCollection<object> gti = ((System.Collections.ObjectModel.ObservableCollection<object>)(inValues[2]));
            return ((GPS.SilverlightMap.LocationService.LocationService)(this)).BeginPointInPolygon(coordinates, point, gti, callback, asyncState);
        }
        
        private object[] OnEndPointInPolygon(System.IAsyncResult result) {
            System.Collections.ObjectModel.ObservableCollection<object> retVal = ((GPS.SilverlightMap.LocationService.LocationService)(this)).EndPointInPolygon(result);
            return new object[] {
                    retVal};
        }
        
        private void OnPointInPolygonCompleted(object state) {
            if ((this.PointInPolygonCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.PointInPolygonCompleted(this, new PointInPolygonCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void PointInPolygonAsync(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates, System.Collections.ObjectModel.ObservableCollection<double> point, System.Collections.ObjectModel.ObservableCollection<object> gti) {
            this.PointInPolygonAsync(coordinates, point, gti, null);
        }
        
        public void PointInPolygonAsync(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates, System.Collections.ObjectModel.ObservableCollection<double> point, System.Collections.ObjectModel.ObservableCollection<object> gti, object userState) {
            if ((this.onBeginPointInPolygonDelegate == null)) {
                this.onBeginPointInPolygonDelegate = new BeginOperationDelegate(this.OnBeginPointInPolygon);
            }
            if ((this.onEndPointInPolygonDelegate == null)) {
                this.onEndPointInPolygonDelegate = new EndOperationDelegate(this.OnEndPointInPolygon);
            }
            if ((this.onPointInPolygonCompletedDelegate == null)) {
                this.onPointInPolygonCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnPointInPolygonCompleted);
            }
            base.InvokeAsync(this.onBeginPointInPolygonDelegate, new object[] {
                        coordinates,
                        point,
                        gti}, this.onEndPointInPolygonDelegate, this.onPointInPolygonCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GPS.SilverlightMap.LocationService.LocationService.BeginPointsInPolygon(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates, System.Collections.Generic.Dictionary<System.Collections.ObjectModel.ObservableCollection<object>, System.Collections.ObjectModel.ObservableCollection<double>> points, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginPointsInPolygon(coordinates, points, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<object>> GPS.SilverlightMap.LocationService.LocationService.EndPointsInPolygon(System.IAsyncResult result) {
            return base.Channel.EndPointsInPolygon(result);
        }
        
        private System.IAsyncResult OnBeginPointsInPolygon(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates = ((System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>>)(inValues[0]));
            System.Collections.Generic.Dictionary<System.Collections.ObjectModel.ObservableCollection<object>, System.Collections.ObjectModel.ObservableCollection<double>> points = ((System.Collections.Generic.Dictionary<System.Collections.ObjectModel.ObservableCollection<object>, System.Collections.ObjectModel.ObservableCollection<double>>)(inValues[1]));
            return ((GPS.SilverlightMap.LocationService.LocationService)(this)).BeginPointsInPolygon(coordinates, points, callback, asyncState);
        }
        
        private object[] OnEndPointsInPolygon(System.IAsyncResult result) {
            System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<object>> retVal = ((GPS.SilverlightMap.LocationService.LocationService)(this)).EndPointsInPolygon(result);
            return new object[] {
                    retVal};
        }
        
        private void OnPointsInPolygonCompleted(object state) {
            if ((this.PointsInPolygonCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.PointsInPolygonCompleted(this, new PointsInPolygonCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void PointsInPolygonAsync(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates, System.Collections.Generic.Dictionary<System.Collections.ObjectModel.ObservableCollection<object>, System.Collections.ObjectModel.ObservableCollection<double>> points) {
            this.PointsInPolygonAsync(coordinates, points, null);
        }
        
        public void PointsInPolygonAsync(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates, System.Collections.Generic.Dictionary<System.Collections.ObjectModel.ObservableCollection<object>, System.Collections.ObjectModel.ObservableCollection<double>> points, object userState) {
            if ((this.onBeginPointsInPolygonDelegate == null)) {
                this.onBeginPointsInPolygonDelegate = new BeginOperationDelegate(this.OnBeginPointsInPolygon);
            }
            if ((this.onEndPointsInPolygonDelegate == null)) {
                this.onEndPointsInPolygonDelegate = new EndOperationDelegate(this.OnEndPointsInPolygon);
            }
            if ((this.onPointsInPolygonCompletedDelegate == null)) {
                this.onPointsInPolygonCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnPointsInPolygonCompleted);
            }
            base.InvokeAsync(this.onBeginPointsInPolygonDelegate, new object[] {
                        coordinates,
                        points}, this.onEndPointsInPolygonDelegate, this.onPointsInPolygonCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override GPS.SilverlightMap.LocationService.LocationService CreateChannel() {
            return new LocationServiceClientChannel(this);
        }
        
        private class LocationServiceClientChannel : ChannelBase<GPS.SilverlightMap.LocationService.LocationService>, GPS.SilverlightMap.LocationService.LocationService {
            
            public LocationServiceClientChannel(System.ServiceModel.ClientBase<GPS.SilverlightMap.LocationService.LocationService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginPointInPolygon(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates, System.Collections.ObjectModel.ObservableCollection<double> point, System.Collections.ObjectModel.ObservableCollection<object> gti, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[3];
                _args[0] = coordinates;
                _args[1] = point;
                _args[2] = gti;
                System.IAsyncResult _result = base.BeginInvoke("PointInPolygon", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.ObjectModel.ObservableCollection<object> EndPointInPolygon(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.ObjectModel.ObservableCollection<object> _result = ((System.Collections.ObjectModel.ObservableCollection<object>)(base.EndInvoke("PointInPolygon", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginPointsInPolygon(System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<double>> coordinates, System.Collections.Generic.Dictionary<System.Collections.ObjectModel.ObservableCollection<object>, System.Collections.ObjectModel.ObservableCollection<double>> points, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[2];
                _args[0] = coordinates;
                _args[1] = points;
                System.IAsyncResult _result = base.BeginInvoke("PointsInPolygon", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<object>> EndPointsInPolygon(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<object>> _result = ((System.Collections.ObjectModel.ObservableCollection<System.Collections.ObjectModel.ObservableCollection<object>>)(base.EndInvoke("PointsInPolygon", _args, result)));
                return _result;
            }
        }
    }
}