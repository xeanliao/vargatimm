using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer.ValueObjects;

namespace GPS.DomainLayer.Entities
{
    public abstract class DjParticipantAssignment
    {
        #region constructors
        protected DjParticipantAssignment() { }

        public DjParticipantAssignment(Int32 newId, UserRoles djRole, DistributionJob dj)
        {
            _id = newId;
            _djRole = djRole;
            _distributionJob = dj;
        }
        #endregion

        #region fields
        private Int32 _id;
        private UserRoles _djRole;
        private Gtu _gtu;
        private User _user;
        private String _fullName = string.Empty;
        private DistributionJob _distributionJob;
        #endregion

        #region public properties
        public virtual UserRoles DjRole
        {
            get { return _djRole; }
        }

        public virtual DistributionJob DistributionJob
        {
            get { return _distributionJob; }
            protected set { _distributionJob = value; }
        }

        public virtual String FullName
        {
            get { return _fullName; }
            protected set { _fullName = value; }
        }

        public virtual Gtu Gtu
        {
            get { return _gtu; }
            set { _gtu = value; }
        }

        public virtual User LoginUser
        {
            get { return _user; }
            set 
            {
                _user = value;

                if (null != _user)
                {
                    _fullName = _user.FullName;
                }
                else 
                {
                    _fullName = string.Empty;
                }
            }
        }
        #endregion

        public virtual void ClonePrototype(DjParticipantAssignment p)
        {
            if (null != p && !Object.ReferenceEquals(p, this))
            {
                _djRole = p._djRole;
                _gtu = p._gtu;
                _user = p._user;
                _fullName = p._fullName;
                _distributionJob = p._distributionJob;
            }
        }

        public virtual void Isolate()
        {
            DistributionJob = null;
            Gtu = null;
            LoginUser = null;
        }
    }
}
