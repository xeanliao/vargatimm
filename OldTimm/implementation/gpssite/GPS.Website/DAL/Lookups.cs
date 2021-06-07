using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;

namespace GPS.Website.DAL
{
    public class Lookups
    {
        // EmployeeRoles
        private static SortedList<int, string> _employeeRoles = null;
        public static SortedList<int, string> EmployeeRoles
        {
            get
            { 
                if(_employeeRoles == null)
                {
                    _employeeRoles = new SortedList<int,string>();
                    _employeeRoles.Add(1, "Walker");
                    _employeeRoles.Add(48, "Driver");
                    _employeeRoles.Add(50, "Auditor");
                }
                return _employeeRoles;
            }
        }

        // UserGroups
        private static SortedList<int, string> _userGroups = null;
        public static SortedList<int, string> UserGroups
        {
            get
            {
                if (_userGroups == null)
                {
                    DAL.TimmDomainService timm = new TimmDomainService();
                    IQueryable<DAL.user> users = timm.GetUsers();
                    foreach (DAL.user u in users)
                    {
                        _userGroups.Add(u.Id, u.FullName);
                    }
                }

                return _userGroups;
            }
        
        }

        public static string[,] USStates
        {
            get
            {
                return new string[,] { 
                    {"AL", "Alabama"},
                    {"AK", "Alaska"},
                    {"AZ", "Arizona"},
                    {"AR", "Arkansas"},
                    {"CA", "California"},
                    {"CO", "Colorado"},
                    {"CT", "Connecticut"},
                    {"DE", "Delaware"},
                    {"DC", "District Of Columbia"},
                    {"FL", "Florida"},
                    {"GA", "Georgia"},
                    {"HI", "Hawaii"},
                    {"ID", "Idaho"},
                    {"IL", "Illinois"},
                    {"IN", "Indiana"},
                    {"IA", "Iowa"},
                    {"KS", "Kansas"},
                    {"KY", "Kentucky"},
                    {"LA", "Louisiana"},
                    {"ME", "Maine"},
                    {"MD", "Maryland"},
                    {"MA", "Massachusetts"},
                    {"MI", "Michigan"},
                    {"MN", "Minnesota"},
                    {"MS", "Mississippi"},
                    {"MO", "Missouri"},
                    {"MT", "Montana"},
                    {"NE", "Nebraska"},
                    {"NV", "Nevada"},
                    {"NH", "New Hampshire"},
                    {"NJ", "New Jersey"},
                    {"NM", "New Mexico"},
                    {"NY", "New York"},
                    {"NC", "North Carolina"},
                    {"ND", "North Dakota"},
                    {"OH", "Ohio"},
                    {"OK", "Oklahoma"},
                    {"OR", "Oregon"},
                    {"PA", "Pennsylvania"},
                    {"RI", "Rhode Island"},
                    {"SC", "South Carolina"},
                    {"SD", "South Dakota"},
                    {"TN", "Tennessee"},
                    {"TX", "Texas"},
                    {"UT", "Utah"},
                    {"VT", "Vermont"},
                    {"VA", "Virginia"},
                    {"WA", "Washington"},
                    {"WV", "West Virginia"},
                    {"WI", "Wisconsin"},
                    {"WY", "Wyoming"},
                    {"PR", "Puerto Rico"},
                    {"VI", "Virgin Islands"},
                    {"MP", "Northern Mariana Islands"},
                    {"GU", "Guam"},
                    {"AS", "American Samoa"},
                    {"PW", "Palau"}
                };
            }
        }

        private static SortedList<int, string> _GtuStatus = null;
        public static SortedList<int, string> GtuStatus
        {
            get
            {
                if (_GtuStatus == null)
                {
                    _GtuStatus = new SortedList<int, string>();
                    _GtuStatus.Add(0, "Normal");
                    _GtuStatus.Add(10, "No GTU signal");
                    _GtuStatus.Add(20, "Inside DND (CustomArea)");
                    _GtuStatus.Add(21, "Inside DND (nAddress)");
                    _GtuStatus.Add(30, "Outside map boundary");
                    _GtuStatus.Add(40, "No movement");
                    _GtuStatus.Add(50, "Duplicate path");
                }
                return _GtuStatus;
            }
        }
    }   // end of class
}