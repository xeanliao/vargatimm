using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer;
using GPS.DataLayer.ValueObjects;

namespace GPS.DomainLayer.Entities
{
    public class DistributionJob
    {
        #region constructors
        protected DistributionJob()
        {
            this._distributionJobMaps = new List<DistributionJobMap>();
        }

        public DistributionJob(Int32 newId, String name, Campaign campaign)
        {
            this._distributionJobMaps = new List<DistributionJobMap>();

            this._id = newId;
            this._name = name;
            this._campaign = campaign;
        }
        #endregion

        #region basic fields
        private int _id;
        private IList<AuditorAssignment> _auditors;
        private IList<DriverAssignment> _drivers;
        private IList<WalkerAssignment> _walkers;
        private String _name;
        private Campaign _campaign;
        #endregion

        #region children
        private IList<DistributionJobMap> _distributionJobMaps;
        #endregion

        #region public properties
        public virtual int Id
        {
            get { return this._id; }
        }

        public virtual Campaign Campaign
        {
            get { return this._campaign; }
        }

        public virtual AuditorAssignment AuditorAssignment
        {
            get { return null != _auditors && _auditors.Count > 0 ? _auditors.ElementAt(0) : null; }
        }

        public virtual IEnumerable<DriverAssignment> DriverAssignments
        {
            get { return _drivers; }
        }

        public virtual String Name
        {
            get { return _name; }
            set 
            {
                RequireJobNameIsNotEmpty(value);
                _name = value; 
            }
        }

        public virtual IEnumerable<DistributionMap> DistributionMaps
        {
            get 
            {
                return this._distributionJobMaps.Select<DistributionJobMap, DistributionMap>(t => t.DistributionMap);
            }
        }

        public virtual IEnumerable<WalkerAssignment> WalkerAssignments
        {
            get { return this._walkers; }
        }
        #endregion

        #region public instance methods
        /// <summary>
        /// Assign a new <see cref="Auditor"/> to this <see cref="DistributionJob"/>.
        /// If there is a preexisting <see cref="Auditor"/>, replace that one with
        /// the new <see cref="Auditor"/> specified.
        /// </summary>
        /// <param name="auditor"></param>
        public virtual void AssignAuditor(AuditorAssignment auditor)
        {
            if (auditor.DistributionJob != this)
            {
                throw new InvalidOperationException("The Auditor is not created for the current Distribition Job.");
            }

            if (null == _auditors)
            { 
                _auditors = new List<AuditorAssignment>(); 
            }
            else
            { 
                _auditors.Clear(); 
            }

            _auditors.Add(auditor);
        }

        public virtual void AssignOneMoreDriver(DriverAssignment driver)
        {
            CheckDriverFullNameIsUnique(driver.FullName);

            if (null == this._drivers)
            {
                _drivers = new List<DriverAssignment>();
            }

            this._drivers.Add(driver);
        }

        public virtual void AssignOneMoreWalker(WalkerAssignment walker)
        {
            CheckWalkerFullNameIsUnique(walker.FullName);

            if (null == _walkers)
            {
                _walkers = new List<WalkerAssignment>();
            }
            
            _walkers.Add(walker);
        }

        public virtual void AssignGtuToAuditor(Gtu gtu)
        {
            if (null == this.AuditorAssignment)
            {
                throw new InvalidOperationException("Trying to assign a Gtu to an unassigned Auditor.");
            }
            this.AuditorAssignment.Gtu = gtu;
        }

        public virtual void AssignGtuToDriver(DriverAssignment driver, Gtu g)
        {
            if (null == this.DriverAssignments || !this.DriverAssignments.Contains(driver))
            {
                throw new InvalidOperationException("Trying to assign a Gtu to an unassigned Driver.");
            }
            driver.Gtu = g;
        }

        public virtual void AssignGtuToWalker(WalkerAssignment walker, Gtu g)
        {
            if (null == this.WalkerAssignments || !this.WalkerAssignments.Contains(walker))
            {
                throw new InvalidOperationException("Trying to assign a Gtu to an unassigned Walker.");
            }
            walker.Gtu = g;
        }

        public virtual void RemoveAuditor()
        {
            if (null != _auditors)
            {
                _auditors.Clear();
            }
        }

        /// <summary>
        /// Replace the existing <see cref="Driver"/>s in the <see cref="DistributionJob"/> with
        /// the new collection of <see cref="Driver"/>s. This will remove all those existing
        /// <see cref="Drivers"/> not in the specified <see cref="Driver"/>.
        /// </summary>
        /// <param name="drivers">The new collection of <see cref="Driver"/>s.</param>
        public virtual void ReplaceDriversWith(IEnumerable<DriverAssignment> drivers)
        {
            // If there is no preexisting driver, just add the new drivers
            if (null == _drivers || _drivers.Count == 0)
            {
                AppendDrivers(drivers);
            }
            else
            {
                RemoveExistingDriversNotIn(drivers);
                ReplaceExistingDriversThatAreIn(drivers);
                AddNewDriversToExistingDriverCollection(drivers);
            }
        }

        public virtual void ReplaceWalkersWith(IEnumerable<WalkerAssignment> walkers)
        {
            // If there is no preexisting walker, just add the new walkers
            if (null == _walkers || _walkers.Count == 0)
            {
                AppendWalkers(walkers);
            }
            else 
            {
                RemoveExistingWalkersNotIn(walkers);
                ReplaceExistingDriversThatAreIn(walkers);
                AddNewWalkersToExistingWalkerCollection(walkers);
            }
        }
        #endregion

