using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TIMM.GPS.ControlCenter.ViewModel
{
    public class ValidationViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        protected Dictionary<string, List<string>> errorList = new Dictionary<string, List<string>>();

        public virtual bool ValidateProperty(string propertyName, object propertyValue)
        {
            //Setup context and output variable
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(this, null, null);
            validationContext.MemberName = propertyName;

            //Perform validation
            bool isValid =
                Validator.TryValidateProperty(propertyValue, validationContext, validationResults);

            if (!isValid) //Register validation errors
            {
                //Perform a full clear
                RemoveErrors(propertyName);

                //Add active validation errors
                foreach (var validationResult in validationResults)
                {
                    AddError(propertyName, validationResult.ErrorMessage);
                }
            }
            else // Remove all error messages
            {
                RemoveErrors(propertyName);
            }

            return isValid;
        }

        public virtual void AddError(string propertyName, string valdationError)
        {
            CheckErrorCollectionForProperty(propertyName);

            if (errorList[propertyName].Contains(valdationError)) return;
            errorList[propertyName].Add(valdationError);

            RaiseErrorsChanged(propertyName);
            RaisePropertyChanged("HasErrors");
        }

        public virtual void RemoveError(string propertyName, string validationError)
        {
            CheckErrorCollectionForProperty(propertyName);

            if (errorList[propertyName].Contains(validationError))
            {
                errorList[propertyName].Remove(validationError);
            }

            RaiseErrorsChanged(propertyName);
            RaisePropertyChanged("HasErrors");
        }

        public virtual void RemoveErrors(string propertyName)
        {
            CheckErrorCollectionForProperty(propertyName);

            errorList[propertyName].Clear();

            RaiseErrorsChanged(propertyName);
            RaisePropertyChanged("HasErrors");
        }

        #region INotifyDataErrorInfo Members

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            CheckErrorCollectionForProperty(propertyName);

            return errorList[propertyName];
        }

        public bool HasErrors
        {
            get { return errorList.Values.Sum(c => c.Count) > 0; }
        }

        #endregion

        protected void CheckErrorCollectionForProperty(string propertyName)
        {
            if (!errorList.ContainsKey(propertyName))
            {
                errorList[propertyName] = new List<string>();
            }
        }

        protected void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
    }
}
