using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Workday.Common;
using System.Data;

namespace Workday.DataAccess
{
    public class DeptDataAccess
    {

        private static string _conn = System.Configuration.ConfigurationManager.ConnectionStrings["Myconnection"].ToString();

        public static bool AddDept(Dept newdept, int conflictdeptid)
        {
            DateTime now = DateTime.Now;
            bool ifadd = false;
            int deptid=0;
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                try
                {   //update dept table and get new deptid
                    SqlCommand cmd = new SqlCommand();
                    if (newdept.Manager.HasValue & newdept.ParentDept.HasValue &conflictdeptid==0)
                    {  //no manager conflict
                        string sql = "insert into [Dept] (DeptName, Manager, ParentDept, CreateDate) values (@value1, @value2, @value3,@value4);SELECT SCOPE_IDENTITY(); ";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@value1", newdept.DeptName);
                        cmd.Parameters.AddWithValue("@value2", newdept.Manager);
                        cmd.Parameters.AddWithValue("@value3", newdept.ParentDept);
                        cmd.Parameters.AddWithValue("@value4", now);
                    }
                    else if (newdept.Manager.HasValue & newdept.ParentDept.HasValue & conflictdeptid != 0)
                    { //manager conflict
                        string sql = "insert into [Dept] (DeptName, Manager, ParentDept, CreateDate) values (@value1, @value2, @value3,@value4);SELECT SCOPE_IDENTITY();update Dept set Manager=NULL where DeptId=@value5;";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@value1", newdept.DeptName);
                        cmd.Parameters.AddWithValue("@value2", newdept.Manager);
                        cmd.Parameters.AddWithValue("@value3", newdept.ParentDept);
                        cmd.Parameters.AddWithValue("@value4", now);
                        cmd.Parameters.AddWithValue("@value5", conflictdeptid);
                    }
                    else if (newdept.Manager.HasValue & !newdept.ParentDept.HasValue & conflictdeptid == 0)
                    { //no manager conflict
                        string sql = "insert into [Dept] (DeptName, Manager, CreateDate) values (@value1, @value2, @value3);SELECT SCOPE_IDENTITY();";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@value1", newdept.DeptName);
                        cmd.Parameters.AddWithValue("@value2", newdept.Manager);
                        cmd.Parameters.AddWithValue("@value3", now);
                    }
                    else if (newdept.Manager.HasValue & !newdept.ParentDept.HasValue & conflictdeptid != 0)
                    { // manager conflict
                        string sql = "insert into [Dept] (DeptName, Manager, CreateDate) values (@value1, @value2, @value3); SELECT SCOPE_IDENTITY(); update Dept set Manager=NULL where DeptId=@value4 ";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@value1", newdept.DeptName);
                        cmd.Parameters.AddWithValue("@value2", newdept.Manager);
                        cmd.Parameters.AddWithValue("@value3", now);
                        cmd.Parameters.AddWithValue("@value4", conflictdeptid);
                    }
                    else if (!newdept.Manager.HasValue & newdept.ParentDept.HasValue)
                    {
                        string sql = "insert into [Dept] (DeptName, ParentDept, CreateDate) values (@value1, @value2, @value3);SELECT SCOPE_IDENTITY(); ";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@value1", newdept.DeptName);
                        cmd.Parameters.AddWithValue("@value2", newdept.ParentDept);
                        cmd.Parameters.AddWithValue("@value3", now);
                    }
                    else
                    {
                        string sql = "insert into [Dept] (DeptName, CreateDate) values (@value1, @value2);SELECT SCOPE_IDENTITY(); ";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@value1", newdept.DeptName);
                        cmd.Parameters.AddWithValue("@value2", now);
                    }
                   
                    var result = cmd.ExecuteScalar();
                    if (!(result is DBNull) & result!=null)
                    {
                        deptid = Convert.ToInt32(result); //ifadd = true ;
                        if (!newdept.Manager.HasValue)
                            ifadd = true;
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
                if (deptid != 0 & newdept.Manager.HasValue)
                {
                    try
                    {   //update user1 table base on new deptid
                        SqlCommand cmd = new SqlCommand();
                        string sql = "update User1 set BelongToDept=@value1 where UserId=@value2; ";
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@value1", deptid);
                        cmd.Parameters.AddWithValue("@value2", newdept.Manager);
                        int result1 = cmd.ExecuteNonQuery();
                        if (result1 != 0 & result1 != -1)
                            ifadd = true;
                        conn.Close();
                    }
                    catch (SqlException ex)
                    {
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

            }
            return ifadd;
        }

        public static List<IShowDepts> IGetAllDepts()
        {
            List<IShowDepts> Depts = new List<IShowDepts>();
            string sql = "select d.DeptId,d.DeptName,d.ParentName,e.UserName as ManagerName,d.CreateDate from(select a.DeptId, a.DeptName, a.CreateDate,b.DeptName as ParentName, a.Manager from Dept a LEFT join Dept b on a.ParentDept = b.DeptId) as d left join[User1] as e on d.Manager = e.UserId";
            using (SqlConnection conn = new SqlConnection(_conn))
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
                            Workday.Common.Dept dept = new Workday.Common.Dept();
                            dept.DeptId = result.GetInt32(0);
                            dept.DeptName = result.GetString(1);
                            if(!result.IsDBNull(2))
                                dept.ParentName = result.GetString(2);
                            if (!result.IsDBNull(3))
                                dept.ManagerName = result.GetString(3);
                            if (!result.IsDBNull(4))
                                dept.CreateDate = result.GetDateTime(4);
                            Depts.Add(dept);
                        }
                    }
                    else
                    {
                        Depts = null;
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

            return Depts;
        }

        public static List<Workday.Common.Dept> GetAllDepts()
        {
            List<Workday.Common.Dept> Depts = new List<Workday.Common.Dept>();
            string sql = "select d.DeptId,d.DeptName,d.ParentId,d.ParentName,e.UserId as ManagerId, e.UserName as ManagerName from(select a.DeptId, a.DeptName, b.DeptId as ParentId, b.DeptName as ParentName, a.Manager from Dept a LEFT join Dept b on a.ParentDept = b.DeptId) as d left join[User1] as e on d.Manager = e.UserId";
            using (SqlConnection conn = new SqlConnection(_conn))
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
                            Workday.Common.Dept dept = new Workday.Common.Dept();
                            dept.DeptId = result.GetInt32(0);
                            dept.DeptName = result.GetString(1);
                            if (!result.IsDBNull(2))
                                dept.ParentDept = result.GetInt32(2);
                            if (!result.IsDBNull(3))
                                dept.ManagerName = result.GetString(3);
                            if (!result.IsDBNull(4))
                                dept.Manager = result.GetInt32(4);
                            if (!result.IsDBNull(5))
                                dept.ManagerName = result.GetString(5);
                            Depts.Add(dept);
                        }
                    }
                    else
                    {
                        Depts = null;
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

            return Depts;
        }

        public static  ParentList GetParentList()
        {
            Common.ParentList Parents = new ParentList();

            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();

                //get all depts list (id and name) 
                try
                {
                    //string sql = "select DeptId, DeptName from Dept  where DeptId<>@value1";
                    string sql = "select DeptId, DeptName from Dept";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    //cmd.Parameters.AddWithValue("@value1", i);
                    SqlDataReader result = cmd.ExecuteReader();

                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            Parents.ParentDict.Add(result.GetInt32(0),result.GetString(1));
                        }
                        result.Close();
                    }
                    else
                    {
                        Parents.ParentDict = null;
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
            return Parents;
        }

        public static int GetParent(int i)
        {
            int parentid;
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                //get the parent id of current dept
                try
                {
                    string sql = "select ParentDept from Dept where DeptId=@value1";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", i);
                    object result = cmd.ExecuteScalar();
                    if (!(result is DBNull))
                    {
                        parentid = Convert.ToInt32(result);
                    }
                    else
                    {
                        parentid = 0;
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
            return parentid;
        }

        public static ManagerList GetManagerList(int i)
        {
            Common.ManagerList Manager = new ManagerList();
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();

                //get all manger list (id and name) 
                try
                {
                    string sql = "select UserId, UserName from [User1]";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    //cmd.CommandType = CommandType.Text;
                    SqlDataReader result = cmd.ExecuteReader();

                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            Manager.ManagerDict.Add(result.GetString(1), result.GetInt32(0));
                        }
                        result.Close();
                    }
                    else
                    {
                        Manager.ManagerDict = null;
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

                //get the manager id of current dept
                try
                {
                    string sql = "select Manager from Dept where DeptId=@value1";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", i);
                    object result = cmd.ExecuteScalar();
                    if (!(result is DBNull))
                    {
                        Manager.CurrentSelect = Convert.ToInt32(result);
                    }
                    else
                    {
                        Manager.CurrentSelect = 0;
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

            return Manager;
        }

        //when no manager conflict, all this mothed
        public static bool UpdateDept(Dept dept)
        {
            bool ifUpdate = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = _conn;
            conn.Open();
            if (conn.State == System.Data.ConnectionState.Open)
            {
                try
                {   //如果部门原理有manager，则需要在update之前，将原来manager 的user表中的dept设置为null
                    string sql = "select Manager from [Dept] where DeptId=@value1";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", dept.DeptId);
                    var result = cmd.ExecuteScalar();
                    if (!(result is DBNull))
                    {
                        string sql1 = "update [User1] set BelongToDept=NULL where UserId=@value2";
                        SqlCommand cmd1 = new SqlCommand(sql1, conn);
                        cmd1.Parameters.AddWithValue("@value2", Convert.ToInt32(result));
                        var result1 = cmd1.ExecuteNonQuery();
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
                try
                {

                    string sql = "update [Dept] set DeptName=@value1, ParentDept=@value2, Manager=@value3 where DeptId=@value4";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", dept.DeptName);
                    if (dept.ParentDept == null)
                        cmd.Parameters.AddWithValue("@value2", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@value2", dept.ParentDept);
                    if (dept.Manager == null)
                        cmd.Parameters.AddWithValue("@value3", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@value3", dept.Manager);
                    cmd.Parameters.AddWithValue("@value4", dept.DeptId);
                    var result = cmd.ExecuteNonQuery();

                    if (result ==1)
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
                if (dept.Manager != null)
                {  //if a dept has manger, then you need to update user table to set that user belong to that dept
                    try
                    {

                        string sql = "update [User1] set BelongToDept=@value1 where UserId=@value2";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@value1", dept.DeptId);
                        cmd.Parameters.AddWithValue("@value2", dept.Manager);
                        var result = cmd.ExecuteNonQuery();
                        if (result == 1)
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
                }

                conn.Close();
            }
            return ifUpdate;
        }

        public static bool UpdateDept(Dept dept,int conflictdeptid)
        {
            bool ifUpdate = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = _conn;
            conn.Open();
            if (conn.State == System.Data.ConnectionState.Open)
            {
                try
                {   //如果部门原理有manager，则需要在update之前，将原来manager 的user表中的dept设置为null
                    string sql = "select Manager from [Dept] where DeptId=@value1";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", dept.DeptId);
                    var result = cmd.ExecuteScalar();

                    if (!(result is DBNull))
                    {
                        string sql1 = "update [User1] set BelongToDept=NULL where UserId=@value2";
                        SqlCommand cmd1 = new SqlCommand(sql1, conn);
                        cmd1.Parameters.AddWithValue("@value2", Convert.ToInt32(result));
                        var result1 = cmd1.ExecuteNonQuery();
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
                try
                {

                    string sql = "update [Dept] set DeptName=@value1, ParentDept=@value2, Manager=@value3 where DeptId=@value4; update [Dept] set Manager=NULL where DeptId=@value5; update [User1] set BelongToDept=@value4 where UserId=@value3"; 
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", dept.DeptName);
                    if (dept.ParentDept == null)
                        cmd.Parameters.AddWithValue("@value2", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@value2", dept.ParentDept);
                    cmd.Parameters.AddWithValue("@value3", dept.Manager);
                    cmd.Parameters.AddWithValue("@value4", dept.DeptId);
                    cmd.Parameters.AddWithValue("@value5", conflictdeptid);
                    var result = cmd.ExecuteNonQuery();

                    if (result ==3)
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

        public static int GetUserCountForADept(int deptid)
        {
            int usercount;
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                //get the parent id of current dept
                try
                {
                    string sql = "select count(*) from User1 where BelongToDept=@value1";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", deptid);
                    usercount = (int)cmd.ExecuteScalar();
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
            return usercount;
        }

        public static int GetSubDeptCountForADept(int deptid)
        {
            int subdeptcount;
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                //get the parent id of current dept
                try
                {
                    string sql = "select count(*) from Dept where ParentDept=@value1";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", deptid);
                    subdeptcount = (int)cmd.ExecuteScalar();
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
            return subdeptcount;
        }
        public static bool DeleteDept(int deptid)
        {
            bool ifdelete = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = _conn;
            conn.Open();
            if (conn.State == System.Data.ConnectionState.Open)
            {
                try
                {

                    string sql = "delete from [Dept] where DeptId=@value1";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@value1", deptid);
                    var result = cmd.ExecuteNonQuery();
                    if (result == 1)
                    {
                        ifdelete = true;
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
            return ifdelete;
        }
    }
}
