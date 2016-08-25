using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Workday.Business;

namespace Workday.Web
{
    public partial class TimeSheet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //ButtonControl();
                //just for debug, reader from db and show image.
                //Common.TimeSheet newsheet = new Common.TimeSheet();
                //newsheet.TimeSheetId = new Guid("3EA9B686-B3DA-4C4A-B758-1EE28FCFC544");
                //Byte[] imgcontent = TimeSheetBusiness.GetStartImageByID(newsheet);
                //string base64string = Convert.ToBase64String(imgcontent, 0, imgcontent.Length);
                //string imageurl = @"data:image/png;base64," + base64string;
                //capresult.ImageUrl = imageurl;

                //just for debug. reponse front end javascript upload method ajax request.
                //if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                //{
                //    string image = Request.Form["image"];
                //    string filename = DateTime.Now.ToString();
                //    if (image != null & image != "")
                //    {
                //        var parts = image.Split(new char[] { ',' }, 2);
                //        var bytes = Convert.FromBase64String(parts[1]);
                //        var path = HttpContext.Current.Server.MapPath(string.Format("~/{0}.png", filename));
                //        System.IO.File.WriteAllBytes(path, bytes);
                //        //Byte[] imgcontent = System.IO.File.ReadAllBytes(path);
                //        //string base64string = Convert.ToBase64String(imgcontent, 0, imgcontent.Length);
                //        //string imageurl = @"data:image/png;base64," + base64string;
                //        //Response.Write(imageurl);
                //        //capresult.ImageUrl = imageurl;

                //    }
                //}
            }

        [WebMethod(EnableSession = true)]
        public static string Upload(string base64,int type)
        {
            var parts = base64.Split(new char[] { ',' }, 2);
            var bytes = Convert.FromBase64String(parts[1]);
            DateTime now = DateTime.Now;
            string Today = now.ToString("MM/dd/yyyy");
            String Time = now.ToString("HH:mm:ss");
            string IP = HttpContext.Current.Request.UserHostAddress;
            bool ifaddtime = false;

            if (HttpContext.Current.Session["UserID"] != null)
            {
                int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
                Common.TimeSheet newsheet = new Common.TimeSheet();
                newsheet.UserId = userid;
                newsheet.Date = Today;
                if (type == 0)
                {
                    newsheet.StartTime = Time;
                    newsheet.StartIp = IP;
                    newsheet.StartImage = bytes;
                    newsheet.TimeSheetId = Guid.NewGuid();
                }
                else if (type == 1)
                {
                    newsheet.EndTime = Time;
                    newsheet.EndIp = IP;
                    newsheet.EndImage = bytes;
                }

                ifaddtime = TimeSheetBusiness.AddTimeSheet(newsheet,type);
                if (ifaddtime)
                {
                    Byte[] imgcontent = TimeSheetBusiness.GetStartImageByID(newsheet,type);
                    string base64string = Convert.ToBase64String(imgcontent, 0, imgcontent.Length);
                    string imageurl = @"data:image/png;base64," + base64string;
                    return imageurl;
                }
            }
            return "Fail";

            //just for debug, write image to file
            //var path = HttpContext.Current.Server.MapPath(string.Format("~/{0}.png", now.ToString("MM-dd-yyyy-HH-mm-ss")));
            //System.IO.File.WriteAllBytes(path, bytes);
            //return "success!";

        }

        //just for debug
        [WebMethod]
        public static string hello(string a)
        {
            return "you are: "+a;
        }

        [WebMethod(EnableSession = true)]
        public static int ButtonControl(int j)
        {
            int i=4;
            if (HttpContext.Current.Session["UserID"] != null)
            {
                int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
                DateTime now = DateTime.Now;
                string Today = now.ToString("MM/dd/yyyy");
                Common.TimeSheet thistime = new Common.TimeSheet();
                thistime.Date = Today;
                thistime.UserId = userid;
                Common.TimeSheet verify = TimeSheetBusiness.VerifyTime(thistime);
                if (verify.StartTime != null & verify.EndTime != null)
                { //have clockin and clockout
                    i = 2;
                    //ClockIn.Enabled = false;
                    //ClockOut.Enabled = false;
                    //NotifyMsg.Text = "You have clockin at " + verify.StartTime + " and clock out at " + verify.EndTime;
                }
                else if (verify.StartTime != null & verify.EndTime == null)
                { //have clockin but not clockout
                    i = 1;
                    //ClockIn.Enabled = false;
                    //ClockOut.Enabled = true;
                    //NotifyMsg.Text = "You have clockin at " + verify.StartTime;
                }
                else if (verify.StartTime == null & verify.EndTime == null)
                { //not clockin and clockout
                    i = 0;
                    //ClockIn.Enabled = true;
                    //ClockOut.Enabled = false;
                    //NotifyMsg.Text = "You do not clockin today. Please clock in now";
                }
            }
            return i;
        }

        [WebMethod(EnableSession = true)]
        public static string GetNotify(int j)
        {
            string time="";
            if (HttpContext.Current.Session["UserID"] != null)
            {
                int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
                DateTime now = DateTime.Now;
                string Today = now.ToString("MM/dd/yyyy");
                Common.TimeSheet thistime = new Common.TimeSheet();
                thistime.Date = Today;
                thistime.UserId = userid;
                Common.TimeSheet current = TimeSheetBusiness.VerifyTime(thistime);
                if (current.StartTime != null & current.EndTime != null)
                { //have clockin and clockout
                    time = "You have clockin at " + current.StartTime + " and clock out at " + current.EndTime;
                }
                else if (current.StartTime != null & current.EndTime == null)
                { //have clockin but not clockout
                    time = "You have clockin at " + current.StartTime;
                }
                else if (current.StartTime == null & current.EndTime == null)
                { //not clockin and clockout
                    time = "You do not clockin today. Please clock in now";
                }
            }
            return time;
        }
    }
}