using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using Workday.Common;

namespace Workday.DataAccess
{
    public class UserDataAccess
    {
        /*private static string _connectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnectionString"];
        private static string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();*/
        private static string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Myconnection"].ToString();
        public static User GetUserById(int userId)
        {
            return null;
        }

        public static User GetUserByName(string name)
        {
            // return null;
            Common.User user = new Common.User();
            //int iuserstatus;
            string sql = "select * from [User1] where UserName='" + name + "'";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader result = cmd.ExecuteReader();

                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            user.UserId = result.GetInt32(0);
                            user.UserName = result.GetString(2);
                            user.Password = result.GetString(3);
                            user.Status = (UserStatus)result.GetInt32(4);
                            user.IsAdmin = (UserIsAdmin)result.GetInt32(5);
                            if(!result.IsDBNull(7) ){ 
                                user.LoginDate = result.GetDateTime(7);
                            }
                            else
                            {
                                user.LoginDate = null;
                            }
                        }
                        result.Close();
                    }
                    else
                    {
                        user = null;
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

            return user;
        }
        
        
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static User AddUser(User user)
        {
#warning 需要防止sql注入，处理字符串或者事情存储过程；需要防止用户名重复；密码需要哈希，加盐
            bool ifexist = CheckDuplicateCustomer(user.Email,user.UserName);
            if (ifexist == false)
            {
                user.UserId = -1;
            }
            else
            {
                int UserStatusint;
                int IsAdminint;
                UserStatusint = (int)user.Status;
                IsAdminint = (int)user.IsAdmin;
                //SHA hash password with salt before write to DB
                user.Password = User.GenerateHash(user.Password);
                DateTime now = DateTime.Now;
                //string sql = "insert into [User] (Email, UserName, Password, UserStatus, IsAdmin) values ('" + user.Email + "', '" + user.UserName + "', '" + user.Password + "','" + UserStatusint + "','" + IsAdminint + "'); select @@identity;";
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    try
                    {
                        string sql = "insert into [User1] (Email, UserName, Password, UserStatus, IsAdmin,CreateDate) values (@value1, @value2, @value3,@value4, @value5,@value6); select @@identity;";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@value1", user.Email);
                        cmd.Parameters.AddWithValue("@value2", user.UserName);
                        cmd.Parameters.AddWithValue("@value3", user.Password);
                        cmd.Parameters.AddWithValue("@value4", UserStatusint);
                        cmd.Parameters.AddWithValue("@value5", IsAdminint);
                        cmd.Parameters.AddWithValue("@value6", now);
                        var result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            user.UserId = Convert.ToInt32(result);//填充UserId
                        }
                    }
                    catch (SqlException ex)
                    {

                        if (ex.ErrorCode == -2146232060)
                        {
                            user.UserId = -1;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    conn.Close();
                }
            }

            return user;
        }

        public static Object ExecuteSqlString(string sqlstring)
        {
            SqlConnection objsqlconn = new SqlConnection(_connectionString);
            objsqlconn.Open();
            DataSet ds = new DataSet();
            SqlCommand objcmd = new SqlCommand(sqlstring, objsqlconn);
            SqlDataAdapter objAdp = new SqlDataAdapter(objcmd);
            objAdp.Fill(ds);
            return ds;
        }

        //this is the old mothed, only used by admin.aspx, should be deprecated. the new mothed is GetAllUser2
        public static AllUser GetAllUser(Common.AllUser users)
        {
            //DataSet ds = new DataSet();
            string sql = "SELECT UserID,Email,UserName,UserStatus,CreateDate from [User1]";
            users.AllUserSet = (DataSet)ExecuteSqlString(sql);
            return users;
        }

