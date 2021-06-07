using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTUService.TIMM;

namespace TIMM.GTUService
{
    public interface IGTUInfoDAO
    {
         void InsertGTUInfo(GTU gtuInfo, int taskInfoId);

         void InsertGTU(string code);

         bool IsExistGTU(string code);

         int AvailableMappingId(GTU gtuInfo);

         bool UpdateGTU(GTU gtuInfo);

         bool UpdateGTUList(string code);

         Dictionary<int, TaskInfoDctValue> AvailableTaskId();

         List<string> getGTULstByTaskId(int tid);

         string getMailAddress(int tid);

         string getDMName(int tid);

         Dictionary<int, List<Coordinate>> getDmCollectionByTaskIds(List<int> tidList);

         List<Coordinate> getDmCollectionByTaskId(int tid);

         Dictionary<int, string> getNDAreaIdsByTaskId(int tid);


         Dictionary<int, string> getNDAreaIdsByBoxes(List<int> boxids);

         List<int> getBoxIds(double MaxLatitude, double MinLatitude, double MaxLongitude, double MinLongitude, int mountLat, int mountLon);

         string getUserForEmail(string uid);
    }
}
