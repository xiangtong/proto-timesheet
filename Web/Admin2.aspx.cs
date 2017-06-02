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
using System.Reflection;

namespace Workday.Web
{
    public partial class Admin2 : System.Web.UI.Page
    {
        private static bool IfInSorting;

        protected void Page_Load(object sender, EventArgs e)
        {
            IfInSorting = false;
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
            if (this.IsPostBack)
            {
                //submit add user
                string email = Request.Form["email"];
                string password = Request.Form["password"];
                string username = Request.Form["username"];
                if (email != "" & email != null & password != "" & password != null & username != "" & username != null)
                {
                    int dept = Convert.ToInt32(Dept_Drop.SelectedValue);
                    if (password == "The default password:111111")
                        password = "111111";
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
                        Label1.Text = "Add user successfully!";
                        BindGridView();
                    }
                    else if (newUser.UserId == -1)
                    {
                        Label1.Text = "user has existed!!";
                    }
                }
            }
        }

        private void BindGridView(int i=0)
        {
            if (AllUserGridView.Attributes["CurrentSortField"] == null && AllUserGridView.Attributes["CurrentSortDir"] == null)
            {
                if (i == 0)
                {  //default gridview bind, not in edit mode
                    AllUserGridView.EditIndex = -1;
                }
                List<IShowUsers> Users = new List<IShowUsers>();
                Users = UserBusiness.GetAllUsers2();
                AllUserGridView.DataSource = Users;
                AllUserGridView.DataBind();
            }
            else if (AllUserGridView.Attributes["CurrentSortField"] != null && AllUserGridView.Attributes["CurrentSortDir"] != null)
            {
                if (i == 0)
                {  //default gridview bind, not in edit mode
                    AllUserGridView.EditIndex = -1;
                }
                List<IShowUsers> Users = new List<IShowUsers>();
                Users = UserBusiness.GetAllUsers2();
                DataTable dataTable = new DataTable();
                dataTable = ConvertListToDataTable(Users);
                // with datatable2, convert first column(deptid) to int type to ensure the sorting result is correct
                DataTable dataTable2 = dataTable.Clone();
                dataTable2.Columns["UserId"].DataType = Type.GetType("System.Int32");
                foreach (DataRow dr in dataTable.Rows)
                {
                    dataTable2.ImportRow(dr);
                }
                if (dataTable2 != null)
                {
                    DataView dataView = new DataView(dataTable2);
                    //string Expression;
                    SortDirection Direction;
                    //GridViewSortDirection(AllUserGridView, e, out Direction, out Expression);
                    GridSortDircetion_Rebind(out Direction);
                    dataView.Sort = AllUserGridView.Attributes["CurrentSortField"] + " " + ConvertSortDirectionToSql(Direction);
                    //set ifinsorting as true so that rowdatabound could tell if in sorting
                    IfInSorting = true;
                    AllUserGridView.DataSource = dataView;
                    AllUserGridView.DataBind();
                }
            }
        }

