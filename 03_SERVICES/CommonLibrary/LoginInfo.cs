using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommonPostingLibrary
{
    #region loginInfoRecord
    public struct LoginInfoRecord
    {
        public string UserCode;
        public string UserNumber;
        public string LastName;
        public string FirstName;
        public string MI;
        public bool HasSupervisoryRights;
        public bool HasMaintenanceRights;

        private SortedList MenuList;

        public LoginInfoRecord(string userCode, string userNumber, string lastName, string firstName, string middleInitial, bool hasSupervisoryRights, bool hasMaintenanceRights, SortedList menuList)
        {
            this.UserCode = userCode;
            this.UserNumber = userNumber;
            this.LastName = lastName;
            this.FirstName = firstName;
            this.MI = middleInitial;
            this.HasSupervisoryRights = hasSupervisoryRights;
            this.HasMaintenanceRights = hasMaintenanceRights;
            this.MenuList = menuList;
        }

        public MenuRecord getMenu(string menuCode)
        {
            MenuRecord menu = new MenuRecord();

            if (menuCode != null && !menuCode.Trim().Equals(string.Empty))
            {
                menuCode = menuCode.ToUpper();
                if (MenuList.ContainsKey(menuCode))
                    menu = (MenuRecord)MenuList.GetByIndex(MenuList.IndexOfKey(menuCode));
                else
                    menu.MenuCode = "";
            }
            return menu;
        }

        public SortedList getMenuList
        {
            get
            {
                return MenuList;
            }
        }
    }

    #endregion

    #region menuRecord

    public struct MenuRecord
    {
        public string MenuCode;
        public bool CanView;
        public bool CanAppend;
        public bool CanEdit;
        public bool CanDelete;
        public bool CanPrint;
        public bool CanReprint;
        public bool CanCheck;
        public bool CanApprove;
        public bool CanHold;
        public bool CanUnHold;

        public MenuRecord(string menuCode, bool canView, bool canAppend, bool canEdit, bool canDelete, bool canPrint, bool canReprint, bool canCheck, bool canApprove, bool canHold, bool canUnHold)
        {
            this.MenuCode = menuCode;
            this.CanView = canView;
            this.CanAppend = canAppend;
            this.CanEdit = canEdit;
            this.CanDelete = canDelete;
            this.CanPrint = canPrint;
            this.CanReprint = canReprint;
            this.CanCheck = canCheck;
            this.CanApprove = canApprove;
            this.CanHold = canHold;
            this.CanUnHold = canUnHold;
        }
    }

    #endregion

    public class LoginInfo
    {
        public static string getUserLoginName()
        {
            if (hasUserLoggedIn())
            {
                LoginInfoRecord userRecord = (LoginInfoRecord)Thread.GetData(Thread.GetNamedDataSlot(CommonConstants.Misc.LoginData));

                if (!userRecord.MI.Equals(string.Empty))
                    userRecord.MI = userRecord.MI + ".";

                return string.Format("{0}, {1} {2}", userRecord.LastName, userRecord.FirstName, userRecord.MI);
            }
            else
                return string.Empty;
        }

        public static LoginInfoRecord getUser()
        {
            if (hasUserLoggedIn())
                return ((LoginInfoRecord)Thread.GetData(Thread.GetNamedDataSlot(CommonConstants.Misc.LoginData)));
            else
                return new LoginInfoRecord();
        }

        private static bool hasUserLoggedIn()
        {
            return (Thread.GetData(Thread.GetNamedDataSlot(CommonConstants.Misc.LoginData)) != null);
        }

        public static MenuRecord getUserMenuRecord(string MenuName)
        {
            if (hasUserLoggedIn())
                return ((LoginInfoRecord)Thread.GetData(Thread.GetNamedDataSlot(CommonConstants.Misc.LoginData))).getMenu(MenuName);
            else
                return new MenuRecord();
        }
    }
}
