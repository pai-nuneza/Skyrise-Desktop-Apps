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
        public string UserPosition;
        public int HasRights;
        public string CompanyCode;
        
        private SortedList MenuList;

        public LoginInfoRecord(string Code, string passWrd, string LName, string FName, string MInitial, bool ViewRate, bool hasMaintRights, string userPos, SortedList menList, int rights, string companyCode)
        {
            this.UserCode = Code;
            this.Password = passWrd;
            this.LastName = LName;
            this.FirstName = FName;
            this.MI = MInitial;
            this.CanViewRate = ViewRate;
            this.HasMaintenanceRights = hasMaintRights;
            this.UserPosition = userPos;
            this.MenuList = menList;
            this.HasRights = rights;
            this.CompanyCode = companyCode;
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
        public bool CanRetrieve;
        public bool CanAdd;
        public bool CanEdit;
        public bool CanDelete;
        public bool CanGenerate;
        public bool CanCheck;
        public bool CanApprove;
        public bool CanPrintPreview;
        public bool CanPrint;
        public bool CanReprint;

        public MenuRecord(string Code, bool Retrieve, bool Add, bool Edit, bool Delete, bool Generate, bool Check, bool Approve, bool PrintPreview, bool Print, bool Reprint)
        {
            this.MenuCode = Code;
            this.CanRetrieve = Retrieve;
            this.CanAdd = Add;
            this.CanEdit = Edit;
            this.CanDelete = Delete;
            this.CanGenerate = Generate;
            this.CanCheck = Check;
            this.CanApprove = Approve;
            this.CanPrintPreview = PrintPreview;
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