        protected void AllUserGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //highlight current sorting column head
            //LinkButton lnkbtn;
            GridView gridView = (GridView)sender;
            string SortColumn = AllUserGridView.Attributes["CurrentSortField"];
            string SortDirect = AllUserGridView.Attributes["CurrentSortDir"];
            if (e.Row.RowType == DataControlRowType.Header)
            {
                int cellIndex = -1;
                foreach (DataControlField field in gridView.Columns)
                {
                    if (field.SortExpression == SortColumn)
                    {
                        cellIndex = gridView.Columns.IndexOf(field);
                    }
                }
                if (cellIndex > -1)
                {
                    //  this is a header row, set the sort style
                    e.Row.Cells[cellIndex].CssClass = SortDirect == "ASC" ? "sortasc" : "sortdesc";
                }
                //foreach (TableCell cell in e.Row.Cells)
                //{
                //string temp = e.Row.Cells[i].Text;
                //if (!string.IsNullOrEmpty(SortExpression))
                //{
                //    if (SortExpression == lnkbtn.Text)
                //    {
                //        cell.BackColor = System.Drawing.Color.Crimson;
                //    }
                //}
                //}
            }
            if (e.Row.RowType == DataControlRowType.DataRow & e.Row.RowState != (DataControlRowState.Edit | DataControlRowState.Alternate) & e.Row.RowState != DataControlRowState.Edit & IfInSorting == false)
            {   //when in normal mode( not edit and not sorting)
                User user = ((User)e.Row.DataItem);
                Label deptlabel = e.Row.FindControl("Dept_Label") as Label;
                HyperLink disanden = e.Row.FindControl("HyperLink1") as HyperLink;
                disanden.Text = LinkText(user.StatusString);
                disanden.NavigateUrl = user.Disenlinkurl;
                if (Session["CurrentUser"].ToString() != null & Session["CurrentUser"].ToString() != "")
                {
                    if (user.UserName == Session["CurrentUser"].ToString())
                        disanden.Visible = false;
                }
                if (AllUserGridView.DataKeys[e.Row.RowIndex]["DeptName"] != null)
                    deptlabel.Text = AllUserGridView.DataKeys[e.Row.RowIndex]["DeptName"].ToString();
                else
                    deptlabel.Text = "";
                // disable edit button when user is a manager
                if (user.IsManager == "Manager")
                {
                    e.Row.Cells[8].Enabled = false;
                }
            }
            else if (e.Row.RowType == DataControlRowType.DataRow & e.Row.RowState != (DataControlRowState.Edit | DataControlRowState.Alternate) & e.Row.RowState != DataControlRowState.Edit & IfInSorting == true)
            {   //when sorting
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                DataRow row = rowView.Row;
                User user = new Common.User();
                user.StatusString = row["StatusString"].ToString();
                user.Disenlinkurl = row["Disenlinkurl"].ToString();
                user.UserName = row["UserName"].ToString();
                user.IsManager = row["IsManager"].ToString();
                // to generate disable/enable link's visibility and deptlable's text
                Label deptlabel = e.Row.FindControl("Dept_Label") as Label;
                HyperLink disanden = e.Row.FindControl("HyperLink1") as HyperLink;
                disanden.Text = LinkText(user.StatusString);
                disanden.NavigateUrl = user.Disenlinkurl;
                if (Session["CurrentUser"].ToString() != null & Session["CurrentUser"].ToString() != "")
                {
                    if (user.UserName == Session["CurrentUser"].ToString())
                        disanden.Visible = false;
                }
                if (AllUserGridView.DataKeys[e.Row.RowIndex]["DeptName"] != null)
                    deptlabel.Text = AllUserGridView.DataKeys[e.Row.RowIndex]["DeptName"].ToString();
                else
                    deptlabel.Text = "";
                // disable edit button when user is a manager
                if (user.IsManager == "Manager")
                {
                    e.Row.Cells[8].Enabled = false;
                }
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
            BindGridView(1);  //1 means in edit mode
            int userid = Int32.Parse(AllUserGridView.Rows[e.NewEditIndex].Cells[0].Text);
            int deptid=0;
            if ( AllUserGridView.DataKeys[e.NewEditIndex]["DeptId"] != null)
            {
                if(AllUserGridView.DataKeys[e.NewEditIndex]["DeptId"].ToString() != "")
                {
                    deptid = Convert.ToInt32(AllUserGridView.DataKeys[e.NewEditIndex]["DeptId"].ToString());
                }
            }
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
            //if user is manager, you could not change dept here. You need to change this user not as the manger of the dept in dept admin aspx first. thne you could change this user's dept.
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
            User user = new User();
            var DeptDrop = AllUserGridView.Rows[e.RowIndex].FindControl("Dept_Drop") as DropDownList;
            GridViewRow Row = AllUserGridView.Rows[e.RowIndex];
            user.UserId = Convert.ToInt32(Row.Cells[0].Text);
            if (Int32.Parse(DeptDrop.SelectedValue.ToString()) != 0)
                user.DeptId = Int32.Parse(DeptDrop.SelectedValue.ToString());

            validatresult result = UserBusiness.ChangeDeptValidate(user);
            if (result.resultid == -1)
            {
                BindGridView();
                Response.Write("<script>alert('" + result.resultdesc + "')</script>");
            }
            else
            {
                bool ifupdate = UserBusiness.ChangeUserDept(user);
                if (ifupdate)
                {
                    BindGridView();
                }
                else
                {
                    BindGridView();
                    Response.Write("<script>alert('update failed for unknown reason!')</script>");
                }
            }
        }

