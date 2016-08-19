using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Workday.Business;
using Workday.Common;
using System.Security.Cryptography;
using System.Data;

namespace Workday.Web
{
    public partial class Admin2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                
                string uid = Request.QueryString["userid"];
                string ustatus = Request.QueryString["currentstatus"];
                if (uid != null & uid != "" & ustatus != "" & ustatus != "")
                {
                    int id = Convert.ToInt32(uid);
                    int status = Convert.ToInt32(ustatus);
                    bool ifupdate = UserBusiness.ChangeUserStatus(id, status);
                    Response.Redirect("admin2.aspx");
                }
                BindGridView();

            }
        }

        private void BindGridView()
        {
            AllUserGridView.EditIndex = -1;
            List<IShowUsers> Users = new List<IShowUsers>();
            Users = UserBusiness.GetAllUsers2();
            AllUserGridView.DataSource = Users;
            AllUserGridView.DataBind();
        }

        protected void AllUserGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow & e.Row.RowState != (DataControlRowState.Edit | DataControlRowState.Alternate) & e.Row.RowState != DataControlRowState.Edit)
            {
                User user= ((User)e.Row.DataItem);
                Label deptlabel = e.Row.FindControl("Dept_Label") as Label;
                HyperLink disanden = e.Row.FindControl("HyperLink1") as HyperLink;
                disanden.Text = LinkText(user.StatusString);
                disanden.NavigateUrl = user.Disenlinkurl;
                if(Session["CurrentUser"].ToString()!=null& Session["CurrentUser"].ToString() != "")
                {
                    if (user.UserName == Session["CurrentUser"].ToString())
                        disanden.Visible = false;
                }
                if (AllUserGridView.DataKeys[e.Row.RowIndex]["DeptName"] != null)
                    deptlabel.Text = AllUserGridView.DataKeys[e.Row.RowIndex]["DeptName"].ToString();
                else
                    deptlabel.Text = "";
            }
        }

        private static string LinkText(string status)
        {
            if (status == "Normal")
            {
                return "Disable";
            }
            else
            {
                return "Enable";
            }
        }

        protected void PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            AllUserGridView.PageIndex = e.NewPageIndex;
            BindGridView();
        }

        protected void AllUserGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            AllUserGridView.EditIndex = e.NewEditIndex;
            List<IShowUsers> Users = new List<IShowUsers>();
            Users = UserBusiness.GetAllUsers2();
            AllUserGridView.DataSource = Users;
            AllUserGridView.DataBind();

            int userid = Int32.Parse(AllUserGridView.Rows[e.NewEditIndex].Cells[0].Text);
            int deptid;
            if (AllUserGridView.DataKeys[e.NewEditIndex]["DeptId"].ToString() != "" & AllUserGridView.DataKeys[e.NewEditIndex]["DeptId"] != null)
                deptid = Convert.ToInt32(AllUserGridView.DataKeys[e.NewEditIndex]["DeptId"].ToString());
            else
                deptid = 0;
            var DeptDrop = AllUserGridView.Rows[e.NewEditIndex].FindControl("Dept_Drop") as DropDownList;
            string ifmanager = AllUserGridView.Rows[e.NewEditIndex].Cells[5].Text;
            ParentList depts = new ParentList();
            depts = DeptBusiness.GetParentListForAdd();
            DeptDrop.DataSource = depts.ParentDict;
            DeptDrop.DataTextField = "Value";
            DeptDrop.DataValueField = "Key";
            DeptDrop.DataBind();
            DeptDrop.Items.Insert(0, new ListItem("null", "0"));
            DeptDrop.SelectedValue = deptid.ToString();
            if (ifmanager == "Manager")
            {
                DeptDrop.Enabled = false;
            }
        }

        protected void AllUserGridView_CancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            BindGridView();
        }

        protected void AllUserGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }
    }
}