        #region Implementations
        private void RequireJobNameIsNotEmpty(String name)
        {
            if (String.IsNullOrEmpty(name))
                throw new InvalidOperationException("The Distribution Job Name should not be empty.");
        }

        private void AppendDrivers(IEnumerable<DriverAssignment> newDrivers)
        {
            foreach (DriverAssignment newDriver in newDrivers)
            {
                AssignOneMoreDriver(newDriver);
            }
        }

        private void RemoveExistingDriversNotIn(IEnumerable<DriverAssignment> drivers)
        {
            IList<DriverAssignment> toBeRemoved = new List<DriverAssignment>();

            foreach (DriverAssignment existing in _drivers)
            {
                if (null == drivers.FirstOrDefault(t => t.FullName.ToLower() == existing.FullName.ToLower()))
                {
                    toBeRemoved.Add(existing);
                }
            }

            foreach (DriverAssignment d in toBeRemoved)
            {
                _drivers.Remove(d);
            }
        }

        private void ReplaceExistingDriversThatAreIn(IEnumerable<DriverAssignment> drivers)
        {
            if (null != _drivers)
            {
                foreach (DriverAssignment d in drivers)
                {
                    DriverAssignment existingDriver = _drivers.FirstOrDefault(t => t.FullName.ToLower() == d.FullName.ToLower());
                    if (null != existingDriver)
                    {
                        d.ClonePrototype(existingDriver);
                        RemoveDriver(existingDriver);
                        AssignOneMoreDriver(d);
                    }
                }
            }
        }

        private void RemoveDriver(DriverAssignment existingDriver)
        {
            if (_drivers.Contains(existingDriver))
            {
                existingDriver.Isolate();
                _drivers.Remove(existingDriver);
            }
        }

        /// <summary>
        /// If any <see cref="Driver"/> in the <paramref name="drivers"/> collection 
        /// does not exist in the existing <see cref="Drivers"/> collection, add the
        /// missing one the existing collection.
        /// </summary>
        /// <param name="drivers">The new collection of <see cref="Driver"/>s.</param>
        private void AddNewDriversToExistingDriverCollection(IEnumerable<DriverAssignment> drivers)
        {
            if (null != _drivers)
            {
                foreach (DriverAssignment d in drivers)
                {
                    DriverAssignment existingDriver = _drivers.FirstOrDefault(t => t.FullName.ToLower() == d.FullName.ToLower());
                    if (null == existingDriver)
                    {
                        AssignOneMoreDriver(d);
                    }
                }
            }
        }

        private void AppendWalkers(IEnumerable<WalkerAssignment> newWalkers)
        {
            foreach (WalkerAssignment walker in newWalkers)
            {
                AssignOneMoreWalker(walker);
            }
        }

        private void RemoveExistingWalkersNotIn(IEnumerable<WalkerAssignment> walkers)
        {
            IList<WalkerAssignment> toBeRemoved = new List<WalkerAssignment>();

            foreach (WalkerAssignment existing in _walkers)
            {
                if (null == walkers.FirstOrDefault(t => t.FullName.ToLower() == existing.FullName.ToLower()))
                {
                    toBeRemoved.Add(existing);
                }
            }

            foreach (WalkerAssignment d in toBeRemoved)
            {
                _walkers.Remove(d);
            }
        }

        private void ReplaceExistingDriversThatAreIn(IEnumerable<WalkerAssignment> walkers)
        {
            if (null != _walkers)
            {
                foreach (WalkerAssignment d in walkers)
                {
                    WalkerAssignment existingWalker = _walkers.FirstOrDefault(t => t.FullName.ToLower() == d.FullName.ToLower());
                    if (null != existingWalker)
                    {
                        d.ClonePrototype(existingWalker);
                        RemoveWalker(existingWalker);
                        AssignOneMoreWalker(d);
                    }
                }
            }
        }

        private void RemoveWalker(WalkerAssignment existingWalker)
        {
            if (_walkers.Contains(existingWalker))
            {
                existingWalker.Isolate();
                _walkers.Remove(existingWalker);
            }
        }

        private void AddNewWalkersToExistingWalkerCollection(IEnumerable<WalkerAssignment> drivers)
        {
            if (null != _walkers)
            {
                foreach (WalkerAssignment d in drivers)
                {
                    WalkerAssignment existingDriver = _walkers.FirstOrDefault(t => t.FullName.ToLower() == d.FullName.ToLower());
                    if (null == existingDriver)
                    {
                        AssignOneMoreWalker(d);
                    }
                }
            }
        }

        #endregion

        #region Business rules
        private void CheckDriverFullNameIsUnique(String fullName)
        {
            if (null != _drivers)
            {
                if (null != _drivers.FirstOrDefault(t => t.FullName.ToLower() == fullName.ToLower()))
                {
                    throw new InvalidOperationException(String.Format("A Driver with full name {0} already exists.", fullName));
                }
            }
        }

        private void CheckWalkerFullNameIsUnique(String fullName)
        {
            if (null != _walkers)
            {
                if (null != _walkers.FirstOrDefault(t => t.FullName.ToLower() == fullName.ToLower()))
                {
                    throw new InvalidOperationException(String.Format("A Driver with full name {0} already exists.", fullName));
                }
            }
        }
        #endregion
    }
}