        protected void AllUserGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            List<IShowUsers> Users = new List<IShowUsers>();
            Users = UserBusiness.GetAllUsers2();
            DataTable dataTable = new DataTable();
            dataTable = ConvertListToDataTable(Users);
            // with datatable2, convert first column(deptid) to int type to ensure the sorting result is correct
            DataTable dataTable2 = dataTable.Clone();
            dataTable2.Columns["UserId"].DataType = Type.GetType("System.Int32");
            foreach (DataRow dr in dataTable.Rows)
            {
                dataTable2.ImportRow(dr);
            }
            if (dataTable2 != null)
            {
                DataView dataView = new DataView(dataTable2);
                string Expression;
                SortDirection Direction;
                GridViewSortDirection(AllUserGridView, e, out Direction, out Expression);
                dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(Direction);
                //set ifinsorting as true so that rowdatabound could tell if in sorting
                IfInSorting = true;
                AllUserGridView.DataSource = dataView;
                AllUserGridView.DataBind();
            }
        }

        private string ConvertSortDirectionToSql(SortDirection sortDirection)
        {
            string newSortDirection = String.Empty;
            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    newSortDirection = "ASC";
                    break;
                case SortDirection.Descending:
                    newSortDirection = "DESC";
                    break;
            }
            return newSortDirection;
        }

        private void GridViewSortDirection(GridView g, GridViewSortEventArgs e, out SortDirection d, out string f)
        {
            f = e.SortExpression;
            d = e.SortDirection;
            //Check if GridView control has required Attributes
            if (g.Attributes["CurrentSortField"] != null && g.Attributes["CurrentSortDir"] != null)
            {
                if (f == g.Attributes["CurrentSortField"])
                {
                    d = SortDirection.Descending;
                    if (g.Attributes["CurrentSortDir"] == "ASC")
                    {
                        d = SortDirection.Ascending;
                    }
                }
            }
            g.Attributes["CurrentSortField"] = f;
            g.Attributes["CurrentSortDir"] = (d == SortDirection.Ascending ? "DESC" : "ASC");
        }

        private void GridSortDircetion_Rebind(out SortDirection d)
        {
            d = AllUserGridView.SortDirection;
            if (AllUserGridView.Attributes["CurrentSortField"] != null && AllUserGridView.Attributes["CurrentSortDir"] != null)
            {
                if (AllUserGridView.Attributes["CurrentSortField"] == "DESC")
                {
                    d = SortDirection.Ascending;
                }
                else if (AllUserGridView.Attributes["CurrentSortDir"] == "ASC")
                {
                    d = SortDirection.Descending;
                }
            }
        }

        protected static DataTable ConvertListToDataTable<IShowDepts>(List<IShowDepts> items)
        {
            DataTable dataTable = new DataTable(typeof(IShowDepts).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(IShowDepts).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (IShowDepts item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        protected void TrigerAdd_Click(object sender, EventArgs e)
        {
            AddUserPanel.Visible = true;
            TrigerAdd.Visible = false;

            ParentList depts = new ParentList();
            depts = DeptBusiness.GetParentListForAdd();
            Dept_Drop.DataSource = depts.ParentDict;
            Dept_Drop.DataTextField = "Value";
            Dept_Drop.DataValueField = "Key";
            Dept_Drop.DataBind();
            Dept_Drop.Items.Insert(0, new ListItem("null", "0"));
            Dept_Drop.SelectedValue = Convert.ToString(0);

        }

        protected void AddUserCancel_Click(object sender, EventArgs e)
        {
            AddUserPanel.Visible = false;
            TrigerAdd.Visible = true;
        }

    }
}