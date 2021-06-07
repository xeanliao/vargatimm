﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3620
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test.Geofencing {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Coordinate", Namespace="http://schemas.datacontract.org/2004/07/WHYTAlgorithmService.Geo")]
    [System.SerializableAttribute()]
    public partial class Coordinate : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double AltitudeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LatitudeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LongitudeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Altitude {
            get {
                return this.AltitudeField;
            }
            set {
                if ((this.AltitudeField.Equals(value) != true)) {
                    this.AltitudeField = value;
                    this.RaisePropertyChanged("Altitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Latitude {
            get {
                return this.LatitudeField;
            }
            set {
                if ((this.LatitudeField.Equals(value) != true)) {
                    this.LatitudeField = value;
                    this.RaisePropertyChanged("Latitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Longitude {
            get {
                return this.LongitudeField;
            }
            set {
                if ((this.LongitudeField.Equals(value) != true)) {
                    this.LongitudeField = value;
                    this.RaisePropertyChanged("Longitude");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Geofencing.IGeofencing")]
    public interface IGeofencing {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGeofencing/IsInTheArea", ReplyAction="http://tempuri.org/IGeofencing/IsInTheAreaResponse")]
        bool IsInTheArea(Test.Geofencing.Coordinate oLocation, string sGUID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGeofencing/IsInTheDNDArea", ReplyAction="http://tempuri.org/IGeofencing/IsInTheDNDAreaResponse")]
        string IsInTheDNDArea(Test.Geofencing.Coordinate oLocation, int[] ndAreaIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGeofencing/RegisterArea", ReplyAction="http://tempuri.org/IGeofencing/RegisterAreaResponse")]
        string RegisterArea(Test.Geofencing.Coordinate[] oArea);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGeofencing/UpdateArea", ReplyAction="http://tempuri.org/IGeofencing/UpdateAreaResponse")]
        bool UpdateArea(string sGUID, Test.Geofencing.Coordinate[] oArea);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGeofencing/RemoveArea", ReplyAction="http://tempuri.org/IGeofencing/RemoveAreaResponse")]
        bool RemoveArea(string sGUID);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IGeofencingChannel : Test.Geofencing.IGeofencing, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class GeofencingClient : System.ServiceModel.ClientBase<Test.Geofencing.IGeofencing>, Test.Geofencing.IGeofencing {
        
        public GeofencingClient() {
        }
        
        public GeofencingClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GeofencingClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GeofencingClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GeofencingClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool IsInTheArea(Test.Geofencing.Coordinate oLocation, string sGUID) {
            return base.Channel.IsInTheArea(oLocation, sGUID);
        }
        
        public string IsInTheDNDArea(Test.Geofencing.Coordinate oLocation, int[] ndAreaIds) {
            return base.Channel.IsInTheDNDArea(oLocation, ndAreaIds);
        }
        
        public string RegisterArea(Test.Geofencing.Coordinate[] oArea) {
            return base.Channel.RegisterArea(oArea);
        }
        
        public bool UpdateArea(string sGUID, Test.Geofencing.Coordinate[] oArea) {
            return base.Channel.UpdateArea(sGUID, oArea);
        }
        
        public bool RemoveArea(string sGUID) {
            return base.Channel.RemoveArea(sGUID);
        }
    }
}
