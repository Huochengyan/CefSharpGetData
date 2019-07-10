using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browserform.common
{
    public class ConvertInfo
    {
        /// <summary>
        /// 转换类
        /// </summary>
        /// <param name="List_MemberList"></param>
        /// <returns></returns>
        public static List<MyDB.MemberListItem> CovnertMemberListItem(List<common.MemberList> List_MemberList)
        {
            List<MyDB.MemberListItem> list = new List<MyDB.MemberListItem>();
            for (int i = 0; i < List_MemberList.Count; i++)
            {
                MyDB.MemberListItem info = new MyDB.MemberListItem();
                info.UserName = List_MemberList[i].UserName.ToString();
                info.NickName= List_MemberList[i].NickName.ToString();
                list.Add(info);
            }
            return list;
        }
    }
}
