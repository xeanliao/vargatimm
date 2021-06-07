using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IdentityModel.Selectors;
using TIMM.GTUService.Service;


namespace GTUService.TIMM
{
    /*
    public class CustomUserNameValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (null == userName || null == password)
            {
                throw new ArgumentNullException();
            }

            if (!(userName == password))
            {
                throw new FaultException("Unknown Username or Password");
            }

        }
    }
    */

    public class GTUUpdateService : IGTUUpdateService
    {
        GTUStore _oGTUStore;
        FrozenStore _oFrozenStore;
        TaskStore _TaskStore;
        
        public GTUUpdateService()
        {
            _oGTUStore = GTUStore.Instance;
            _oFrozenStore = FrozenStore.Instance;
            _TaskStore = TaskStore.Instance;
        }
        public void UpdateGTU(String sGTUCode, GTU oGTU)
        {
            System.Diagnostics.Trace.TraceInformation("Start UpdateGTU! GTUCode:" + sGTUCode + ",GTULatitude:" + oGTU.CurrentCoordinate.Latitude + "\n");


            try
            {
                oGTU.Code = sGTUCode;
                int[] dndAreaIds = _TaskStore.getAroundDNDAreaIds(oGTU.Code);
                              

                if (_TaskStore.IsInDNDArea(oGTU, dndAreaIds))
                {
                    oGTU.Status = Status.InDNDArea;
                    EmailSender.Instance.AddQ(oGTU);
                }

                

                string rStr = _TaskStore.getRegisterStr(oGTU.Code);
                if (rStr != string.Empty)
                {
                    bool isIn = _TaskStore.IsInArea(oGTU.CurrentCoordinate, rStr);
                    //bool isIn = false;
                    System.Diagnostics.Trace.TraceInformation("isIn:" + isIn + "\n");
                    if (!isIn)
                        oGTU.Status = Status.OutBoundary;
                    else
                    {
                        if (oGTU.Status == Status.InDNDArea)
                            oGTU.Status = Status.UnKnown;
                        else
                            oGTU.Status = Status.Normal;
                    }
                    if (_oFrozenStore.UpdateFrozen(oGTU))
                    {
                        oGTU.dndInfo = string.Empty;
                        EmailSender.Instance.AddQ(oGTU);
                    }
                }
               

                _oGTUStore.UpdateGTU(sGTUCode, oGTU);

                

                GTUDBUpdater.Instance.AddQ(oGTU);

                
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceInformation("UpdateGTU Error:" + ex.Message + "\n");
            }
            System.Diagnostics.Trace.TraceInformation("End UpdateGTU" + "\n");
        }
    }
}
