﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3607
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GTU.DataLayer
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[System.Data.Linq.Mapping.DatabaseAttribute(Name="GTUTracking")]
	public partial class GTUTrackDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertTrackItem(TrackItem instance);
    partial void UpdateTrackItem(TrackItem instance);
    partial void DeleteTrackItem(TrackItem instance);
    #endregion
		
		public GTUTrackDataContext() : 
				base(global::GTU.DataLayer.Properties.Settings.Default.GTUTrackingConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public GTUTrackDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GTUTrackDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GTUTrackDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GTUTrackDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<TrackItem> TrackItems
		{
			get
			{
				return this.GetTable<TrackItem>();
			}
		}
	}
	
	[Table(Name="dbo.TrackItem")]
	public partial class TrackItem : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _TrackId;
		
		private string _UniqueId;
		
		private string _longitude;
		
		private string _Latitude;
		
		private System.Nullable<System.DateTime> _SendTime;
		
		private System.Nullable<bool> _InDeliverable;
		
		private System.Nullable<bool> _InNonDeliverable;
		
		private System.Nullable<bool> _StayAlarm;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnTrackIdChanging(int value);
    partial void OnTrackIdChanged();
    partial void OnUniqueIdChanging(string value);
    partial void OnUniqueIdChanged();
    partial void OnlongitudeChanging(string value);
    partial void OnlongitudeChanged();
    partial void OnLatitudeChanging(string value);
    partial void OnLatitudeChanged();
    partial void OnSendTimeChanging(System.Nullable<System.DateTime> value);
    partial void OnSendTimeChanged();
    partial void OnInDeliverableChanging(System.Nullable<bool> value);
    partial void OnInDeliverableChanged();
    partial void OnInNonDeliverableChanging(System.Nullable<bool> value);
    partial void OnInNonDeliverableChanged();
    partial void OnStayAlarmChanging(System.Nullable<bool> value);
    partial void OnStayAlarmChanged();
    #endregion
		
		public TrackItem()
		{
			OnCreated();
		}
		
		[Column(Storage="_TrackId", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int TrackId
		{
			get
			{
				return this._TrackId;
			}
			set
			{
				if ((this._TrackId != value))
				{
					this.OnTrackIdChanging(value);
					this.SendPropertyChanging();
					this._TrackId = value;
					this.SendPropertyChanged("TrackId");
					this.OnTrackIdChanged();
				}
			}
		}
		
		[Column(Storage="_UniqueId", DbType="NVarChar(50)")]
		public string UniqueId
		{
			get
			{
				return this._UniqueId;
			}
			set
			{
				if ((this._UniqueId != value))
				{
					this.OnUniqueIdChanging(value);
					this.SendPropertyChanging();
					this._UniqueId = value;
					this.SendPropertyChanged("UniqueId");
					this.OnUniqueIdChanged();
				}
			}
		}
		
		[Column(Storage="_longitude", DbType="NVarChar(50)")]
		public string longitude
		{
			get
			{
				return this._longitude;
			}
			set
			{
				if ((this._longitude != value))
				{
					this.OnlongitudeChanging(value);
					this.SendPropertyChanging();
					this._longitude = value;
					this.SendPropertyChanged("longitude");
					this.OnlongitudeChanged();
				}
			}
		}
		
		[Column(Storage="_Latitude", DbType="NVarChar(50)")]
		public string Latitude
		{
			get
			{
				return this._Latitude;
			}
			set
			{
				if ((this._Latitude != value))
				{
					this.OnLatitudeChanging(value);
					this.SendPropertyChanging();
					this._Latitude = value;
					this.SendPropertyChanged("Latitude");
					this.OnLatitudeChanged();
				}
			}
		}
		
		[Column(Storage="_SendTime", DbType="DateTime")]
		public System.Nullable<System.DateTime> SendTime
		{
			get
			{
				return this._SendTime;
			}
			set
			{
				if ((this._SendTime != value))
				{
					this.OnSendTimeChanging(value);
					this.SendPropertyChanging();
					this._SendTime = value;
					this.SendPropertyChanged("SendTime");
					this.OnSendTimeChanged();
				}
			}
		}
		
		[Column(Storage="_InDeliverable", DbType="Bit")]
		public System.Nullable<bool> InDeliverable
		{
			get
			{
				return this._InDeliverable;
			}
			set
			{
				if ((this._InDeliverable != value))
				{
					this.OnInDeliverableChanging(value);
					this.SendPropertyChanging();
					this._InDeliverable = value;
					this.SendPropertyChanged("InDeliverable");
					this.OnInDeliverableChanged();
				}
			}
		}
		
		[Column(Storage="_InNonDeliverable", DbType="Bit")]
		public System.Nullable<bool> InNonDeliverable
		{
			get
			{
				return this._InNonDeliverable;
			}
			set
			{
				if ((this._InNonDeliverable != value))
				{
					this.OnInNonDeliverableChanging(value);
					this.SendPropertyChanging();
					this._InNonDeliverable = value;
					this.SendPropertyChanged("InNonDeliverable");
					this.OnInNonDeliverableChanged();
				}
			}
		}
		
		[Column(Storage="_StayAlarm", DbType="Bit")]
		public System.Nullable<bool> StayAlarm
		{
			get
			{
				return this._StayAlarm;
			}
			set
			{
				if ((this._StayAlarm != value))
				{
					this.OnStayAlarmChanging(value);
					this.SendPropertyChanging();
					this._StayAlarm = value;
					this.SendPropertyChanged("StayAlarm");
					this.OnStayAlarmChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591