using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.ModelLayer.Common
{
    public class CommEnums
    {
        /// <summary>
        /// Support 3 Report Modes
        /// </summary>
        public enum ReportMode
        {
            /// <summary>
            /// Use GPRS as default, SMS for the backup
            /// </summary>
            DefaultOnGPRS = 0,
            /// <summary>
            /// Force to use GPRS
            /// </summary>
            ForceOnGPRS = 1,
            /// <summary>
            /// Force to use SMS
            /// </summary>
            ForceOnSMS = 2
        }

        /// <summary>
        /// 3 Connection Mode
        /// </summary>
        public enum ActiveSession
        {
            /// <summary>
            /// The connection is based on TCP protocol. to connect the platform every time when it wants to send data 
            /// and it will shut down the connection when the terminal finishses data sending
            /// </summary>
            TCPReconnectMode = 0,
            /// <summary>
            /// The connection is based on TCP protocol. The terminal will connnect to the platform and maintain the connection.
            /// </summary>
            TCPContinuousMode = 1,
            /// <summary>
            /// The terminal will send data to the platform by UDP protocol.
            /// </summary>
            UDPMode = 2
        }

        /// <summary>
        /// Base on the configuration the terminal will act as 2 steps when it receive an incoming call
        /// </summary>
        public enum LocationByCall
        {
            /// <summary>
            /// Disconnect the call
            /// </summary>
            DisconnectCall = 0,
            /// <summary>
            /// Report the postion and the incoming phone number to the platform
            /// </summary>
            ReportPostionAndPhoneNumber = 1
        }

        /// <summary>
        /// Over speed alarm enable
        /// </summary>
        public enum SpeedWarnEnable
        {
            /// <summary>
            /// Disable over speed alarm funtion
            /// </summary>
            Disable = 0,
            /// <summary>
            /// Enable over speed alarm function
            /// </summary>
            Enable = 1
        }

        /// <summary>
        /// Power down the terminal
        /// </summary>
        public enum PowerKeyEnable
        {
            /// <summary>
            /// Can not power down the terminal
            /// </summary>
            Disable = 0,
            /// <summary>
            /// Power down the terminal
            /// </summary>
            Enable = 1
        }

        /// <summary>
        /// Function Key Enable
        /// </summary>
        public enum FunctionKeyEnable
        {
            /// <summary>
            /// the function is disable
            /// </summary>
            Disable = 0,
            /// <summary>
            /// the fucntion is enalbe
            /// </summary>
            Enable = 1
        }

        /// <summary>
        /// 
        /// </summary>
        public enum FunctionKeyMode
        {
            /// <summary>
            /// Geo-Fence Mode
            /// </summary>
            GeoFenceMode = 0,
            /// <summary>
            /// SOS Mode
            /// </summary>
            SOSMode = 1
        }

        /// <summary>
        /// 
        /// </summary>
        public enum MovementDetectMode
        {
            Disable = 0,
            Enable = 1
        }

        /// <summary>
        /// 
        /// </summary>
        public enum FullChargeBoot
        {
            FullyPowerOn = 0,
            ChargeMode = 1
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ZoneAlert
        {
            LeaveSafeZone = 0,
            EnterSafeZone = 1
        }

        /// <summary>
        /// 
        /// </summary>
        public enum GPSFix
        {
            SuccessfullyGPSFixing = 1,
            FailedOnGPSFixing = 0
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ActiveGeoFence
        {
            Disable = 0,
            Enable = 1
        }

        /// <summary>
        /// 
        /// </summary>
        public enum GeoFenceType
        {
            ReportWhenEntersGeoFence = 1,
            ReportWhenExitsFromGeoFence = 2,
            ReportWhenEntersOrExitsFromGeoFence = 3
        }

        /// <summary>
        /// 
        /// </summary>
        public enum AckType
        {
            TCPPacket = 0,
            UDPPacket = 1,
            SMSPDU = 4
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ChargerConnected
        {
            ChargerIsInserted = 0,
            ChargerIsNotConnected = 1
        }

        /// <summary>
        /// 
        /// </summary>
        public enum CommandOption
        {
            GetLatestTimeofGPSFixingSuccessfully = 0,
            RequireReportCurrentPostionImmediately = 1,
            GetTerminalConfiguration = 2,
            RebootTerminal = 3,
            ResetAllToFactorySetting = 4,
            GetICCID = 5,
            GetGSMSignalLevel = 6,
            GetSoftwareVersion = 7,
            GetHardwareVersion = 8,
            ConfigureOverSpeedAlarm = 9,
            GetBatteryLevel = 10
        }
    }
}
