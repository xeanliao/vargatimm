﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3620
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GTUService.TIMM
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GTU", Namespace="http://schemas.datacontract.org/2004/07/GTUService.TIMM")]
    public partial class GTU : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int AccuracyField;
        
        private int AreaCodeField;
        
        private int CellIDField;
        
        private string CodeField;
        
        private int CountField;
        
        private GTUService.TIMM.Coordinate CurrentCoordinateField;
        
        private double DistanceField;
        
        private long GPSFixField;
        
        private int HeadingField;
        
        private int IDField;
        
        private string IPAddressField;
        
        private int LocationIDField;
        
        private int NetworkCodeField;
        
        private GTUService.TIMM.PowerInfo PowerInfoField;
        
        private System.DateTime ReceivedTimeField;
        
        private System.DateTime SendTimeField;
        
        private double SpeedField;
        
        private GTUService.TIMM.Status StatusField;
        
        private string VersionField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Accuracy
        {
            get
            {
                return this.AccuracyField;
            }
            set
            {
                this.AccuracyField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int AreaCode
        {
            get
            {
                return this.AreaCodeField;
            }
            set
            {
                this.AreaCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CellID
        {
            get
            {
                return this.CellIDField;
            }
            set
            {
                this.CellIDField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Code
        {
            get
            {
                return this.CodeField;
            }
            set
            {
                this.CodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Count
        {
            get
            {
                return this.CountField;
            }
            set
            {
                this.CountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public GTUService.TIMM.Coordinate CurrentCoordinate
        {
            get
            {
                return this.CurrentCoordinateField;
            }
            set
            {
                this.CurrentCoordinateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Distance
        {
            get
            {
                return this.DistanceField;
            }
            set
            {
                this.DistanceField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long GPSFix
        {
            get
            {
                return this.GPSFixField;
            }
            set
            {
                this.GPSFixField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Heading
        {
            get
            {
                return this.HeadingField;
            }
            set
            {
                this.HeadingField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ID
        {
            get
            {
                return this.IDField;
            }
            set
            {
                this.IDField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string IPAddress
        {
            get
            {
                return this.IPAddressField;
            }
            set
            {
                this.IPAddressField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int LocationID
        {
            get
            {
                return this.LocationIDField;
            }
            set
            {
                this.LocationIDField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NetworkCode
        {
            get
            {
                return this.NetworkCodeField;
            }
            set
            {
                this.NetworkCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public GTUService.TIMM.PowerInfo PowerInfo
        {
            get
            {
                return this.PowerInfoField;
            }
            set
            {
                this.PowerInfoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime ReceivedTime
        {
            get
            {
                return this.ReceivedTimeField;
            }
            set
            {
                this.ReceivedTimeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime SendTime
        {
            get
            {
                return this.SendTimeField;
            }
            set
            {
                this.SendTimeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Speed
        {
            get
            {
                return this.SpeedField;
            }
            set
            {
                this.SpeedField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public GTUService.TIMM.Status Status
        {
            get
            {
                return this.StatusField;
            }
            set
            {
                this.StatusField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Version
        {
            get
            {
                return this.VersionField;
            }
            set
            {
                this.VersionField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Coordinate", Namespace="http://schemas.datacontract.org/2004/07/GTUService.TIMM")]
    public partial class Coordinate : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private double AltitudeField;
        
        private double LatitudeField;
        
        private double LongitudeField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Altitude
        {
            get
            {
                return this.AltitudeField;
            }
            set
            {
                this.AltitudeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Latitude
        {
            get
            {
                return this.LatitudeField;
            }
            set
            {
                this.LatitudeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Longitude
        {
            get
            {
                return this.LongitudeField;
            }
            set
            {
                this.LongitudeField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PowerInfo", Namespace="http://schemas.datacontract.org/2004/07/GTUService.TIMM")]
    public enum PowerInfo : int
    {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ON = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OFF = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Low = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UnKnown = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Status", Namespace="http://schemas.datacontract.org/2004/07/GTUService.TIMM")]
    public enum Status : int
    {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Normal = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OutBoundary = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Frozen = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OutAndFrozen = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UnKnown = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InDNDArea = 5,
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IGTUUpdateService")]
public interface IGTUUpdateService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGTUUpdateService/UpdateGTU", ReplyAction="http://tempuri.org/IGTUUpdateService/UpdateGTUResponse")]
    void UpdateGTU(string sGTUCode, GTUService.TIMM.GTU oGTU);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface IGTUUpdateServiceChannel : IGTUUpdateService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public partial class GTUUpdateServiceClient : System.ServiceModel.ClientBase<IGTUUpdateService>, IGTUUpdateService
{
    
    public GTUUpdateServiceClient()
    {
    }
    
    public GTUUpdateServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public GTUUpdateServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public GTUUpdateServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public GTUUpdateServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public void UpdateGTU(string sGTUCode, GTUService.TIMM.GTU oGTU)
    {
        base.Channel.UpdateGTU(sGTUCode, oGTU);
    }
}
