<%@ Page Title="" Language="C#" MasterPageFile="~/Workday.Master" AutoEventWireup="true" CodeBehind="AddUser.aspx.cs" Inherits="Workday.Web.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <script type="text/javascript">

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

           // if (isValid == true) {
           //         setcookieforuser()
           //     }
            // alert(isValid) // for debugging

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="container">
        <div>
            <asp:Label ID="Label1" runat="server" Text="" Font-Size="Large" ForeColor="Red"></asp:Label>
            <br/><br/>
        </div>
        <div class="row">
            <div class="col-md-2">Email Address</div>
            <div class="col-md-6"><input type="text" class="form-control" name="email" id="a" onblur="validateText(this)" /></div>
            <div class="col-md-4" ><span id="emailerr"></span></div>
        </div>
        <div class="row">
            <div class="col-md-2">User Name</div>
            <div class="col-md-6"><input type="text" class="form-control" name="username" id="u" onblur="validateText(this)"/></div>
            <div class="col-md-4" ><span id="nameerr"></span></div>
        </div>
        <div class="row">
            <div class="col-md-2">User Password</div>
            <div class="col-md-6"><input type="text" class="form-control" name="password" id="pass1" value="The default password:111111" onblur="validateText(this)" onfocus="cleardefaultpass()"/></div>
            <div class="col-md-4" ><span id="passerr"></span></div>
        </div>
        <div class="row">
            <div class="col-md-2">Confirm Password</div>
            <div class="col-md-6"><input type="text" class="form-control" name="password2" id="pass2" value="The default password:111111" onblur="validateText(this)" /></div>
            <div class="col-md-4" ><span id="passerr2"></span></div>
        </div>
         <div class="row">
            <div class="col-md-2">Select Dept</div>
            <div class="col-md-6">
                <asp:DropDownList ID="Dept_Drop" runat="server"></asp:DropDownList> </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <input type="submit" class="btn btn-primary" value="Add" onclick="return validateForm()" />
                <input type="button" class="btn btn-default" value="Cancel" onclick="history.back();" />
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function check() {
            return true;
        }
    </script>
</asp:Content>
