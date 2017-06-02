using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Workday.Common;
using System.Data.SqlClient;
using System.Data;

namespace Workday.DataAccess
{
    public class TimeSheetDataAccess
    {
        private static string _conn = System.Configuration.ConfigurationManager.ConnectionStrings["Myconnection"].ToString();

        public static bool AddTimeSheet(TimeSheet thistime,int type)
        {  //type=0 means clockin, type=1 means clockout

            if (type == 0 & GetTimeIdByUserIdAndDate(thistime)!=null)
            {// if there has been row of this user and date, you could not insert another row
                return false;
            }
            else if(type==1 & (VerifyTime(thistime).EndTime!=null& VerifyTime(thistime).EndTime != ""))
            { //if there has been endtime of this user and date, you could not update it again.
                return false;
            }
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                string sql="";
                Nullable<Guid> timesheetid=null;
                try
                {
                    if (type == 0)
                    {
                        sql = "insert into [TimeSheet1] (UserId, WorkDate, StartTime, StartIp, StartImage,TimeSheetId) values (@value1, @value2, @value3,@value4, @value5,@value6); ";
                    }
                    else if (type == 1)
                    {
                        timesheetid = GetTimeIdByUserIdAndDate(thistime);
                        sql = "update [TimeSheet1] set EndTime=@value1, EndIp=@value2, EndImage=@value3 where TimeSheetId=@value4;";
                    }
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (type == 0)
                    {
                        cmd.Parameters.AddWithValue("@value1", thistime.UserId);
                        cmd.Parameters.AddWithValue("@value2", thistime.Date);
                        cmd.Parameters.AddWithValue("@value3", thistime.StartTime);
                        cmd.Parameters.AddWithValue("@value4", thistime.StartIp);
                        cmd.Parameters.AddWithValue("@value6", thistime.TimeSheetId);
                        SqlParameter param = cmd.Parameters.Add("@value5", SqlDbType.VarBinary);
                        param.Value = thistime.StartImage;
                    }
                    else if (type == 1)
                    {
                        cmd.Parameters.AddWithValue("@value1", thistime.EndTime);
                        cmd.Parameters.AddWithValue("@value2", thistime.EndIp);
                        cmd.Parameters.AddWithValue("@value4", timesheetid);
                        SqlParameter param = cmd.Parameters.Add("@value3", SqlDbType.VarBinary);
                        param.Value = thistime.EndImage;
                    }
                    var result = cmd.ExecuteNonQuery();

                    if (result != 0 & result != -1)
                    {
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                conn.Close();
            }
            return false;
        }

        public static byte[] GetStartImageByID(TimeSheet thistime,int type)
        {
            byte[] imagecontent;
            string sql = "";
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                try
                {
                    if(type==0)
                        sql = "select StartImage from [TimeSheet1] where TimeSheetId=@value1";
                    else if(type==1)
                        sql = "select EndImage from [TimeSheet1] where UserId=@value1 and WorkDate=@value2";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (type == 0)
                    {
                        cmd.Parameters.AddWithValue("@value1", thistime.TimeSheetId);
                    }
                    else if (type == 1)
                    {
                        cmd.Parameters.AddWithValue("@value1", thistime.UserId);
                        cmd.Parameters.AddWithValue("@value2", thistime.Date);
                    }
                    imagecontent = cmd.ExecuteScalar() as byte[];
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                conn.Close();
            }
            return imagecontent;
        }

        private DataTable GetData(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
                    }
                }
                return dt;
            }
        }

        private static Nullable<Guid> GetTimeIdByUserIdAndDate(TimeSheet thistime)
        {
            Nullable<Guid> timesheetid=null;
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();

                try
                {
                    string sql = "select TimeSheetID from [TimeSheet1] where UserId=@value1 and WorkDate=@value2";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", thistime.UserId);
                    cmd.Parameters.AddWithValue("@value2", thistime.Date);
                    object temp = cmd.ExecuteScalar();
                    if ( temp != null)
                        timesheetid = (Guid)temp;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                conn.Close();
                return timesheetid;
            }
        }

        public static TimeSheet VerifyTime(TimeSheet thistime)
        {
            TimeSheet verifytime=new TimeSheet();
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                try
                {
                    string sql = "select StartTime, EndTime from [TimeSheet1] where UserId=@value1 and WorkDate=@value2";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", thistime.UserId);
                    cmd.Parameters.AddWithValue("@value2", thistime.Date);
                    SqlDataReader result = cmd.ExecuteReader();
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            if (!result.IsDBNull(0) & result.IsDBNull(1))
                            { // have clockin not clockout
                                //TimeSpan start = result.GetTimeSpan(0);
                                //string temp = start.ToString(@"hh\:mm\:ss");
                                verifytime.StartTime = result.GetTimeSpan(0).ToString(@"hh\:mm\:ss");
                            }
                            else if (!result.IsDBNull(0) & !result.IsDBNull(1))
                            { // have clockin and clockout
                                verifytime.StartTime = result.GetTimeSpan(0).ToString(@"hh\:mm\:ss");
                                verifytime.EndTime = result.GetTimeSpan(1).ToString(@"hh\:mm\:ss");
                            }   
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                conn.Close();
                return verifytime;
            }
        }

        public static List<TsForView> GetTimeByUser(int userid, DateTime begindate, DateTime enddate, int type)
        {
            List<TsForView> TimeViews = new List<TsForView>();
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                string sql;
                try
                {
                    if (type == 0)
                    {// get time of one user for himself
                        //sql = "select * from (select t.UserId,t.WorkDate,t.StartTime,t.EndTime,r.IsReviewed,r.IsApproved,r.RefuseReason,r.ApprovedWorkTime,r.ReviewedBy from TimeSheet1 as t  left join TimeReview as r on t.TimeSheetId=r.TimeSheetId) as tv where tv.UserId=@value1 and tv.WorkDate between @value2 and @value3";
                        sql = "select tvv.*,u.UserName from (select * from (select t.UserId,t.WorkDate,t.StartTime,t.EndTime,r.IsReviewed,r.IsApproved,r.RefuseReason,r.ApprovedWorkTime,r.ReviewedBy from TimeSheet1 as t  left join TimeReview as r on t.TimeSheetId=r.TimeSheetId) as tv where tv.UserId=@value1 and tv.WorkDate between @value2 and @value3) as tvv left join User1 as u on tvv.ReviewedBy=u.UserId";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@value1", userid);
                        cmd.Parameters.AddWithValue("@value2", begindate);
                        cmd.Parameters.AddWithValue("@value3", enddate);
                        SqlDataReader result = cmd.ExecuteReader();
                        if (result.HasRows)
                        {
                            while (result.Read())
                            {
                                TsForView TimeView = new TsForView();
                                TimeView.UserId = result.GetInt32(0);
                                TimeView.Date = result.GetDateTime(1).ToString();
                                if (!result.IsDBNull(2))
                                    TimeView.StartTime = result.GetTimeSpan(2).ToString(@"hh\:mm\:ss");
                                if (!result.IsDBNull(3))
                                    TimeView.EndTime = result.GetTimeSpan(3).ToString(@"hh\:mm\:ss");
                                if(!result.IsDBNull(2)& !result.IsDBNull(3))
                                {
                                    TimeSpan duration = result.GetTimeSpan(3) - result.GetTimeSpan(2);
                                    //float hour = duration.Hours + (float)duration.Minutes / 60 + (float)duration.Seconds / 3600;
                                    TimeView.WorkDuration = duration.TotalHours.ToString("0.0");
                                }
                                if (result.IsDBNull(4))
                                    TimeView.ReviewResult = Common.TsReviewResult.NoProcess;
                                else if (result.GetInt32(4) == 1 & result.GetInt32(5) == 1)
                                    TimeView.ReviewResult = Common.TsReviewResult.Approved;
                                else if (result.GetInt32(4) == 1 & result.GetInt32(5) == 0)
                                    TimeView.ReviewResult = Common.TsReviewResult.Refused;
                                if (!result.IsDBNull(6))
                                    TimeView.RefuseReason = result.GetString(6);
                                if (!result.IsDBNull(7))
                                {
                                    //double temp =result.GetDouble(7).ToString(;
                                    //float temp1 = (float)temp;
                                    TimeView.ApprovedDuration = result.GetDouble(7).ToString("00.00");
                                }

                                if (!result.IsDBNull(9))
                                    TimeView.ReviewedBy = result.GetString(9);
                                TimeViews.Add(TimeView);
                            }
                        }
                    }
                    else if (type == 1)
                    {//get time of one user for his managers's review
                        sql = "select tvv.*, uu.UserName from (select tr.* ,u.UserName as ReviewedName from (select t.TimeSheetId,t.UserId,t.WorkDate,t.StartTime,t.StartIp,t.StartImage,t.EndTime,t.EndIp,t.EndImage,r.IsReviewed,r.IsApproved,r.RefuseReason,r.ApprovedWorkTime,r.ReviewedBy from (select * from TimeSheet1 where UserId=@value1 and WorkDate between @value2 and @value3) as t left join TimeReview as r on t.TimeSheetId=r.TimeSheetId) as tr left join User1 as u on tr.ReviewedBy=u.UserId) as tvv left join User1 as uu on tvv.UserId=uu.UserId";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@value1", userid);
                        cmd.Parameters.AddWithValue("@value2", begindate);
                        cmd.Parameters.AddWithValue("@value3", enddate);
                        SqlDataReader result = cmd.ExecuteReader();
                        if (result.HasRows)
                        {
                            while (result.Read())
                            {
                                TsForView TimeView = new TsForView();
                                TimeView.TimeSheetId = result.GetGuid(0);
                                TimeView.UserId = result.GetInt32(1);
                                TimeView.Date = result.GetDateTime(2).ToString();
                                if (!result.IsDBNull(3))
                                    TimeView.StartTime = result.GetTimeSpan(3).ToString(@"hh\:mm\:ss");
                                if (!result.IsDBNull(4))
                                    TimeView.StartIp = result.GetString(4);
                                if (!result.IsDBNull(5))
                                    TimeView.StartImage = (Byte[])result.GetValue(5);
                                if (!result.IsDBNull(6))
                                    TimeView.EndTime = result.GetTimeSpan(6).ToString(@"hh\:mm\:ss");
                                if (!result.IsDBNull(3) & !result.IsDBNull(6))
                                {
                                    TimeSpan duration = result.GetTimeSpan(6) - result.GetTimeSpan(3);
                                    //float hour = duration.Hours + (float)duration.Minutes / 60 + (float)duration.Seconds / 3600;
                                    TimeView.WorkDuration = duration.TotalHours.ToString("0.0");
                                }
                                if (!result.IsDBNull(7))
                                    TimeView.EndIp = result.GetString(7);
                                if (!result.IsDBNull(8))
                                    TimeView.EndImage = (Byte[])result.GetValue(8);
                                if (result.IsDBNull(9))
                                    TimeView.ReviewResult = Common.TsReviewResult.NoProcess;
                                else if (result.GetInt32(9) == 1 & result.GetInt32(10) == 1)
                                    TimeView.ReviewResult = Common.TsReviewResult.Approved;
                                else if (result.GetInt32(9) == 1 & result.GetInt32(10) == 0)
                                    TimeView.ReviewResult = Common.TsReviewResult.Refused;
                                if (!result.IsDBNull(11))
                                    TimeView.RefuseReason = result.GetString(11);
                                if (!result.IsDBNull(12))
                                    TimeView.ApprovedDuration = result.GetDouble(12).ToString("0.0");
                                if (!result.IsDBNull(14))
                                    TimeView.ReviewedBy = result.GetString(14);
                                if (!result.IsDBNull(15))
                                    TimeView.UserName = result.GetString(15);
                                TimeViews.Add(TimeView);
                            }
                        }
                    }
                    else  //type=2
                    {//get time of all users in this dept for manager's review
                        sql = "select tvv.*, uu.UserName from (select tr.* ,u.UserName as ReviewedName from (select t.TimeSheetId,t.UserId,t.WorkDate,t.StartTime,t.StartIp,t.StartImage,t.EndTime,t.EndIp,t.EndImage,r.IsReviewed,r.IsApproved,r.RefuseReason,r.ApprovedWorkTime,r.ReviewedBy from (select * from TimeSheet1 where UserId in (select UserId from User1 where BelongToDept=(select BelongToDept from User1 where UserId=@value1)) and WorkDate between @value2 and @value3) as t left join TimeReview as r on t.TimeSheetId=r.TimeSheetId) as tr left join User1 as u on tr.ReviewedBy=u.UserId) as tvv left join User1 as uu on tvv.UserId=uu.UserId";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@value1", userid);
                        cmd.Parameters.AddWithValue("@value2", begindate);
                        cmd.Parameters.AddWithValue("@value3", enddate);
                        SqlDataReader result = cmd.ExecuteReader();
                        if (result.HasRows)
                        {
                            while (result.Read())
                            {
                                TsForView TimeView = new TsForView();
                                TimeView.TimeSheetId = result.GetGuid(0);
                                TimeView.UserId = result.GetInt32(1);
                                TimeView.Date = result.GetDateTime(2).ToString();
                                if (!result.IsDBNull(3))
                                    TimeView.StartTime = result.GetTimeSpan(3).ToString(@"hh\:mm\:ss");
                                if (!result.IsDBNull(4))
                                    TimeView.StartIp = result.GetString(4);
                                if (!result.IsDBNull(5))
                                    TimeView.StartImage = (Byte[])result.GetValue(5);
                                if (!result.IsDBNull(6))
                                    TimeView.EndTime = result.GetTimeSpan(6).ToString(@"hh\:mm\:ss");
                                if (!result.IsDBNull(3) & !result.IsDBNull(6))
                                {
                                    TimeSpan duration = result.GetTimeSpan(6) - result.GetTimeSpan(3);
                                    //float hour = duration.Hours + (float)duration.Minutes / 60 + (float)duration.Seconds / 3600;
                                    TimeView.WorkDuration = duration.TotalHours.ToString("0.0");
                                }
                                if (!result.IsDBNull(7))
                                    TimeView.EndIp = result.GetString(7);
                                if (!result.IsDBNull(8))
                                    TimeView.EndImage = (Byte[])result.GetValue(8);
                                if (result.IsDBNull(9))
                                    TimeView.ReviewResult = Common.TsReviewResult.NoProcess;
                                else if (result.GetInt32(9) == 1 & result.GetInt32(10) == 1)
                                    TimeView.ReviewResult = Common.TsReviewResult.Approved;
                                else if (result.GetInt32(9) == 1 & result.GetInt32(10) == 0)
                                    TimeView.ReviewResult = Common.TsReviewResult.Refused;
                                if (!result.IsDBNull(11))
                                    TimeView.RefuseReason = result.GetString(11);
                                if (!result.IsDBNull(12))
                                    TimeView.ApprovedDuration = result.GetDouble(12).ToString("0.0");
                                if (!result.IsDBNull(14))
                                    TimeView.ReviewedBy = result.GetString(14);
                                if (!result.IsDBNull(15))
                                    TimeView.UserName = result.GetString(15);
                                TimeViews.Add(TimeView);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                conn.Close();
                return TimeViews;
            }
        }

        public static UserList GetUserList(int managerid)
        {
            Common.UserList Users = new UserList();
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();

                //get all User list report to this manger (id and name) 
                try
                {
                    //string sql = "select u.UserId, u.UserName from User1 as u , Dept as d where u.BelongToDept=d.DeptId and d.Manager=@value1 and u.UserId<>@value1 ";
                    string sql = "select u.UserId, u.UserName from User1 as u , Dept as d where u.BelongToDept=d.DeptId and d.Manager=@value1";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", managerid);
                    SqlDataReader result = cmd.ExecuteReader();
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            Users.UserDict.Add(result.GetString(1), result.GetInt32(0));
                        }
                        result.Close();
                    }
                    else
                    {
                        Users.UserDict = null;
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                conn.Close();
            }

            return Users;
        }

        public static bool ApproveTime(TsForView thistime)
        {
            float approvedduration = float.Parse(thistime.WorkDuration);
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                string sql = "insert into[TimeReview] (TimeSheetId, IsReviewed, IsApproved, ReviewedBy, ReviewDate,ApprovedWorkTime) values(@value1, @value2, @value3, @value4, @value5,@value6); ";
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", thistime.TimeSheetId);
                    cmd.Parameters.AddWithValue("@value2", 1);
                    cmd.Parameters.AddWithValue("@value3", 1);
                    cmd.Parameters.AddWithValue("@value4", thistime.ReviewedUserId);
                    cmd.Parameters.AddWithValue("@value5", thistime.ReviewDate);
                    cmd.Parameters.AddWithValue("@value6", approvedduration);
                    var result = cmd.ExecuteNonQuery();

                    if (result != 0 & result != -1)
                    {
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                conn.Close();
            }
            return false;
        }

    }
}


