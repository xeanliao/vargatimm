using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;

namespace GPS.DomainLayer.ActiveRecordDatabase
{
    /// <summary>
    /// This class is root of assistant database . Only for config another database access
    /// see more http://stw.castleproject.org/Active%20Record.Accessing%20more%20than%20one%20database.ashx
    /// </summary>
    public abstract class AssistantDatabase : ActiveRecordBase
    {
    }
}
