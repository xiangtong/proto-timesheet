<%@ Page Title="" Language="C#" MasterPageFile="~/Workday.Master" AutoEventWireup="true" CodeBehind="Admin2.aspx.cs" Inherits="Workday.Web.Admin2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script type="text/javascript">
        function addfrom(){
            currenturl = window.location.pathname
            currenturl = currenturl.slice(1)
            tourl = "AddUser.aspx?from=" + currenturl
            window.location = tourl;
        }

        function validate(id) {
            //validate if the content of the text box is empty. If empty, retrun false, if not empty ,return true.
            var value
            if (document.getElementById(id)!=null)
                value = document.getElementById(id).value
            if (value == null || value == "") {
                return false
            }
            else { return true }
        }

        function validateForm() {
            var isValid = true
           // //console.log(isValid) //for debugging
            //at = validate("a")
            //ut = validate("u")
            //p1 = validate("pass1")
            //p2 = validate("pass2")
            if (validate("a") == false) {
                document.getElementById("emailerr").innerHTML = "email should not be null!"
                isValid = false;
            }
            if (validate("u") == false) {
                document.getElementById("nameerr").innerHTML = "username should not be null!"
                isValid = false;
            }
            if (validate("pass1") == false & validate("pass2") == false) {
                document.getElementById("passerr").innerHTML = "password should not be null!"
                isValid = false;
            }
           if(document.getElementById("pass1").value != document.getElementById("pass2").value) {
                document.getElementById("passerr").innerHTML = "passwords are not identical"
                pass1.value = ""
                pass2.value = ""
                isValid = false;
            }
            return isValid
        }

        function setCookie(c_name, value, expiredays) {
            var exdate = new Date()
            exdate.setDate(exdate.getDate() + expiredays)
            document.cookie = c_name + "=" + escape(value) +
            ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString())
        }

        function setcookieforuser() {
            var user = document.getElementById("u").value
            if (user != null & user != "")
                setCookie("username", user, 7)
        }

        function validateText(textbox) {
          
            if (textbox.id == "a" & textbox.value!="" & textbox.value!=null) {
                document.getElementById("emailerr").innerHTML=""
            }
            else if (textbox.id == "u" & textbox.value!="" &textbox.value!=null) {
                document.getElementById("nameerr").innerHTML=""
            }
            else if (textbox.id == "pass1" & textbox.value != "" & textbox.value != null) {

                if (document.getElementById("passerr").innerHTML == "password should not be null!") {
                    document.getElementById("passerr").innerHTML = ""
                }
            }
            else if (textbox.id == "pass2" & textbox.value != "" & textbox.value != null) {
                if (document.getElementById("pass1").value == document.getElementById("pass2").value) {
                    //alert("same this time")
            document.getElementById("passerr").innerHTML = ""
                }
            }
        }

        function cleardefaultpass() {
            var text = document.getElementById("pass1").value
            if (text == "The default password:111111") {
                document.getElementById("pass1").value = ""
                document.getElementById("pass2").value = ""
            }
        }
  </script>
    <style type="text/css">
        th.sortasc a {
            color:red;
            display: block;
            padding: 0 18px 0 10px;
            background: url(img/asc.gif) no-repeat right center;
        }
        th.sortdesc a {
            color:red;
            display: block;
            padding: 0 18px 0 10px;
            background: url(img/desc.gif) no-repeat right center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <asp:Panel ID="AddUserPanel" runat="server" Visible="false">
        <div>
            <asp:Label ID="Label1" runat="server" Text="" Font-Size="Large" ForeColor="Red"></asp:Label>
            <br/><br/>
        </div>
        <div class="row">
            <div class="col-md-2">Email Address</div>
            <div class="col-md-6"><input type="text" class="form-control" name="email" id="a" onblur="validateText(this)" /><span id="emailerr"></span></div>
        </div>
        <div class="row">
            <div class="col-md-2">User Name</div>
            <div class="col-md-6"><input type="text" class="form-control" name="username" id="u" onblur="validateText(this)"/><span id="nameerr"></span></div>
        </div>
        <div class="row">
            <div class="col-md-2">User Password</div>
            <div class="col-md-6"><input type="text" class="form-control" name="password" id="pass1" value="The default password:111111" onblur="validateText(this)" onfocus="cleardefaultpass()"/><span id="passerr"></span></div>
        </div>
        <div class="row">
            <div class="col-md-2">Confirm Password</div>
            <div class="col-md-6"><input type="text" class="form-control" name="password2" id="pass2" value="The default password:111111" onblur="validateText(this)" /><span id="passerr2"></span></div>
        </div>
         <div class="row">
            <div class="col-md-2">Select Dept</div>
            <div class="col-md-6">
                <asp:DropDownList ID="Dept_Drop" runat="server"></asp:DropDownList> </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <input type="submit" class="btn btn-primary" value="Add" onclick="return validateForm()" />
                &nbsp;
                <asp:Button ID="cancel" runat="server" OnClick="AddUserCancel_Click" Text="Cancel" />
            </div>
        </div>
    </asp:Panel>
      <div style="padding-left:800px">  <asp:Button ID="TrigerAdd" runat="server" Text="Add User"  OnClick="TrigerAdd_Click" />  <br/><br />   </div>
    <div id="allusershow" style="padding-left:50px">
        <asp:GridView ID="AllUserGridView"  AllowPaging="True" PageSize="10" AllowSorting="true" AutoGenerateColumns="False" runat="server" Font-Name="Verdana" 
             Font-Size="10pt" Cellpadding="15"
             HeaderStyle-BackColor="Gray" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#dddddd"
             GridLines="none" OnPageIndexChanging="PageIndexChanging" OnRowDataBound=" AllUserGridView_RowDataBound"
             DataKeyNames="DeptName,DeptId" OnRowCancelingEdit="AllUserGridView_CancelingEdit" OnRowEditing="AllUserGridView_RowEditing" OnRowUpdating="AllUserGridView_RowUpdating" OnSorting="AllUserGridView_Sorting"> 
            <HeaderStyle BackColor="Gray" ForeColor="White"></HeaderStyle>
        <%--  <SortedAscendingHeaderStyle CssClass="sortasc" />  <SortedDescendingHeaderStyle CssClass="sortdesc"  />--%>
            <AlternatingRowStyle BackColor="#DDDDDD"></AlternatingRowStyle>
            <Columns>
                <asp:BoundField HeaderText="User_Id" DataField="UserID" ReadOnly="true"  SortExpression="UserId"/>
                <asp:BoundField HeaderText="User_Email" DataField="Email" ReadOnly="true" />
                <asp:BoundField HeaderText="User_Name" DataField="UserName" ReadOnly="true" SortExpression="UserName" />
                <asp:BoundField HeaderText="User_Status" DataField="StatusString" ReadOnly="true"/>
                <asp:TemplateField HeaderText="Dept_Name" SortExpression="DeptName">
                     <ItemTemplate>
                         <asp:Label ID="Dept_Label" runat="server"></asp:Label>
                     </ItemTemplate>
                     <EditItemTemplate>
                         <asp:DropDownList ID="Dept_Drop" runat="server"></asp:DropDownList>
                     </EditItemTemplate>
                 </asp:TemplateField>
                <asp:BoundField HeaderText="Is_Manager" DataField="IsManager" ReadOnly="true" SortExpression="IsManager" />
                <asp:BoundField HeaderText="Create_Date" DataField="CreateDate" ReadOnly="true"/>
                <asp:TemplateField HeaderText="Dis/En" >
                    <ItemTemplate>
                        <asp:HyperLink ID="HyperLink1" runat="server">HyperLink</asp:HyperLink>
                      <%--   <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# "admin2.aspx?userid=" + Eval("UserID")+"&currentstatus="+Eval("UserStatus") %>'
                            Text='<%# LinkText(Convert.ToInt32(Eval("UserStatus"))) %>'
                            Visible='<%# Convert.ToString(Eval("UserName")) != Session["CurrentUser"].ToString() ? true : false%>'>HyperLink</asp:HyperLink>--%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField HeaderText="Edit" ShowEditButton="true" />

         </Columns>
        </asp:GridView>
        <br/>
    </div>
        <div style="padding-left:600px">
        <br/>
        <input id="Button12" type="button" value="AddUser" onclick="addfrom()"/>
    </div>
  <%--  <div>
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="Button1" runat="server" Text="md5-encode" OnClick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" Text="md5-decode" OnClick="Button2_Click" />
        <br/>
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    </div>--%>
</asp:Content>
