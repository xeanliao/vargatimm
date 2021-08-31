﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test.GTUUpdate {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GTU", Namespace="http://schemas.datacontract.org/2004/07/GTUService.TIMM")]
    [System.SerializableAttribute()]
    public partial class GTU : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int AccuracyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int AreaCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CellIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Test.GTUUpdate.Coordinate CurrentCoordinateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long GPSFixField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int HeadingField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IPAddressField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int LocationIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int NetworkCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Test.GTUUpdate.PowerInfo PowerInfoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime ReceivedTimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime SendTimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double SpeedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VersionField;
        
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
        public int Accuracy {
            get {
                return this.AccuracyField;
            }
            set {
                if ((this.AccuracyField.Equals(value) != true)) {
                    this.AccuracyField = value;
                    this.RaisePropertyChanged("Accuracy");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int AreaCode {
            get {
                return this.AreaCodeField;
            }
            set {
                if ((this.AreaCodeField.Equals(value) != true)) {
                    this.AreaCodeField = value;
                    this.RaisePropertyChanged("AreaCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CellID {
            get {
                return this.CellIDField;
            }
            set {
                if ((this.CellIDField.Equals(value) != true)) {
                    this.CellIDField = value;
                    this.RaisePropertyChanged("CellID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Code {
            get {
                return this.CodeField;
            }
            set {
                if ((object.ReferenceEquals(this.CodeField, value) != true)) {
                    this.CodeField = value;
                    this.RaisePropertyChanged("Code");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Count {
            get {
                return this.CountField;
            }
            set {
                if ((this.CountField.Equals(value) != true)) {
                    this.CountField = value;
                    this.RaisePropertyChanged("Count");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Test.GTUUpdate.Coordinate CurrentCoordinate {
            get {
                return this.CurrentCoordinateField;
            }
            set {
                if ((object.ReferenceEquals(this.CurrentCoordinateField, value) != true)) {
                    this.CurrentCoordinateField = value;
                    this.RaisePropertyChanged("CurrentCoordinate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long GPSFix {
            get {
                return this.GPSFixField;
            }
            set {
                if ((this.GPSFixField.Equals(value) != true)) {
                    this.GPSFixField = value;
                    this.RaisePropertyChanged("GPSFix");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Heading {
            get {
                return this.HeadingField;
            }
            set {
                if ((this.HeadingField.Equals(value) != true)) {
                    this.HeadingField = value;
                    this.RaisePropertyChanged("Heading");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ID {
            get {
                return this.IDField;
            }
            set {
                if ((this.IDField.Equals(value) != true)) {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string IPAddress {
            get {
                return this.IPAddressField;
            }
            set {
                if ((object.ReferenceEquals(this.IPAddressField, value) != true)) {
                    this.IPAddressField = value;
                    this.RaisePropertyChanged("IPAddress");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int LocationID {
            get {
                return this.LocationIDField;
            }
            set {
                if ((this.LocationIDField.Equals(value) != true)) {
                    this.LocationIDField = value;
                    this.RaisePropertyChanged("LocationID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NetworkCode {
            get {
                return this.NetworkCodeField;
            }
            set {
                if ((this.NetworkCodeField.Equals(value) != true)) {
                    this.NetworkCodeField = value;
                    this.RaisePropertyChanged("NetworkCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Test.GTUUpdate.PowerInfo PowerInfo {
            get {
                return this.PowerInfoField;
            }
            set {
                if ((this.PowerInfoField.Equals(value) != true)) {
                    this.PowerInfoField = value;
                    this.RaisePropertyChanged("PowerInfo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime ReceivedTime {
            get {
                return this.ReceivedTimeField;
            }
            set {
                if ((this.ReceivedTimeField.Equals(value) != true)) {
                    this.ReceivedTimeField = value;
                    this.RaisePropertyChanged("ReceivedTime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime SendTime {
            get {
                return this.SendTimeField;
            }
            set {
                if ((this.SendTimeField.Equals(value) != true)) {
                    this.SendTimeField = value;
                    this.RaisePropertyChanged("SendTime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Speed {
            get {
                return this.SpeedField;
            }
            set {
                if ((this.SpeedField.Equals(value) != true)) {
                    this.SpeedField = value;
                    this.RaisePropertyChanged("Speed");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Version {
            get {
                return this.VersionField;
            }
            set {
                if ((object.ReferenceEquals(this.VersionField, value) != true)) {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Coordinate", Namespace="http://schemas.datacontract.org/2004/07/GTUService.TIMM")]
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PowerInfo", Namespace="http://schemas.datacontract.org/2004/07/GTUService.TIMM")]
    public enum PowerInfo : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ON = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OFF = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Low = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UnKnown = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="GTUUpdate.IGTUUpdateService")]
    public interface IGTUUpdateService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGTUUpdateService/UpdateGTU", ReplyAction="http://tempuri.org/IGTUUpdateService/UpdateGTUResponse")]
        void UpdateGTU(string sGTUCode, Test.GTUUpdate.GTU oGTU);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IGTUUpdateServiceChannel : Test.GTUUpdate.IGTUUpdateService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class GTUUpdateServiceClient : System.ServiceModel.ClientBase<Test.GTUUpdate.IGTUUpdateService>, Test.GTUUpdate.IGTUUpdateService {
        
        public GTUUpdateServiceClient() {
        }
        
        public GTUUpdateServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GTUUpdateServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GTUUpdateServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GTUUpdateServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void UpdateGTU(string sGTUCode, Test.GTUUpdate.GTU oGTU) {
            base.Channel.UpdateGTU(sGTUCode, oGTU);
        }
    }
}