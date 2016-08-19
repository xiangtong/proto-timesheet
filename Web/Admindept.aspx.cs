using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Workday.Common;
using Workday.Business;
using System.Data;
using System.Reflection;

namespace Workday.Web
{
    public partial class Admindept : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                BindGrid();

                //if from user confirmation for update, then get dept info from cookie and update or add to DB
                string ifupdate = Request.QueryString["ifu"];
                string ifadd = Request.QueryString["ifa"];
                if (ifupdate == "confirm")
                {  //update dept to db
                    Dept dept = new Dept();
                    if (Request.Cookies["UpdateDept"] != null)
                    {
                        int deptid2=0;
                        if (Request.Cookies["UpdateDept"]["Id"] != null)
                            dept.DeptId = Convert.ToInt32(Request.Cookies["UpdateDept"]["Id"]);
                        if (Request.Cookies["UpdateDept"]["Name"] != null)
                            dept.DeptName= Request.Cookies["UpdateDept"]["Name"];
                        if (Request.Cookies["UpdateDept"]["Parent"] != null)
                            dept.ParentDept = Convert.ToInt32(Request.Cookies["UpdateDept"]["Parent"]);
                        if (Request.Cookies["UpdateDept"]["Manager"] != null)
                            dept.Manager = Convert.ToInt32(Request.Cookies["UpdateDept"]["Manager"]);
                        if (Request.Cookies["UpdateDept"]["ConflictDeptId"] != null)
                            deptid2 = Convert.ToInt32(Request.Cookies["UpdateDept"]["ConflictDeptId"]);
                        if (Request.Cookies["UpdateDept"]["Id"] != null & Request.Cookies["UpdateDept"]["ConflictDeptId"] != null)
                        {
                            updateresult updatedeptresult = DeptBusiness.UpdateDept(dept, deptid2);
                            int a = updatedeptresult.resultid;
                            if (a == 5 || a == 6)
                            {   //update successfully! or exception
                                BindGrid();
                                NewDeptName.Text = "";
                                ManagerDropForAdd.SelectedIndex = 0;
                                ParentDropForAdd.SelectedIndex = 0;
                                Response.Write("<script>alert('" + updatedeptresult.resultdesc + "')</script>");
                            }
                        }
                        //delete cookie after get info from it
                        HttpCookie myCookie = new HttpCookie("UpdateDept");
                        myCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(myCookie);

                    }
                }
                if (ifadd == "confirm")
                {  //add new dept into db
                    Dept dept = new Dept();
                    if (Request.Cookies["NewDept"] != null)
                    {
                        int deptid2 = 0;
                        if (Request.Cookies["NewDept"]["Name"] != null)
                            dept.DeptName = Request.Cookies["NewDept"]["Name"];
                        if (Request.Cookies["NewDept"]["Parent"] != null)
                            dept.ParentDept = Convert.ToInt32(Request.Cookies["NewDept"]["Parent"]);
                        if (Request.Cookies["NewDept"]["Manager"] != null)
                            dept.Manager = Convert.ToInt32(Request.Cookies["NewDept"]["Manager"]);
                        if (Request.Cookies["NewDept"]["ConflictDeptId"] != null)
                            deptid2 = Convert.ToInt32(Request.Cookies["NewDept"]["ConflictDeptId"]);
                        if (Request.Cookies["NewDept"]["Name"] != null & Request.Cookies["NewDept"]["ConflictDeptId"] != null)
                        {
                            updateresult updatedeptresult = DeptBusiness.AddDept(dept, deptid2);
                            int a = updatedeptresult.resultid;
                            if (a == 5 || a == 6)
                            {   //update successfully! or exception
                                BindGrid();
                                Response.Write("<script>alert('" + updatedeptresult.resultdesc + "')</script>");
                            }
                        }
                        //delete cookie after get info from it
                        HttpCookie myCookie = new HttpCookie("NewDept");
                        myCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(myCookie);

                    }
                }
            }
            //if user click cancel in the manager conflict dialog, then also need to delete cookie.
            if (Request.Cookies["UpdateDept"] != null)
            {
                //delete cookie after get info from it
                HttpCookie myCookie = new HttpCookie("UpdateDept");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }
            if (Request.Cookies["NewDept"] != null)
            {
                //delete cookie after get info from it
                HttpCookie myCookie = new HttpCookie("NewDept");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }
        }

        protected void AllDeptsGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            AllDeptsGridView.EditIndex = e.NewEditIndex;
            int deptid = Int32.Parse(AllDeptsGridView.Rows[e.NewEditIndex].Cells[0].Text);
            //string deptname = AllDeptsGridView.Rows[e.NewEditIndex].Cells[1].Text;
            List<IShowDepts> Depts = new List<IShowDepts>();
            Depts = DeptBusiness.GetAllDepts();
            AllDeptsGridView.DataSource = Depts;
            AllDeptsGridView.DataBind();

            //var NameText = AllDeptsGridView.Rows[e.NewEditIndex].FindControl("Dept_Name_Text") as TextBox;
            //NameText.Text = deptname;

            var ParentDrop = AllDeptsGridView.Rows[e.NewEditIndex].FindControl("Parent_Name_Drop") as DropDownList;
            ParentList parents = new ParentList();
            parents = DeptBusiness.GetParentListForUpdate(deptid);
            ParentDrop.DataSource = parents.ParentDict;
            ParentDrop.DataTextField = "Value";
            ParentDrop.DataValueField = "Key";
            ParentDrop.DataBind();
            ParentDrop.Items.Insert(0, new ListItem("null", "0"));
            ParentDrop.SelectedValue = parents.CurrentSelect.ToString();

            var ManagerDrop = AllDeptsGridView.Rows[e.NewEditIndex].FindControl("Manager_Name_Drop") as DropDownList;
            ManagerList managers = new ManagerList();
            managers = DeptBusiness.GetManagerList(deptid);
            ManagerDrop.DataSource = managers.ManagerDict;
            ManagerDrop.DataTextField = "Key";
            ManagerDrop.DataValueField = "Value";
            ManagerDrop.DataBind();
            ManagerDrop.Items.Insert(0, new ListItem("null", "0"));
            ManagerDrop.SelectedValue = managers.CurrentSelect.ToString();
        }

        protected void AllDeptsGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            //AllDeptsGridView.EditIndex = -1;
            //List<IShowDepts> Depts = new List<IShowDepts>();
            //Depts = DeptBusiness.GetAllDepts();
            //AllDeptsGridView.DataSource = Depts;
            //AllDeptsGridView.DataBind();
            BindGrid();
        }

        protected void AllDeptsGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Dept updatedept = new Dept();
            var ParentDrop = AllDeptsGridView.Rows[e.RowIndex].FindControl("Parent_Name_Drop") as DropDownList;
            var ManagerDrop = AllDeptsGridView.Rows[e.RowIndex].FindControl("Manager_Name_Drop") as DropDownList;
            GridViewRow Row = AllDeptsGridView.Rows[e.RowIndex];
            updatedept.DeptId = Convert.ToInt32(Row.Cells[0].Text);
            updatedept.DeptName = ((TextBox)(Row.Cells[1].Controls[0])).Text;
            if(Int32.Parse(ParentDrop.SelectedValue.ToString())!=0)
                updatedept.ParentDept= Int32.Parse(ParentDrop.SelectedValue.ToString());
            updatedept.ParentName = ParentDrop.SelectedItem.Text;
            if(Int32.Parse(ManagerDrop.SelectedValue.ToString())!=0)
                updatedept.Manager = Int32.Parse(ManagerDrop.SelectedValue.ToString());
            updatedept.ManagerName = ManagerDrop.SelectedItem.Text;

            updateresult updatedeptresult = DeptBusiness.UpdateDeptValidate(updatedept,"update");
            int a = updatedeptresult.resultid;
            if (a == -1 || a == 0 || a == 1 || a == 2)
            { //no change or name is null or parent loop or (name conflict and with same parent), then do no update
                BindGrid();
                Response.Write("<script>alert('" + updatedeptresult.resultdesc + "')</script>");
            }
            else if (a == 3)
            { //manager conflict, need confirm
                //add cookie, so that when user confirm in client side, server could get these info and update them to DB
                HttpCookie myCookie = new HttpCookie("UpdateDept");
                myCookie["Id"] = updatedept.DeptId.ToString() ;
                myCookie["Name"] = updatedept.DeptName;
                if(updatedept.ParentDept!=null)
                    myCookie["Parent"] = updatedept.ParentDept.ToString();
                myCookie["Manager"] = updatedept.Manager.ToString();
                myCookie["ConflictDeptId"] = updatedeptresult.conflictdeptid.ToString();
                myCookie.Expires = DateTime.Now.AddDays(1d);
                Response.Cookies.Add(myCookie);
                //rebind gridview, so that when user click cancel, it is a normal gridview show.
                BindGrid();
                //popup up confirm dialog, if user click yes, redirect to admindept.aspx?ifu=confirm
                string desc = updatedeptresult.resultdesc;
                string StrScript;
                string URL = "admindept.aspx?ifu=confirm";
                StrScript = ("<script language=javascript>");
                StrScript += "var retValue=window.confirm('" + desc + "');" + "if(retValue){window.location='" + URL + "';}";
                StrScript += ("</script>");
                System.Web.HttpContext.Current.Response.Write(StrScript);

            }
            else if (a == 4)
            {   //call updatedept mothed to update db (0 means without manager conflict)
                updateresult updateresult = DeptBusiness.UpdateDept(updatedept,0);
                BindGrid();
                Response.Write("<script>alert('" + updateresult.resultdesc + "')</script>");
            }

        }

        private void BindGrid()
        {
            AllDeptsGridView.EditIndex = -1;
            List<IShowDepts> Depts = new List<IShowDepts>();
            Depts = DeptBusiness.GetAllDepts();
            AllDeptsGridView.DataSource = Depts;
            AllDeptsGridView.DataBind();
        }

        protected void AllDeptsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridViewRow Row = AllDeptsGridView.Rows[e.RowIndex];
            int deptid = Convert.ToInt32(Row.Cells[0].Text);
            deldeptresult delresult = new deldeptresult();
            delresult = DeptBusiness.deleteverify(deptid);
            if (delresult.id == 1 || delresult.id == 2)
            {  //could not delete
                BindGrid();
                Response.Write("<script>alert('" + delresult.desc + "')</script>");
            }
            if(delresult.id == 3)
            {   //could not delete
                BindGrid();
                string desc = "The dept could not be deleted for unknow reason!";
                Response.Write("<script>alert('" + desc + "')</script>");
            }
            else if (delresult.id == 0)
            { //delete this dept
                deldeptresult result = new deldeptresult();
                result = DeptBusiness.DeleteDept(deptid);
                BindGrid();
                Response.Write("<script>alert('" + result.desc + "')</script>");
            }
         
        }

        protected void AllDeptsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow & e.Row.RowState != (DataControlRowState.Edit|DataControlRowState.Alternate) & e.Row.RowState != DataControlRowState.Edit)
            {
                LinkButton delete = e.Row.Cells[6].Controls[0] as LinkButton;
                delete.OnClientClick = "return confirm('are you sure to delete this dept?')";
            }
        }

        protected void TrigerAdd_Click(object sender, EventArgs e)
        {
            AddDeptPanel.Visible = true;
            TrigerAdd.Visible = false;

            ParentList parents = new ParentList();
            parents = DeptBusiness.GetParentListForAdd();
            ParentDropForAdd.DataSource = parents.ParentDict;
            ParentDropForAdd.DataTextField = "Value";
            ParentDropForAdd.DataValueField = "Key";
            ParentDropForAdd.DataBind();
            ParentDropForAdd.Items.Insert(0, new ListItem("null", "0"));
            ParentDropForAdd.SelectedIndex = 0;

            ManagerList managers = new ManagerList();
            managers = DeptBusiness.GetManagerList(0);
            ManagerDropForAdd.DataSource = managers.ManagerDict;
            ManagerDropForAdd.DataTextField = "Key";
            ManagerDropForAdd.DataValueField = "Value";
            ManagerDropForAdd.DataBind();
            ManagerDropForAdd.Items.Insert(0, new ListItem("null", "0"));
            ManagerDropForAdd.SelectedIndex = 0;
        }

        protected void AddDeptCancel_Click(object sender, EventArgs e)
        {
            AddDeptPanel.Visible = false;
            TrigerAdd.Visible = true;
        }

        protected void AddDeptButton_Click(object sender, EventArgs e)
        {
            Dept newdept = new Dept();
            newdept.DeptId = -1; // set a temp id for verification later. 
            newdept.DeptName = NewDeptName.Text;
            if (Int32.Parse(ParentDropForAdd.SelectedValue.ToString()) != 0)
                newdept.ParentDept = Int32.Parse(ParentDropForAdd.SelectedValue.ToString());
            newdept.ParentName = ParentDropForAdd.SelectedItem.Text;
            if (Int32.Parse(ManagerDropForAdd.SelectedValue.ToString()) != 0)
                newdept.Manager = Int32.Parse(ManagerDropForAdd.SelectedValue.ToString());
            newdept.ManagerName = ManagerDropForAdd.SelectedItem.Text;

            updateresult newdeptresult = DeptBusiness.UpdateDeptValidate(newdept, "add");
            int a = newdeptresult.resultid;
            if (a == 0  || a == 2)
            { //if name is null or (name conflict and with same parent), then do no add
                BindGrid();
                Response.Write("<script>alert('" + newdeptresult.resultdesc + "')</script>");
            }
            else if (a == 3)
            { //manager conflict, need confirm
                //add cookie, so that when user confirm in client side, server could get these info and update them to DB
                HttpCookie myCookie = new HttpCookie("NewDept");
                myCookie["Name"] = newdept.DeptName;
                if (newdept.ParentDept != null)
                    myCookie["Parent"] = newdept.ParentDept.ToString();
                myCookie["Manager"] = newdept.Manager.ToString();
                myCookie["ConflictDeptId"] = newdeptresult.conflictdeptid.ToString();
                myCookie.Expires = DateTime.Now.AddDays(1d);
                Response.Cookies.Add(myCookie);
                //rebind gridview, so that when user click cancel, it is a normal gridview show.
                BindGrid();
                //popup up confirm dialog, if user click yes, redirect to admindept.aspx?ifu=confirm
                string desc = newdeptresult.resultdesc;
                string StrScript;
                string URL = "admindept.aspx?ifa=confirm";
                StrScript = ("<script language=javascript>");
                StrScript += "var retValue=window.confirm('" + desc + "');" + "if(retValue){window.location='" + URL + "';}";
                StrScript += ("</script>");
                System.Web.HttpContext.Current.Response.Write(StrScript);

            }
            else if (a == 4)
            {   //call newdept mothed to add into db (0 means without manager conflict)
                updateresult updateresult = DeptBusiness.AddDept(newdept, 0);
                NewDeptName.Text = "";
                ManagerDropForAdd.SelectedIndex = 0;
                ParentDropForAdd.SelectedIndex = 0;
                BindGrid();
                Response.Write("<script>alert('" + updateresult.resultdesc + "')</script>");
            }

        }

        protected void AllDeptsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            AllDeptsGridView.PageIndex  = e.NewPageIndex;
            BindGrid();
        }

        protected void AllDeptsGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            List<IShowDepts> Depts = new List<IShowDepts>();
            Depts = DeptBusiness.GetAllDepts();
            DataTable dataTable = new DataTable();
            dataTable = ConvertListToDataTable(Depts);
            // with datatable2, convert first column(deptid) to int type to ensure the sorting result is correct
            DataTable dataTable2 = dataTable.Clone();
            dataTable2.Columns["DeptId"].DataType = Type.GetType("System.Int32");
            foreach (DataRow dr in dataTable.Rows)
            {
                dataTable2.ImportRow(dr);
            }

            if (dataTable2 != null)
            {
                DataView dataView = new DataView(dataTable2);
                string Expression;
                SortDirection Direction;
                GridViewSortDirection(AllDeptsGridView, e, out Direction, out Expression);
                dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(Direction);
                AllDeptsGridView.DataSource = dataView;
                AllDeptsGridView.DataBind();
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
    }
}