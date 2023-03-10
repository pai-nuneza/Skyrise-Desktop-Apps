using System;

namespace CommonPostingLibrary
{
    public interface IAbstractForm
    {
        CommonEnum.FormState GetFormState { get; set; }
        void NewMethod();
        void ModifyMethod();
        void CancelMethod();
        void SaveMethod();
        void DeleteMethod();
        void FormCloseMethod();
        void EnterQueryMethod();
        void ExecuteQueryMethod();
        void CancelExcuteMethod();
        void LockMethod();
    }
}
