using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Test.GTUUpdate;
using Test.GTUQuery;
using Test.Geofencing;
using Geo = global::Test.Geofencing;
using System.Net.Mail;
using System.Net;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        System.ComponentModel.AsyncOperationManager.SynchronizationContext = new System.Threading.SynchronizationContext();

        
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        using (GTUUpdateServiceClient service = new GTUUpdateServiceClient())
        {
            Test.GTUUpdate.GTU gtu = new Test.GTUUpdate.GTU();
            
            gtu.Speed = 10;
            gtu.Heading = 20;            
            gtu.IPAddress = this.Request.UserHostAddress; 
            gtu.AreaCode = 30;
            gtu.NetworkCode = 4;            
            gtu.CellID = 50;
            gtu.GPSFix = 7;
            gtu.Accuracy = 9;
            gtu.Count = 5;
            gtu.LocationID = 6;
            gtu.Version = "1";
                                  
            gtu.CurrentCoordinate = new Test.GTUUpdate.Coordinate();
            gtu.CurrentCoordinate.Altitude = 111;
            gtu.CurrentCoordinate.Latitude = 222;
            gtu.CurrentCoordinate.Longitude = 333;

            gtu.SendTime = DateTime.Now;
            gtu.ReceivedTime = DateTime.Now;
                        
            gtu.PowerInfo = Test.GTUUpdate.PowerInfo.Low;
            
            gtu.Code = DateTime.Now.ToString("yyyyMMddHHmmss"); 
  
 
            service.UpdateGTU(gtu.Code, gtu);

        }
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        using (GTUQueryServiceClient service = new GTUQueryServiceClient())
        { 
            string[] codeList = service.GetGTUCodeList();
            Test.GTUQuery.GTU[] gtus = service.GetGTUs(codeList);

            this.GridView1.DataSource = gtus;
            this.GridView1.DataBind();
            
        }
    }

    public void Button3_Click(object sender, EventArgs e)
    {
        tb1.Text = "";
        using (GTUQueryServiceClient service = new GTUQueryServiceClient())
        {
            //Test.GTUQuery.Coordinate co = new Test.GTUQuery.Coordinate();
            //List<Test.GTUQuery.Coordinate> colst = new List<Test.GTUQuery.Coordinate>();
            //co.Latitude = 2;
            //co.Longitude = 3;
            //colst.Add(co);
            //string str= service.RegisterDM(colst.ToArray());
            //tb1.Text = str;
            //string[] codeList = { "353453", "353453", "65645" };

            //Test.GTUQuery.GTU[] gtus = service.GetGTUs(codeList);
            List<Test.GTUQuery.Coordinate> colst = new List<Test.GTUQuery.Coordinate>();
            string str = service.RegisterDM(colst.ToArray());
            tb1.Text = str;
            //this.GridView1.DataSource = gtus;
            //this.GridView1.DataBind();

        }
    }

    public void Button4_Click(object sender, EventArgs e)
    {
        if (nlbl.Text == "") nlbl.Text = "0";
        string str = tb1.Text;
        tb2.Text = nlbl.Text;
        tb3.Text = "0";
        double lat = Convert.ToDouble(tb2.Text);
        double lon = Convert.ToDouble(tb3.Text);
        using (GTUQueryServiceClient service = new GTUQueryServiceClient())
        {
            lbl1.Text =service.IsInArea(new Test.GTUQuery.Coordinate { Latitude = lat, Longitude = lon }, str).ToString();
        }
        if(nlbl.Text!="")
        nlbl.Text = (int.Parse(nlbl.Text) + 1).ToString();
    }

    public void Button5_Click(object sender, EventArgs e)
    {
        tb1.Text = "";
        using (GeofencingClient gservice = new GeofencingClient())
        {
            //Test.GTUQuery.Coordinate co = new Test.GTUQuery.Coordinate();
            //List<Test.GTUQuery.Coordinate> colst = new List<Test.GTUQuery.Coordinate>();
            //co.Latitude = 2;
            //co.Longitude = 3;
            //colst.Add(co);
            //string str= service.RegisterDM(colst.ToArray());
            //tb1.Text = str;
            //string[] codeList = { "353453", "353453", "65645" };

            //Test.GTUQuery.GTU[] gtus = service.GetGTUs(codeList);
            List<global::Test.Geofencing.Coordinate> area = new List<global::Test.Geofencing.Coordinate>();
            //global::Test.Geofencing.Coordinate coord = new Test.Geofencing.Coordinate();
            area.Add(new Geo.Coordinate { Latitude = 33.9557495117188, Longitude = -118.273628234863 });
            area.Add(new Geo.Coordinate { Latitude = 33.9557495117188, Longitude = -118.273880004883 });
            area.Add(new Geo.Coordinate { Latitude = 33.9560508728027, Longitude = -118.278213500977 });
            area.Add(new Geo.Coordinate { Latitude = 33.9565582275391, Longitude = -118.280281066895 });
            area.Add(new Geo.Coordinate { Latitude = 33.9566192626953, Longitude = -118.280479431152 });
            area.Add(new Geo.Coordinate { Latitude = 33.956729888916, Longitude = -118.280479431152 });
            area.Add(new Geo.Coordinate { Latitude = 33.960090637207, Longitude = -118.280532836914 });
            area.Add(new Geo.Coordinate { Latitude = 33.9602699279785, Longitude = -118.280532836914 });
            area.Add(new Geo.Coordinate { Latitude = 33.9612007141113, Longitude = -118.281112670898 });
            area.Add(new Geo.Coordinate { Latitude = 33.9613494873047, Longitude = -118.280982971191 });
            area.Add(new Geo.Coordinate { Latitude = 33.9610595703125, Longitude = -118.280090332031 });
            area.Add(new Geo.Coordinate { Latitude = 33.9613189697266, Longitude = -118.280052185059 });
            area.Add(new Geo.Coordinate { Latitude = 33.9609489440918, Longitude = -118.279937744141 });
            area.Add(new Geo.Coordinate { Latitude = 33.960880279541, Longitude = -118.279922485352 });
            area.Add(new Geo.Coordinate { Latitude = 33.9607810974121, Longitude = -118.279823303223 });
            area.Add(new Geo.Coordinate { Latitude = 33.9606590270996, Longitude = -118.279823303223 });
            area.Add(new Geo.Coordinate { Latitude = 33.9601402282715, Longitude = -118.279327392578 });
            area.Add(new Geo.Coordinate { Latitude = 33.9601402282715, Longitude = -118.279289245605 });
            area.Add(new Geo.Coordinate { Latitude = 33.9601287841797, Longitude = -118.278282165527 });
            area.Add(new Geo.Coordinate { Latitude = 33.96044921875, Longitude = -118.277923583984 });
            area.Add(new Geo.Coordinate { Latitude = 33.9601402282715, Longitude = -118.273880004883 });
            area.Add(new Geo.Coordinate { Latitude = 33.9601516723633, Longitude = -118.271690368652 });
            area.Add(new Geo.Coordinate { Latitude = 33.9601593017578, Longitude = -118.269500732422 });
            area.Add(new Geo.Coordinate { Latitude = 33.9601593017578, Longitude = -118.26944732666 });
            area.Add(new Geo.Coordinate { Latitude = 33.9603118896484, Longitude = -118.269309997559 });
            area.Add(new Geo.Coordinate { Latitude = 33.961009979248, Longitude = -118.26927947998 });
            area.Add(new Geo.Coordinate { Latitude = 33.9619293212891, Longitude = -118.26927947998 });
            area.Add(new Geo.Coordinate { Latitude = 33.962818145752, Longitude = -118.26927947998 });
            area.Add(new Geo.Coordinate { Latitude = 33.9630508422852, Longitude = -118.26927947998 });
            area.Add(new Geo.Coordinate { Latitude = 33.9631881713867, Longitude = -118.269149780273 });
            area.Add(new Geo.Coordinate { Latitude = 33.9630699157715, Longitude = -118.26732635498 });
            area.Add(new Geo.Coordinate { Latitude = 33.9630699157715, Longitude = -118.267127990723 });
            area.Add(new Geo.Coordinate { Latitude = 33.9635314941406, Longitude = -118.26538848877 });
            area.Add(new Geo.Coordinate { Latitude = 33.9637298583984, Longitude = -118.26538848877 });
            area.Add(new Geo.Coordinate { Latitude = 33.9639282226563, Longitude = -118.26538848877 });
            area.Add(new Geo.Coordinate { Latitude = 33.9642791748047, Longitude = -118.265533447266 });
            area.Add(new Geo.Coordinate { Latitude = 33.9644203186035, Longitude = -118.265403747559 });
            area.Add(new Geo.Coordinate { Latitude = 33.9644317626953, Longitude = -118.26490020752 });
            area.Add(new Geo.Coordinate { Latitude = 33.9642791748047, Longitude = -118.264762878418 });
            area.Add(new Geo.Coordinate { Latitude = 33.9639282226563, Longitude = -118.264892578125 });
            area.Add(new Geo.Coordinate { Latitude = 33.9637298583984, Longitude = -118.264892578125 });
            area.Add(new Geo.Coordinate { Latitude = 33.9635314941406, Longitude = -118.264892578125 });
            area.Add(new Geo.Coordinate { Latitude = 33.9632911682129, Longitude = -118.264686584473 });
            area.Add(new Geo.Coordinate { Latitude = 33.9630508422852, Longitude = -118.264892578125 });
            area.Add(new Geo.Coordinate { Latitude = 33.9628486633301, Longitude = -118.264892578125 });
            area.Add(new Geo.Coordinate { Latitude = 33.9619293212891, Longitude = -118.264892578125 });
            area.Add(new Geo.Coordinate { Latitude = 33.9617309570313, Longitude = -118.264892578125 });
            area.Add(new Geo.Coordinate { Latitude = 33.9610404968262, Longitude = -118.265090942383 });
            area.Add(new Geo.Coordinate { Latitude = 33.9601287841797, Longitude = -118.265083312988 });
            area.Add(new Geo.Coordinate { Latitude = 33.9596900939941, Longitude = -118.264701843262 });
            area.Add(new Geo.Coordinate { Latitude = 33.9592590332031, Longitude = -118.265090942383 });
            area.Add(new Geo.Coordinate { Latitude = 33.9583396911621, Longitude = -118.265083312988 });
            area.Add(new Geo.Coordinate { Latitude = 33.956470489502, Longitude = -118.265090942383 });
            area.Add(new Geo.Coordinate { Latitude = 33.9560317993164, Longitude = -118.264686584473 });
            area.Add(new Geo.Coordinate { Latitude = 33.9557914733887, Longitude = -118.264892578125 });
            area.Add(new Geo.Coordinate { Latitude = 33.9555892944336, Longitude = -118.264892578125 });
            area.Add(new Geo.Coordinate { Latitude = 33.9546813964844, Longitude = -118.26490020752 });
            area.Add(new Geo.Coordinate { Latitude = 33.9538116455078, Longitude = -118.26490020752 });
            area.Add(new Geo.Coordinate { Latitude = 33.9536094665527, Longitude = -118.26490020752 });
            area.Add(new Geo.Coordinate { Latitude = 33.9532585144043, Longitude = -118.264770507813 });
            area.Add(new Geo.Coordinate { Latitude = 33.9531211853027, Longitude = -118.26490020752 });
            area.Add(new Geo.Coordinate { Latitude = 33.9531211853027, Longitude = -118.265159606934 });
            area.Add(new Geo.Coordinate { Latitude = 33.9531211853027, Longitude = -118.265357971191 });
            area.Add(new Geo.Coordinate { Latitude = 33.9535713195801, Longitude = -118.267127990723 });
            area.Add(new Geo.Coordinate { Latitude = 33.9535713195801, Longitude = -118.26732635498 });
            area.Add(new Geo.Coordinate { Latitude = 33.9535713195801, Longitude = -118.267532348633 });
            area.Add(new Geo.Coordinate { Latitude = 33.9533309936523, Longitude = -118.268882751465 });
            area.Add(new Geo.Coordinate { Latitude = 33.9533309936523, Longitude = -118.269081115723 });
            area.Add(new Geo.Coordinate { Latitude = 33.9530792236328, Longitude = -118.26927947998 });
            area.Add(new Geo.Coordinate { Latitude = 33.952880859375, Longitude = -118.26927947998 });
            area.Add(new Geo.Coordinate { Latitude = 33.9521484375, Longitude = -118.269309997559 });
            area.Add(new Geo.Coordinate { Latitude = 33.9519500732422, Longitude = -118.269287109375 });
            area.Add(new Geo.Coordinate { Latitude = 33.9517593383789, Longitude = -118.269271850586 });
            area.Add(new Geo.Coordinate { Latitude = 33.9513206481934, Longitude = -118.267929077148 });
            area.Add(new Geo.Coordinate { Latitude = 33.9503898620606, Longitude = -118.26927947998 });
            area.Add(new Geo.Coordinate { Latitude = 33.9501914978027, Longitude = -118.26927947998 });
            area.Add(new Geo.Coordinate { Latitude = 33.9499893188477, Longitude = -118.26927947998 });
            area.Add(new Geo.Coordinate { Latitude = 33.9499397277832, Longitude = -118.269332885742 });
            area.Add(new Geo.Coordinate { Latitude = 33.9499397277832, Longitude = -118.26953125 });
            area.Add(new Geo.Coordinate { Latitude = 33.9499397277832, Longitude = -118.269729614258 });
            area.Add(new Geo.Coordinate { Latitude = 33.9496917724609, Longitude = -118.271026611328 });
            area.Add(new Geo.Coordinate { Latitude = 33.9496917724609, Longitude = -118.27123260498 });
            area.Add(new Geo.Coordinate { Latitude = 33.9498291015625, Longitude = -118.271369934082 });
            area.Add(new Geo.Coordinate { Latitude = 33.9499282836914, Longitude = -118.271507263184 });
            area.Add(new Geo.Coordinate { Latitude = 33.9499282836914, Longitude = -118.271713256836 });
            area.Add(new Geo.Coordinate { Latitude = 33.9499397277832, Longitude = -118.273643493652 });
            area.Add(new Geo.Coordinate { Latitude = 33.9501419067383, Longitude = -118.273643493652 });
            area.Add(new Geo.Coordinate { Latitude = 33.9503593444824, Longitude = -118.273643493652 });
            area.Add(new Geo.Coordinate { Latitude = 33.9505081176758, Longitude = -118.273513793945 });
            area.Add(new Geo.Coordinate { Latitude = 33.950870513916, Longitude = -118.273620605469 });
            area.Add(new Geo.Coordinate { Latitude = 33.9510688781738, Longitude = -118.273651123047 });
            area.Add(new Geo.Coordinate { Latitude = 33.951229095459, Longitude = -118.273666381836 });
            area.Add(new Geo.Coordinate { Latitude = 33.9519004821777, Longitude = -118.273620605469 });
            area.Add(new Geo.Coordinate { Latitude = 33.9520606994629, Longitude = -118.273620605469 });
            area.Add(new Geo.Coordinate { Latitude = 33.9528312683105, Longitude = -118.27384185791 });
            area.Add(new Geo.Coordinate { Latitude = 33.9535598754883, Longitude = -118.273620605469 });
            area.Add(new Geo.Coordinate { Latitude = 33.9537582397461, Longitude = -118.273628234863 });
            area.Add(new Geo.Coordinate { Latitude = 33.9546508789063, Longitude = -118.273628234863 });
            area.Add(new Geo.Coordinate { Latitude = 33.9555206298828, Longitude = -118.273628234863 });
            area.Add(new Geo.Coordinate { Latitude = 33.9557495117188, Longitude = -118.273628234863 });
            string str = gservice.RegisterArea(area.ToArray());
            tb4.Text = str;
            //this.GridView1.DataSource = gtus;
            //this.GridView1.DataBind();

        }
    }

    public void Button6_Click(object sender, EventArgs e)
    {
        if (nlb2.Text == "") nlbl.Text = "0";
        for (int i = 0; i < 20; i++)
        {
            string str = tb4.Text;
            //tb2.Text = nlbl.Text;
            //tb3.Text = "0";
            double lat = i;
            double lon = i;
            using (GeofencingClient gservice = new GeofencingClient())
            {
                lbl2.Text = gservice.IsInTheArea(new Geo.Coordinate { Latitude = lat, Longitude = lon }, str).ToString();
            }
            if (nlb2.Text != "")
                nlb2.Text = (int.Parse(nlb2.Text) + 1).ToString();
            nlb3.Text = nlb3.Text + "&&" + i.ToString();
        }
        
    }

    public void Button7_Click(object sender, EventArgs e)
    {
        Test.GTUUpdate.GTU gtu = new Test.GTUUpdate.GTU();
        string gCode = "0a7af384-2e12-4b25-98c2-c2db9a6327b8";
        gtu.CurrentCoordinate = new Test.GTUUpdate.Coordinate();
        gtu.CurrentCoordinate.Latitude = 34.0109803765587;
        gtu.CurrentCoordinate.Longitude = -118.112430900551;
        gtu.Code = "0a7af384-2e12-4b25-98c2-c2db9a6327b8";
        using (GTUUpdateServiceClient gservice = new GTUUpdateServiceClient()) 
        {
            gservice.UpdateGTU(gCode,gtu);
        }


    }

    public void Button8_Click(object sender, EventArgs e)
    {
        Test.GTUUpdate.GTU gtu = new Test.GTUUpdate.GTU();
        string gCode = "0a7af384-2e12-4b25-98c2-c2db9a6327b8";
        List<string> gtuLst = new List<string>();
        gtuLst.Add(gCode);
        //gtu.CurrentCoordinate = new Test.GTUUpdate.Coordinate();
        //gtu.CurrentCoordinate.Latitude = 34.0109803765587;
        //gtu.CurrentCoordinate.Longitude = -118.112430900551;
        //gtu.Code = "0a7af384-2e12-4b25-98c2-c2db9a6327b8";
        using (GTUQueryServiceClient gservice = new GTUQueryServiceClient())
        {
            gservice.GetGTUs(gtuLst.ToArray());
        }
    }

    public void Button9_Click(object sender, EventArgs e)
    {
        SendMail();
    }
    public void SendMail()
    {
        try
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.From = new System.Net.Mail.MailAddress("changricky@gmail.com", "ricky chang");
            //msg.Attachments.Add(new System.Net.Mail.Attachment("c:\\AUTOEXEC.BAT"));
            msg.To.Add("changricky@gmail.com");
            msg.Subject = "Test Email With Attachment!!!";
            msg.Body = "Hello world2!!!";
            msg.BodyEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            msg.Priority = System.Net.Mail.MailPriority.High;
            System.Net.Mail.SmtpClient cliect = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            cliect.Credentials = new System.Net.NetworkCredential("changrickysmtp@gmail.com", "ces12345");
            cliect.EnableSsl = true; 
            cliect.Send(msg);

        }
        catch (Exception ex)
        {

        }
    }

    private static void SendCompletedCallback(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        if (e.Cancelled)
        {

        }
        if (e.Error != null)
        {

        }
        else
        {

        }
    }

    public void Button10_Click(object sender, EventArgs e)
    {
        //double lat=34.0742749326674;
        //double lon = -118.275590667725;
        //List<int> ids = new List<int>();
        //ids.Add(377469865);
        //ids.Add(51);

        Test.GTUUpdate.GTU gtu = new Test.GTUUpdate.GTU();
        string gCode = "011874000051222";
        gtu.CurrentCoordinate = new Test.GTUUpdate.Coordinate();
        gtu.CurrentCoordinate.Latitude = 33.502146790938;
        gtu.CurrentCoordinate.Longitude = -117.642318;
        gtu.SendTime = DateTime.Now;
        gtu.ReceivedTime = DateTime.Now;
        gtu.Code = "011874000051222";
        using (GTUUpdateServiceClient gservice = new GTUUpdateServiceClient())
        {
            gservice.UpdateGTU(gCode, gtu);
        }

    }
}
