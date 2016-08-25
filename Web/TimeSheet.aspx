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
         </script>
<div id="container">
 <%--           <asp:Calendar ID="Calendar1" runat="server"></asp:Calendar>--%>
    </div>
    <p id="notifymsg">default text</p>
    <video id="video" width="320" height="240" autoplay="autoplay"></video>
    <button id="snap" onclick="snapandupload();return false">拍照和上传</button>
    <script>
        controlbutton();
    </script>
    <button ID="ClockIn" onclick="snapandupload(0);return false">ClockIn</button>
    <button ID="ClockOut"  onclick="snapandupload(1);return false">ClockOut</button>
    <canvas id="canvas" width="320" height="240" style="visibility:hidden" ></canvas>
    <img id="testimage" width="320" height="240" style="visibility:hidden" />
     <%--<asp:Image ID="capresult" Width="320" Height="240" runat="server" Visible="true" />--%>

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

            // 获取摄像头的方式
            if (navigator.getUserMedia) { // 标准
                navigator.getUserMedia(videoObj, function (stream) {
                    video.src = stream;
                    video.play();
                }, errBack);
            } else if (navigator.webkitGetUserMedia) { // WebKit浏览器
                navigator.webkitGetUserMedia(videoObj, function (stream) {
                    video.src = window.URL.createObjectURL(stream);
                    video.play();
                }, errBack);
            }
            else if (navigator.mozGetUserMedia) { // Firefox浏览器
                navigator.mozGetUserMedia(videoObj, function (stream) {
                    video.src = window.URL.createObjectURL(stream);
                    video.play();
                }, errBack);
            }

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

</asp:Content>
