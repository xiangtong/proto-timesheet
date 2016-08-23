using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Workday.Business;
using Workday.Common;

namespace Workday.Web
{
    public partial class Register : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {

            string from=null;

            if (Request.QueryString["from"] != null)
            {
                from = Request.QueryString["from"];
            }

            if (!this.IsPostBack)
            {
                ParentList depts = new ParentList();
                depts = DeptBusiness.GetParentListForAdd();
                Dept_Drop.DataSource = depts.ParentDict;
                Dept_Drop.DataTextField = "Value";
                Dept_Drop.DataValueField = "Key";
                Dept_Drop.DataBind();
                Dept_Drop.Items.Insert(0, new ListItem("null", "0"));
                Dept_Drop.SelectedValue = Convert.ToString(0);
            }
            if (this.IsPostBack)
            {
                //提交登录
                string email = Request.Form["email"];
                string password = Request.Form["password"];
                string username = Request.Form["username"];
                int dept = Convert.ToInt32(Dept_Drop.SelectedValue);
                
                if(email!="" & email!=null & password!="" & password!=null & username!="" & username!=null)
                {
                    if(password== "The default password:111111")
                    {
                        password = "111111";
                    }
                    Common.User newUser = new Common.User();
                    newUser.Email = email;
                    newUser.Password = password;
                    newUser.UserName = username;
                    newUser.Status = Common.UserStatus.Normal;
                    newUser.IsAdmin = Common.UserIsAdmin.NotAdmin;
                    if (dept != 0)
                        newUser.DeptId = dept;
                    newUser = UserBusiness.AddUser(newUser);
                    if (newUser != null && newUser.UserId > 0)
                    { //注册成功
                        if (from != null) { 
                        Response.Redirect(from);
                        }
                        else
                        {
                            Response.Redirect("Admin.aspx");
                        }
                    }
                    else if (newUser.UserId == -1)
                    {
                        Label1.Text = "user has existed!!";
                    }
                }
            }
        }
    }
}