        public static List<IShowUsers> GetAllUser2()
        {
            List<IShowUsers> Users = new List<IShowUsers>();
            string sql = "SELECT u.UserID,u.Email,u.UserName,u.UserStatus,d.DeptId, d.DeptName,d.Manager,u.CreateDate from [User1] as u left join Dept as d on u.BelongToDept=d.DeptId order by UserId";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader result = cmd.ExecuteReader();

                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            User user = new User();
                            user.UserId = result.GetInt32(0);
                            user.Email = result.GetString(1);
                            user.UserName = result.GetString(2);
                            if (result.GetInt32(3) == 0)
                                user.Status = Common.UserStatus.Normal;
                            else if(result.GetInt32(3)==1)
                                user.Status = Common.UserStatus.Forbidden;
                            if (!result.IsDBNull(4))
                                user.DeptId = result.GetInt32(4);
                            if (!result.IsDBNull(5))
                                user.DeptName = result.GetString(5);
                            if (!result.IsDBNull(6))
                                user.Manager = result.GetInt32(6);
                            user.CreateDate = result.GetDateTime(7);
                            Users.Add(user);
                        }
                    }
                    else
                    {
                        Users = null;
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


        public static SecretQuestion GetSecretQuestion()
        {
            Common.SecretQuestion Secret = new SecretQuestion();
            //int iuserstatus;
            string sql = "select * from [SecretQuestion]";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader result = cmd.ExecuteReader();

                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            Secret.QuestionList.Add(result.GetInt32(0), result.GetString(1));
                        }
                        result.Close();
                    }
                    else
                    {
                        Secret = null;
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

            return Secret;
        }


        public static bool ChangeUserStatus(int userid, int currentstatus)
        {

            bool ifChangeStatus = false;
            int newstatus=0;
            if (currentstatus == 1)
                newstatus = 0;
            else if (currentstatus == 0)
                newstatus = 1;
            string sql = "update [User1] set UserStatus="+newstatus+" where UserID="+userid;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var result = cmd.ExecuteNonQuery();

                    if (result != 0 & result!=-1)
                    {
                        ifChangeStatus = true;
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

            return ifChangeStatus;
        }

        public static bool ChangeDefaultPassword(User user)
        {
            bool ifUpdate = false;
            user.Password = User.GenerateHash(user.Password);
            user.LoginDate = DateTime.Now;

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = _connectionString;
            conn.Open();
            if (conn.State == System.Data.ConnectionState.Open)
            {
                try {

                    string sql = "update [User1] set Password=@value1, SecretQuestion=@value2, SecretAnswer=@value3, LastLoginDate=@value4 where UserName=@value5";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", user.Password);
                    cmd.Parameters.AddWithValue("@value2", user.SecretQuestion);
                    cmd.Parameters.AddWithValue("@value3", user.SecretAnswer);
                    cmd.Parameters.AddWithValue("@value4", user.LoginDate);
                    cmd.Parameters.AddWithValue("@value5", user.UserName);
                    var result = cmd.ExecuteNonQuery();

                    if (result != 0 & result != -1)
                    {
                        ifUpdate = true;
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
            return ifUpdate;

        }

        static bool CheckDuplicateCustomer(string email,string name)
        {
            int CustomerCount=0;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = _connectionString;
            con.Open();
            if (con.State == System.Data.ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand("select count(*) from [user1] where Email = @email or UserName=@name", con);
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@name", name);
                CustomerCount = Convert.ToInt32(cmd.ExecuteScalar());
            }
            con.Close();
            if (CustomerCount > 0)
                return false;
            else
                return true;
        }

        public static bool ChangeUserDept(User user)
        {
            bool ifUpdate = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = _connectionString;
            conn.Open();
            if (conn.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    string sql = "update [User1] set Dept=@value1 where UserName=@value2";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", user.DeptId);
                    cmd.Parameters.AddWithValue("@value2", user.UserId);
                    var result = cmd.ExecuteNonQuery();
                    if (result != 0 & result != -1)
                    {
                        ifUpdate = true;
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
            return ifUpdate;

        }
    }

}