using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public interface IMaintenanceForms
    {
        /*
         * Class inheriting this class would have a basic 
         * form functionality. Forms could add a new record,
         * modify a record, delete a record, query a record
         * and list a record.
         */

        CommonEnum.FormState GetFormState { get;set;}
        void NewMethod();
        void ModifyMethod();
        void CancelMethod();
        void SaveMethod();
        void DeleteMethod();
        void FormCloseMethod();
        void LockMethod();
    }
}
