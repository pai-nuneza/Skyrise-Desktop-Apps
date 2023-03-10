using System;
using System.Collections.Generic;
using System.Text;

namespace CommonPostingLibrary
{
    public interface IMaintenanceEditOnly
    {
        /*
                                                                                         * Class inheriting this class would only be
                                                                                         * editable forms. 
                                                                                         */

        CommonEnum.FormState GetFormState { get; set; }
        void ModifyMethod();
        void CancelMethod();
        void SaveMethod();
        void FormCloseMethod();
        void LockMethod();
    }
}
