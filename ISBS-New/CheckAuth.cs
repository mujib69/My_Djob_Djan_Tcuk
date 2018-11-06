using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISBS_New
{
    class CheckAuth
    {
        private string userGroupAuthID;
        private string groupCode;
        private string authName;
        private string menuID;

        public CheckAuth(string userGroupAuthID, string groupCode, string authName, string menuID)
        {
            this.userGroupAuthID = userGroupAuthID;
            this.groupCode = groupCode;
            this.authName = authName;
            this.menuID = menuID;
        }

        public string UserGroupAuthID
        {
            get { return userGroupAuthID; }
            set { userGroupAuthID = value; }
        }

        public string GroupCode
        {
            get { return groupCode; }
            set { groupCode = value; }
        }

        public string AuthName
        {
            get { return authName; }
            set { authName = value; }
        }

        public string MenuID
        {
            get { return menuID; }
            set { menuID = value; }
        }

    }
}
