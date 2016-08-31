using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Workday.Common;
using Workday.DataAccess;

namespace Workday.Business
{
    public class TimeSheetBusiness
    {
        public static bool AddTimeSheet(TimeSheet thistime, int type)
        {
            return TimeSheetDataAccess.AddTimeSheet(thistime,type);
        }

        public static byte[] GetStartImageByID(TimeSheet thistime,int type)
        {
            return TimeSheetDataAccess.GetStartImageByID(thistime,type);
        }

        public static TimeSheet VerifyTime(TimeSheet thistime)
        {
            return TimeSheetDataAccess.VerifyTime(thistime);
        }

        public static List<TsForView> GetTimeByUser(int userid, DateTime begindate, DateTime enddate, int type, int deptid = 0)
        {
            List<TsForView> TimeViews = new List<TsForView>();
            TimeViews = TimeSheetDataAccess.GetTimeByUser(userid, begindate, enddate, type, deptid);
            if (TimeViews != null)
            {
                foreach (TsForView item in TimeViews)
                {
                    if (item.ReviewResult == TsReviewResult.NoProcess)
                        item.TsRRImgUrl = "~/img/noprocess.png";
                    else if (item.ReviewResult == TsReviewResult.Approved)
                        item.TsRRImgUrl = "~/img/approve.jpg";
                    else if (item.ReviewResult == TsReviewResult.Refused)
                        item.TsRRImgUrl = "~/img/refuse.png";

                }
            }
            return TimeViews;
        }
    }
}
