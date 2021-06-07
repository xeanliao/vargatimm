using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;

[DataContract]
public class BasicCampaignProperties {
    /// <summary>
    /// The unique identity of the campaign.
    /// </summary>
    [DataMember(IsRequired = true)]
    public Int32 Id {
        get;
        set;
    }

    /// <summary>
    /// The client name of the campaign.
    /// </summary>
    [DataMember(IsRequired = true)]
    public String ClientName {
        get;
        set;
    }

    /// <summary>
    /// The contact name of the campaign.
    /// </summary>
    [DataMember(IsRequired = true)]
    public String ContactName {
        get;
        set;
    }

    /// <summary>
    /// The client code of the campaign.
    /// </summary>
    [DataMember(IsRequired = true)]
    public String ClientCode {
        get;
        set;
    }

    /// <summary>
    /// The logo of the campaign.
    /// </summary>
    //[DataMember(IsRequired = true)]
    //public String Logo {
    //    get;
    //    set;
    //}

    /// <summary>
    /// The area description of the campaign.
    /// </summary>
    [DataMember(IsRequired = true)]
    public String AreaDescription {
        get;
        set;
    }

    /// <summary>
    /// The creation date of the campaign.
    /// </summary>
    [DataMember(IsRequired = true)]
    public String Date {
        get;
        set;
    }

    /// <summary>
    /// The sequence of the campaign.
    /// </summary>
    [DataMember(IsRequired = true)]
    public Int32 Sequence {
        get;
        set;
    }
}
