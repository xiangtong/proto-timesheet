<%@ Page Title="" Language="C#" MasterPageFile="~/Workday.Master" AutoEventWireup="true" CodeBehind="TimeSheet.aspx.cs" Inherits="Workday.Web.TimeSheet" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">         </asp:ScriptManager> 
 <script type="text/javascript">     
     function controlbutton() {
         PageMethods.ButtonControl(0,sufunc,erfunc)
     }
     function GetNotifyMsg() {
         PageMethods.GetNotify(0, sucfunc, errfunc)
     }
     //disable or enable button 
     function sufunc(i) {
         if(i==0){
             document.getElementById("ClockIn").disabled=false;
             document.getElementById("ClockOut").disabled=true;
         }
         else if(i==1){
             document.getElementById("ClockIn").disabled=true;
             document.getElementById("ClockOut").disabled=false;
         }
         else if(i==2){
             document.getElementById("ClockIn").disabled=true;
             document.getElementById("ClockOut").disabled=true;
         }
         GetNotifyMsg()
     }

     function erfunc(err) {
         alert("Error to control button" );
     }

     function sucfunc(time) {
         document.getElementById("notifymsg").innerHTML = time;
     }

     function errfunc() {
         document.getElementById("notifymsg").innerHTML = "unknown error message!"
     }

<%--     function popupsimg() {
         obj simgobj= document.getElementById('<%=Simg.ClientID%>');
        }

     function popupeimg() {
         obj eimgobj = document.getElementById("Eimg");
        }--%>
         </script>
