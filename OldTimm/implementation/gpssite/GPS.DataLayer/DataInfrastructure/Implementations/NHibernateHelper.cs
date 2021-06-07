using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;

namespace GPS.DataLayer.DataInfrastructure.Implementations
{
    internal class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        public NHibernateHelper() { }

        private static void BuildSessionFactory()
        {
            if (null == _sessionFactory)
            {
                _sessionFactory = new Configuration().Configure().BuildSessionFactory();
            }
        }

        public ISession GetANewSession()
        {
            return SessionFactory.OpenSession();
        }

        public IStatelessSession GetANewStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }

        /// <summary>
        /// Single Session Factory in the application domain.
        /// </summary>
        public static ISessionFactory SessionFactory
        {
            get
            {
                BuildSessionFactory();
                return _sessionFactory;
            }
        }
    }
}
