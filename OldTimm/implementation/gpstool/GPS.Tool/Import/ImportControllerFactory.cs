using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Import
{
    public class ImportControllerFactory
    {
        public static ImportAreaController CreateController(string pClassification)
        {
            Classification classification = (Classification)Enum.Parse(
                    typeof(Classification), pClassification);
            ImportAreaController importContr = null;
            switch (classification)
            {
                case Classification.CBSA:
                    //importContr = new CBSAController();
                    importContr = new CBSAImporter();
                    break;
                case Classification.Urban:
                    importContr = new UrbanAreaController();
                    break;
                case Classification.County:
                    importContr = new CountyController();
                    break;
                case Classification.CountyArea:
                    importContr = new CountyAreaController();
                    break;
                case Classification.SLD_Senate:
                    importContr = new SLDSenateAreaController();
                    break;
                case Classification.SLD_House:
                    importContr = new SLDHouseAreaController();
                    break;
                case Classification.SD_Elem:
                    importContr = new ElementarySchoolAreaController();
                    break;
                case Classification.SD_Secondary:
                    importContr = new SecondarySchoolAreaController();
                    break;
                case Classification.SD_Unified:
                    importContr = new SLDUnifiedSchoolController();
                    break;
                case Classification.Voting_District:
                    break;
                case Classification.Z3:
                    importContr = new ThreeZipAreaController();
                    break;
                case Classification.Z5:
                    importContr = new FiveZipImporter();
                    break;
                case Classification.TRK:
                    importContr = new TrkAreaController();
                    break;
                case Classification.BG:
                    importContr = new BgAreaController();
                    break;
                case Classification.ZIP:
                    importContr = new ZIPController();
                    break;
                case Classification.CRoute:
                    importContr = new CRouteController();
                    break;
                case Classification.ZIP4:
                    importContr = new ZIP4Controller();
                    break;
            }
            return importContr;
        }
    }
}