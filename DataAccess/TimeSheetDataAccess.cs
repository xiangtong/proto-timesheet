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
    }
}