<div id="container">

    </div>
    <p id="notifymsg">default text</p>
    <video id="video" width="320" height="240" autoplay="autoplay"></video>
    <%--<button id="snap" onclick="snapandupload();return false">拍照和上传</button>--%>
    <script>
        controlbutton();
    </script>
    <button ID="ClockIn" onclick="snapandupload(0);return false">ClockIn</button>
    <button ID="ClockOut"  onclick="snapandupload(1);return false">ClockOut</button>
    <canvas id="canvas" width="320" height="240" style="visibility:hidden" ></canvas>
    <img id="testimage" width="320" height="240" style="visibility:hidden" />
     <%--<asp:Image ID="capresult" Width="320" Height="240" runat="server" Visible="true" />--%>
    <asp:Calendar ID="Calendar1" runat="server" OnDayRender="Calendar1_DayRender" OnSelectionChanged="Calendar1_SelectionChanged"></asp:Calendar>
    <br />
    
    <script type="text/javascript">
        
        window.addEventListener("DOMContentLoaded", function () {

            // 获取基本的元素,设置.
            var canvas = document.getElementById("canvas");
            var context = canvas.getContext("2d");
            // hide the canvas
            //context.style.display = "none";
            var image = new Image();
            var video = document.getElementById("video");
            var videoObj = { "video": true };
            var errBack = function (error) {
                console.log("Video capture error: ", error.code);
            };

            // Prefer camera resolution nearest to 1280x720.
            var constraints = { audio: false, video: { width: 1280, height: 720 } };

            navigator.mediaDevices.getUserMedia(constraints)
            .then(function (mediaStream) {
                var video = document.querySelector('video');
                video.srcObject = mediaStream;
                video.onloadedmetadata = function (e) {
                    video.play();
                };
            })
            .catch(function (err) { console.log(err.name + ": " + err.message); }); // always check for errors at the end.

            //过时的方法，新的浏览器已经不支持  https://developer.mozilla.org/en-US/docs/Web/API/MediaDevices/getUserMedia#Frame_rate
            //// 获取摄像头的方式
            //if (navigator.getUserMedia) { // 标准
            //    navigator.getUserMedia(videoObj, function (stream) {
            //        video.src = stream;
            //        video.play();
            //    }, errBack);
            //} else if (navigator.webkitGetUserMedia) { // WebKit浏览器
            //    navigator.webkitGetUserMedia(videoObj, function (stream) {
            //        video.src = window.URL.createObjectURL(stream);
            //        video.play();
            //    }, errBack);
            //}
            //else if (navigator.mozGetUserMedia) { // Firefox浏览器
            //    navigator.mozGetUserMedia(videoObj, function (stream) {
            //        video.src = window.URL.createObjectURL(stream);
            //        video.play();
            //    }, errBack);
            //}

            // Trigger photo take and upload 
            //document.getElementById("snap").addEventListener("click", function () {
            //    context.drawImage(video, 0, 0, 640, 480);
            //});
        }, false);

        function snapandupload(type) {
            if (video.readyState == 4) {
                var canvas = document.getElementById("canvas");
                var context = canvas.getContext("2d");
                context.drawImage(video, 0, 0, 320, 240);
                var image = new Image();
                image.src = canvas.toDataURL("image/png",0.1);
                var base64 = image.src;
                //alert(base64)
                //upload(base64);
                PageMethods.Upload(base64,type,sfunc,efunc)
            }
        }
            //if upload success, show the uploaded image (get binary from DB)
        function sfunc(imageurl) {
            document.getElementById("testimage").style.visibility = "visible"
            document.getElementById("testimage").src = imageurl
            controlbutton()
            //alert("stop")
        }

        function efunc(err) {
            alert("Error:" + err._message);
        }
        
        //not a formal mothed, just for debug.
        //function upload(base64){
        //        var xmlhttp;
        //        if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        //            xmlhttp = new XMLHttpRequest();
        //        }
        //        else {// code for IE6, IE5
        //            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        //        }
        //        xmlhttp.onreadystatechange = function () {
        //            if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
        //                //var image1 = document.getElementById("capresult")
        //                //image1.src = xmlhttp.responseText;
        //                alert("success!")
        //            }
        //        }
        //        xmlhttp.open("POST", "TimeSheet.aspx", true);
        //        xmlhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        //        xmlhttp.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        //        postcontent = "image=" + base64;
        //        xmlhttp.send(postcontent);
        //    }

    </script>
    <asp:GridView ID="TimeSheetGridView" AutoGenerateColumns="False" runat="server" OnRowDataBound="TimeSheetGridView_RowDataBound">
       <Columns>
            <asp:BoundField HeaderText="Date_" DataField="Date" ReadOnly="true" />
            <asp:BoundField HeaderText="Start_Time" DataField="StartTime" ReadOnly="true" />
            <asp:BoundField HeaderText="End_Time" DataField="EndTime" ReadOnly="true" />
            <asp:BoundField HeaderText="WorkTime(h)" DataField="WorkDuration" ReadOnly="true"/>
            <asp:TemplateField HeaderText="Review_result" >
                <ItemTemplate>
                      <asp:Image ID="RRimg" runat="server" Height="15px" Width="15px" /> <br />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Refuse_Reason" DataField="RefuseReason" ReadOnly="true"/>
            <asp:BoundField HeaderText="Approved(h)" DataField="ApprovedDuration" ReadOnly="true"/>
        </Columns>
    </asp:GridView>

    <br />
    <asp:DropDownList ID="SelectUserDrop" runat="server" OnSelectedIndexChanged="SelectUserDrop_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true">
    </asp:DropDownList>

    <br />
    <asp:GridView ID="ManagerTimeSheetGridView" AutoGenerateColumns="False" runat="server" 
        DataKeyNames="TimeSheetId" OnRowDataBound="ManagerTimeSheetGridView_RowDataBound" OnRowCommand="Approve_Refuse_Time" >
       <Columns>
            <asp:BoundField HeaderText="UserName" DataField="UserName" ReadOnly="true" />
            <asp:BoundField HeaderText="Date_" DataField="Date" ReadOnly="true" />
            <asp:BoundField HeaderText="Start_Time" DataField="StartTime" ReadOnly="true" />
            <asp:BoundField HeaderText="Start_IP" DataField="StartIp" ReadOnly="true" />
            <asp:TemplateField HeaderText="Start_Img" >
                <ItemTemplate>
                      <asp:Image ID="Simg" runat="server" Height="100px" Width="100px" /> <br />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="End_Time" DataField="EndTime" ReadOnly="true" />
            <asp:BoundField HeaderText="End_IP" DataField="EndIp" ReadOnly="true" />
            <asp:TemplateField HeaderText="End_Img" >
                <ItemTemplate>
                      <asp:Image ID="Eimg" runat="server" Height="100px" Width="100px" ClientIDMode="Static"/> <br />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="WorkTime(h)" DataField="WorkDuration" ReadOnly="true"/>
            <asp:TemplateField HeaderText="Review_result" >
                <ItemTemplate>
                      <asp:Image ID="RRimg0" runat="server" Height="15px" Width="15px" /> <br />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Refuse_Reason" DataField="RefuseReason" ReadOnly="true"/>
            <asp:BoundField HeaderText="Approved(h)" DataField="ApprovedDuration" ReadOnly="true"/>
            <asp:BoundField HeaderText="ReviewedBy" DataField="ReviewedBy" ReadOnly="true"/>
            <asp:TemplateField HeaderText="Approve">
                <ItemTemplate>
                    <asp:Button ID="App_Time"  Runat="Server" CausesValidation="false" Text="Approve"
                    CommandName="Approve_Time"
                    CommandArgument='<%#((GridViewRow) Container).RowIndex %>'/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Refuse">
                <ItemTemplate>
                    <asp:Button ID="Ref_Time"  Runat="Server" CausesValidation="false" Text="Refuse"
                    CommandName="Refuse_Time"
                    CommandArgument='<%#((GridViewRow) Container).RowIndex %>'/>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <br />

</asp:Content>
