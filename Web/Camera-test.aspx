<%@ Page Title="" Language="C#" MasterPageFile="~/Workday.Master" AutoEventWireup="true" CodeBehind="Camera-test.aspx.cs" Inherits="Workday.Web.Camera_test" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="container">

    </div>
    <p id="notifymsg">default text</p>
    <video id="video" width="320" height="240" autoplay="autoplay"></video>
    <%--<button id="snap" onclick="snapandupload();return false">拍照和上传</button>--%>
<%--    <script>
        controlbutton();
    </script>--%>
    <button ID="ClockIn" onclick="snapandupload(0);return false">ClockIn</button>
    <button ID="ClockOut"  onclick="snapandupload(1);return false">ClockOut</button>
    <canvas id="canvas" width="320" height="240" style="visibility:hidden" ></canvas>
    <img id="testimage" width="320" height="240" style="visibility:hidden" />
     <%--<asp:Image ID="capresult" Width="320" Height="240" runat="server" Visible="true" />--%>
    <%--<asp:Calendar ID="Calendar1" runat="server" OnDayRender="Calendar1_DayRender" OnSelectionChanged="Calendar1_SelectionChanged"></asp:Calendar>--%>
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
            .then(function(mediaStream) {
                var video = document.querySelector('video');
                video.srcObject = mediaStream;
                video.onloadedmetadata = function(e) {
                    video.play();
                };
            })
            .catch(function(err) { console.log(err.name + ": " + err.message); }); // always check for errors at the end.

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
        
    </script>
</asp:Content>
