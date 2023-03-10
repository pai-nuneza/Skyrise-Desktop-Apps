using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommonLibrary
{
    #region loginInfoRecord

    public struct LoginInfoRecord
    {
        public string UserCode;
        public string Password;
        public string LastName;
        public string FirstName;
        public string MI;
        public bool CanViewRate;
        public bool HasMaintenanceRights;
        public int HasRights;
        public string DBNumber;
        public string CompanyCode;

        public string CentralProfileName;
        
        private SortedList MenuList;

        public LoginInfoRecord(string Code, string passWrd, string LName, string FName, string MInitial, bool ViewRate, bool hasMaintRights, SortedList menList, int rights, string DBNumber, string CompanyCode, string CentralProfile)
        {
            this.UserCode = Code;
            this.Password = passWrd;
            this.LastName = LName;
            this.FirstName = FName;
            this.MI = MInitial;
            this.CanViewRate = ViewRate;
            this.HasMaintenanceRights = hasMaintRights;
            this.MenuList = menList;
            this.HasRights = rights;
            this.DBNumber = DBNumber;
            this.CompanyCode = CompanyCode;
            this.CentralProfileName = CentralProfile;
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
        public bool CanUpdate;
        public bool CanDelete;
        public bool CanProcess;
        public bool CanApprove;
        public bool CanPrint;
        public bool CanReprint;

        public MenuRecord(string Code, bool View, bool Append, bool Update, bool Delete, bool Process, bool Approve, bool Print, bool Reprint)
        {
            this.MenuCode = Code;
            this.CanView = View;
            this.CanAppend = Append;
            this.CanUpdate = Update;
            this.CanDelete = Delete;
            this.CanProcess = Process;
            this.CanApprove = Approve;
            this.CanPrint = Print;
            this.CanReprint = Reprint;
        }
    }

    #endregion

    public class LoginInfo
    {
        public static LoginInfoRecord getViewRate()
        {
            if (hasUserLoggedIn())
                return ((LoginInfoRecord)Thread.GetData(Thread.GetNamedDataSlot(CommonConstants.Misc.LoginData)));
            else
                return new LoginInfoRecord();
        }

        public static string getUserLoginName()
        {
            if (hasUserLoggedIn())
            {
                LoginInfoRecord userRecord = (LoginInfoRecord)Thread.GetData(Thread.GetNamedDataSlot(CommonConstants.Misc.LoginData));

                if (!userRecord.MI.Trim().Equals(string.Empty))
                    userRecord.MI = userRecord.MI.Trim().Substring(0,1) + ".";

                return string.Format("{0}, {1}  {2}", userRecord.LastName, userRecord.FirstName, userRecord.MI);
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
