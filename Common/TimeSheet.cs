using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workday.Common
{
    public class TimeSheet
    {
        public Guid TimeSheetId { get; set; }

        public int UserId { get; set; }

        public string Date { get; set; }

        public string StartTime { get; set; }

        public string StartIp { get; set; }

        public byte[] StartImage { get; set; }

        public string EndTime { get; set; }

        public string EndIp { get; set; }

        public byte[] EndImage { get; set; }

        public string WorkContent { get; set; }

    }

    public class TsForView : TimeSheet
    {
        public string UserName { get; set; }

        public string WorkDuration { get; set; }

        public TsReviewResult ReviewResult { get; set; }

        public string TsRRImgUrl { get; set; }  //approval img or refuse img or not process img

        public float ApprovedDuration { get; set; }

        public string RefuseReason { get; set; }
    }

    public enum TsReviewResult
    {
        NoProcess,
        Approved,
        Refused,
    }

}
