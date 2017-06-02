using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Workday.Business;
using Workday.Common;

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
            if (!IsPostBack)
            {
                Calendar1.SelectionMode = CalendarSelectionMode.Day;
                Calendar1.FirstDayOfWeek = (FirstDayOfWeek)1;
                DateTime today = DateTime.Today;
                Calendar1.SelectedDate = today;
                DateTime Monday = today.AddDays(-(int)today.DayOfWeek + 1);
                //DateTime Monday = today.AddDays(-10);
                DateTime Sunday = today.AddDays(-(int)today.DayOfWeek + 7);
                if (HttpContext.Current.Session["UserID"] != null)
                {
                    int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
                    List<TsForView> TimeSheets = new List<TsForView>();
                    List<TsForView> MTimeSheets = new List<TsForView>();
                    BindUserDrop();
                    int selectuserid = Int32.Parse(SelectUserDrop.SelectedValue.ToString());
                    TimeSheets = TimeSheetBusiness.GetTimeByUser(userid, Monday, Sunday, 0);
                    MTimeSheets = TimeSheetBusiness.GetTimeByUser(selectuserid, Monday, Sunday, 1);
                    TimeSheetGridView.DataSource = TimeSheets;
                    TimeSheetGridView.DataBind();
                    ManagerTimeSheetGridView.DataSource = MTimeSheets;
                    ManagerTimeSheetGridView.DataBind();
                }
            }
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
                }
                else if (verify.StartTime != null & verify.EndTime == null)
                { //have clockin but not clockout
                    i = 1;
                }
                else if (verify.StartTime == null & verify.EndTime == null)
                { //not clockin and clockout
                    i = 0;
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

        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            if (e.Day.Date > DateTime.Today)
            {
                e.Day.IsSelectable = false;
            }
            DateTime SelectDay = Calendar1.SelectedDate;
            DateTime Monday = SelectDay.AddDays(-(int)SelectDay.DayOfWeek + 1);
            DateTime Sunday = SelectDay.AddDays(-(int)SelectDay.DayOfWeek + 7);
            if(e.Day.Date<=Sunday & e.Day.Date >= Monday)
            {
                e.Cell.BackColor = System.Drawing.Color.Gray;
            }
        }

        protected void TimeSheetGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gridView = (GridView)sender;
            if (e.Row.RowType == DataControlRowType.DataRow & e.Row.RowState != (DataControlRowState.Edit | DataControlRowState.Alternate))
            {   //when in normal mode( not edit and not sorting)
                TsForView TimeSheet = ((TsForView)e.Row.DataItem);
                Image rrimg = e.Row.FindControl("RRimg") as Image;
                rrimg.ImageUrl = TimeSheet.TsRRImgUrl;
              }
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            DateTime  SelectDay = Calendar1.SelectedDate;
            DateTime Monday = SelectDay.AddDays(-(int)SelectDay.DayOfWeek + 1);
            //DateTime Monday = SelectDay.AddDays(-10);
            DateTime Sunday = SelectDay.AddDays(-(int)SelectDay.DayOfWeek + 7);
            if (HttpContext.Current.Session["UserID"] != null)
            {
                int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
                List<TsForView> TimeSheets = new List<TsForView>();
                List<TsForView> MTimeSheets = new List<TsForView>();
                int selectuserid = Int32.Parse(SelectUserDrop.SelectedValue.ToString());
                TimeSheets = TimeSheetBusiness.GetTimeByUser(userid, Monday, Sunday, 0);
                MTimeSheets = TimeSheetBusiness.GetTimeByUser(selectuserid, Monday, Sunday, 1);
                TimeSheetGridView.DataSource = TimeSheets;
                TimeSheetGridView.DataBind();
                ManagerTimeSheetGridView.DataSource = MTimeSheets;
                ManagerTimeSheetGridView.DataBind();
            }
        }

        protected void ManagerTimeSheetGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gridView = (GridView)sender;
            if (e.Row.RowType == DataControlRowType.DataRow & e.Row.RowState != (DataControlRowState.Edit | DataControlRowState.Alternate))
            {   //when in normal mode( not edit and not sorting)
                TsForView TimeSheet = ((TsForView)e.Row.DataItem);
                Image rrimg = e.Row.FindControl("RRimg0") as Image;
                rrimg.ImageUrl = TimeSheet.TsRRImgUrl;
                if (TimeSheet.StartImage != null)
                {
                    Image Simg = e.Row.FindControl("Simg") as Image;
                    string imageurl= @"data:image/png;base64," + Convert.ToBase64String(TimeSheet.StartImage, 0, TimeSheet.StartImage.Length);
                    Simg.ImageUrl = imageurl;
                    //Simg.Attributes.Add("OnMouseOver", "popupsimg(imageurl);");
                    //Simg.Attributes.Add("OnMouseOver", " this.src = '/img/approve.jpg'");
                }
                if (TimeSheet.EndImage != null)
                {
                    Image Eimg = e.Row.FindControl("Eimg") as Image;
                    string imageurl2 = @"data:image/png;base64," + Convert.ToBase64String(TimeSheet.EndImage, 0, TimeSheet.EndImage.Length);
                    Eimg.ImageUrl = imageurl2;
                    //Eimg.Attributes.Add("OnMouseOver", "popupeimg(imageurl2);");
                }
                if(TimeSheet.ReviewResult!= Common.TsReviewResult.NoProcess)
                {
                    Button ApproveT = e.Row.FindControl("App_Time") as Button;
                    Button RefuseT = e.Row.FindControl("Ref_Time") as Button;
                    ApproveT.Visible = false;
                    RefuseT.Visible = false;
                }
            }
        }

        protected void BindUserDrop()
        {
            if (HttpContext.Current.Session["UserID"] != null)
            {
                int managerid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
                UserList Users = new UserList();
                Users = TimeSheetBusiness.GetUserList(managerid);
                if (Users.UserDict != null)
                {
                    SelectUserDrop.DataSource = Users.UserDict;
                    SelectUserDrop.DataTextField = "Key";
                    SelectUserDrop.DataValueField = "Value";
                    SelectUserDrop.DataBind();
                    SelectUserDrop.SelectedIndex = 0;
                }
                else
                {
                    SelectUserDrop.Items.Insert(0, new ListItem("null", "0"));
                    SelectUserDrop.SelectedIndex = 0;
                }
            }

        }

        protected void SelectUserDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["UserID"] != null)
            {
                DateTime SelectDay = Calendar1.SelectedDate;
                DateTime Monday = SelectDay.AddDays(-(int)SelectDay.DayOfWeek + 1);
                DateTime Sunday = SelectDay.AddDays(-(int)SelectDay.DayOfWeek + 7);
                int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
                List<TsForView> MTimeSheets = new List<TsForView>();
                int selectuserid = Int32.Parse(SelectUserDrop.SelectedValue.ToString());
                if(selectuserid!=-1)
                    MTimeSheets = TimeSheetBusiness.GetTimeByUser(selectuserid, Monday, Sunday, 1);
                else  //select all users's time sheets
                    MTimeSheets = TimeSheetBusiness.GetTimeByUser(userid, Monday, Sunday, 2);
                ManagerTimeSheetGridView.DataSource = MTimeSheets;
                ManagerTimeSheetGridView.DataBind();
            }
        }

        protected void Approve_Refuse_Time(object sender, GridViewCommandEventArgs e)
        {
            //throw new NotImplementedException();
            TsForView TimeSheet1=new TsForView();
            DateTime now = DateTime.Now;
            string Today = now.ToString("MM/dd/yyyy");
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = ManagerTimeSheetGridView.Rows[index];
            TimeSheet1.WorkDuration= row.Cells[8].Text;
            TimeSheet1.TimeSheetId = new Guid(ManagerTimeSheetGridView.DataKeys[index].Value.ToString());
            TimeSheet1.ReviewDate = Today;
            if (Session["UserID"] != null)
                TimeSheet1.ReviewedUserId = Convert.ToInt32(Session["UserID"]);
            if (e.CommandName == "Approve_Time")
            {
                if (TimeSheetBusiness.ApproveTime(TimeSheet1) == true)
                {
                    DateTime SelectDay = Calendar1.SelectedDate;
                    DateTime Monday = SelectDay.AddDays(-(int)SelectDay.DayOfWeek + 1);
                    //DateTime Monday = SelectDay.AddDays(-10);
                    DateTime Sunday = SelectDay.AddDays(-(int)SelectDay.DayOfWeek + 7);
                    if (HttpContext.Current.Session["UserID"] != null)
                    {
                        int userid = Convert.ToInt32(HttpContext.Current.Session["UserID"].ToString());
                        List<TsForView> TimeSheets = new List<TsForView>();
                        List<TsForView> MTimeSheets = new List<TsForView>();
                        int selectuserid = Int32.Parse(SelectUserDrop.SelectedValue.ToString());
                        TimeSheets = TimeSheetBusiness.GetTimeByUser(userid, Monday, Sunday, 0);
                        if(selectuserid!=-1)
                            MTimeSheets = TimeSheetBusiness.GetTimeByUser(selectuserid, Monday, Sunday, 1);
                        else
                            MTimeSheets = TimeSheetBusiness.GetTimeByUser(userid, Monday, Sunday, 2);
                        TimeSheetGridView.DataSource = TimeSheets;
                        TimeSheetGridView.DataBind();
                        ManagerTimeSheetGridView.DataSource = MTimeSheets;
                        ManagerTimeSheetGridView.DataBind();
                    }
                }
            }
        }
    }
}