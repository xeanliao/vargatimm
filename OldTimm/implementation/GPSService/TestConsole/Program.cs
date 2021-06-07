using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTUService.TIMM;
using System.Net;


namespace GTUService.TIMM
{
    class Program
    {

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Press 'c' to test data, press any other key to exit");
                char key = Console.ReadKey().KeyChar;
                if (key != 'C' && key != 'c')
                {
                    Environment.Exit(0);
                }

                // Test GTUUpdateService 
                using (GTUUpdateServiceClient service = new GTUUpdateServiceClient())
                {
                    // Construct a vitual GTU information
                    GTU gtu = new GTU();
                    gtu.Speed = 10;
                    gtu.Heading = 20;
                    IPHostEntry ipEntry = Dns.GetHostByName(Dns.GetHostName());
                    gtu.IPAddress = (ipEntry.AddressList.Length == 0) ? "" : ipEntry.AddressList[0].ToString();
                    gtu.AreaCode = 30;
                    gtu.NetworkCode = 4;
                    gtu.CellID = 50;
                    gtu.GPSFix = 7;
                    gtu.Accuracy = 9;
                    gtu.Count = 5;
                    gtu.LocationID = 6;
                    gtu.Version = "1";
                    gtu.CurrentCoordinate = new Coordinate();
                    gtu.CurrentCoordinate.Altitude = 111;
                    gtu.CurrentCoordinate.Latitude = 222;
                    gtu.CurrentCoordinate.Longitude = 333;
                    gtu.SendTime = DateTime.Now;
                    gtu.ReceivedTime = DateTime.Now;
                    gtu.PowerInfo = PowerInfo.ON;
                    gtu.Code = DateTime.Now.ToString("yyyyMMddHHmmss");  // Save the GTU device code here

                    // Insert GTU information into database
                    service.UpdateGTU(gtu.Code, gtu);

                }
                

                // Test GTUQueryService
                using (GTUQueryServiceClient service = new GTUQueryServiceClient())
                {
                    // Get all keys from hashtable
                    string[] codeList = service.GetGTUCodeList();

                    GTU[] gtus = service.GetGTUs(codeList);

                    Console.WriteLine("\r\nThere are " + gtus.Length + " rows in Hashtable: ");

                    // Print hashtable
                    for (int i = 0; i < gtus.Length; i++)
                    {
                        Console.WriteLine("Code:" + gtus[0].Code + "  SendTime:" + gtus[0].SendTime.ToString());
                    }

                    Console.WriteLine("\r");
                }

            } 
        }


    }
}
