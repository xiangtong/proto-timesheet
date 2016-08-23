using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Workday.Common;
using Workday.DataAccess;

namespace Workday.Business
{
    public class UserBusiness
    {
        public static User GetUserById(int userId)
        {
            return UserDataAccess.GetUserById(userId);
        }

        public static User GetUserByName(string name)
        {
            return UserDataAccess.GetUserByName(name);
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static User AddUser(User user)
        {
            return UserDataAccess.AddUser(user);
        }

        /// <summary>
        /// 验证用户，用于登录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static User Verify(string name, string password, out string errorMsg,out string isAdmin,out bool isFLogin)
        {
            errorMsg = null;
            isAdmin = null;
            isFLogin = false;
            password = User.GenerateHash(password);
            User user = GetUserByName(name);

            if (user == null)
            {
                errorMsg = "invalid account";
            }
            else if (user.Password != password)
            {
                user = null;
                errorMsg = "password error";
            }
            else if (user.Status != UserStatus.Normal)
            {
                user = null;
                errorMsg = "this user has been disabled!";
            }
            else if (user.LoginDate == null)
            {
                isFLogin = true;
            }

            if (user != null)
            {
                if (user.IsAdmin == UserIsAdmin.IsAdmin)
                {
                    isAdmin = user.IsAdmin.ToString();
                }
            }

            return user;
        }

        //this is the old mothed, only used by admin.aspx, should be deprecated. the new mothed is GetAllUser2
        public static AllUser GetAllUsers(AllUser users)
        {
            return UserDataAccess.GetAllUser(users);
        }

        public static List<IShowUsers> GetAllUsers2()
        {
            List<IShowUsers> Users = UserDataAccess.GetAllUser2();
            if (Users != null)
            {
                foreach (IShowUsers item in Users)
                {
                    if (item.Status == Common.UserStatus.Normal)
                        item.StatusString = "Normal";
                    else if (item.Status == Common.UserStatus.Forbidden)
                        item.StatusString = "Disabled";
                    if (item.UserId == item.Manager)
                        item.IsManager = "Manager";
                    item.Disenlinkurl = "admin2.aspx?userid=" + item.UserId + "&currentstatus=" + Convert.ToString((int)item.Status);
                }
            }
            return Users;
        }

        public static bool ChangeUserStatus(int userid,int currentstatus)
        {
            return UserDataAccess.ChangeUserStatus(userid, currentstatus);
        }

        public static bool ChangeDefaultPassword(User user)
        {
            return UserDataAccess.ChangeDefaultPassword(user);
        }

        public static Dictionary<int, string> GetSecretQuestion()
        {
            Dictionary<int, string> questionlist = new Dictionary<int, string>();
            SecretQuestion temp = new SecretQuestion();
            temp=UserDataAccess.GetSecretQuestion();
            questionlist = temp.QuestionList;
            return questionlist;
        }

        public static bool ChangeUserDept(User user)
        {
            return UserDataAccess.ChangeUserDept(user);
        }

        public static validatresult ChangeDeptValidate(User user)
        {
            validatresult result = new validatresult();
            List<IShowUsers> users = new List<IShowUsers>();
            users = UserDataAccess.GetAllUser2();
            foreach (User u in users)
            {
                if (u.UserId == user.UserId)
                {
                    if (u.DeptId == user.DeptId)
                    {
                        // no change,return -1
                        result.resultid = -1;
                        result.resultdesc = "No Change";
                        return result;
                    }
                }
            }
            // validate successfully. you could update 
            result.resultid = 0;
            result.resultdesc = "you could change!";
            return result;

        }
    }

}
