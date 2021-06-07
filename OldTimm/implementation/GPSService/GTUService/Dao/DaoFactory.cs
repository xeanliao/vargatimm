using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;

namespace TIMM.GTUService
{
    public static class DaoFactory
    {
        private static object _LockObj = new object();
        private static Dictionary<Type, Object> _InstanceDictionary = new Dictionary<Type, object>();

        public static T CreateInstance<T>()
        {
            Type instaceType = typeof(T);
            if (instaceType.IsInterface)
            {
                if (!_InstanceDictionary.ContainsKey(instaceType))
                {
                    lock (_LockObj)
                    {
                        if (!_InstanceDictionary.ContainsKey(instaceType))
                        {
                            string spaceName = instaceType.Namespace;
                            string provider = ConfigurationManager.AppSettings["DBProvider"];
                            string className = instaceType.Name.Substring(1);
                            Assembly target = Assembly.Load(instaceType.Assembly.FullName);
                            object instance = target.CreateInstance(string.Format("{0}.{1}.{2}", spaceName, provider, className));
                            if (instance != null)
                            {
                                _InstanceDictionary.Add(instaceType, instance);
                            }
                            else
                            {
                                throw new Exception("create Dao instace failed.");
                            }
                        }
                    }
                }
                return (T)_InstanceDictionary[instaceType];
            }
            else
            {
                throw new Exception("T must be interface");
            }
        }
    }
}
