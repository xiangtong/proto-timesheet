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

    }
}
