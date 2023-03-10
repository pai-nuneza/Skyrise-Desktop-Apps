using System;
using System.Collections.Generic;
using System.Text;

namespace TimeKeepingManager.Con
{
    public class DeviceDataObjectHelper
    {
        public class DeviceEmployeeObjects
        {
            public string Trf_EmployeeID;
            public string Trf_RFID;
            public int Trf_FingerIndex;
            public string Trf_Template;
            public string Trf_FaceData;
            public int Trf_Privilege;
            public string Trf_Password;
            public bool Trf_Enabled;
            public int Trf_Flag;
            public DateTime ludatetime;
            public string Usr_Login = DeviceGlobals._UsrLogin;
        }

        public DeviceDataObjectHelper.DeviceEmployeeObjects InitializeEmployeeObjects(DeviceDataObjectHelper.DeviceEmployeeObjects  ZkObj)
        {
            ZkObj.Trf_EmployeeID = "";
            ZkObj.Trf_RFID = "";
            ZkObj.Trf_FingerIndex = 0;
            ZkObj.Trf_Template = "";
            ZkObj.Trf_FaceData = "";
            ZkObj.Trf_Privilege = 0;
            ZkObj.Trf_Password = "";
            ZkObj.Trf_Enabled = false;
            ZkObj.Trf_Flag = 0;
            ZkObj.ludatetime = DateTime.Now;
            return ZkObj;
        }

        public class DeviceTimeLogObjects
        {
            public string Tel_IDNo;
            public string Tel_LogDate;
            public string Tel_LogTime;
            public string Tel_LogType;
            public string Tel_StationNo;
            public int Tel_IsPosted;
            public int Tel_IsUploaded;
            public DateTime ludatetime;
            public string Usr_login = DeviceGlobals._UsrLogin;
            public VerifyMode Dtr_VerifyMode;
            public string Dtr_RawDateTime;
            public Standard Dtr_Standard;

        }

        public DeviceTimeLogObjects InitializeTimeLogObjects(DeviceTimeLogObjects paramZkLogs)
        {
            paramZkLogs.Tel_IDNo = "";
            paramZkLogs.Tel_LogDate = "";
            paramZkLogs.Tel_LogTime = "";
            paramZkLogs.Tel_LogType = "I";
            paramZkLogs.Tel_IsUploaded = 0;
            paramZkLogs.ludatetime = DateTime.Now;
            paramZkLogs.Dtr_VerifyMode = VerifyMode.Unkown;
            paramZkLogs.Dtr_RawDateTime = "";
            paramZkLogs.Dtr_Standard = Standard.Yes;
            return paramZkLogs;
        }

        public enum VerifyMode : int
        { 
            Unkown = 0,
            Pin = 1,
            RFID = 2,
            Finger = 3,
            Face = 4
        }

        private class VerificationDesc
        {
            public const string Unknown = "Unknown";
            public const string Pin = "Pin Code";
            public const string RFIDCard = "RFID Card";
            public const string FingerPrint = "FingerPrint";
            public const string Face = "Face Data";
        }

        public VerifyMode GetVerficationMode(DeviceVersions.DeviceTypes DeviceType, Int32 VerificationCode)
        {
            switch (DeviceType)
            {
                case DeviceVersions.DeviceTypes.Veriface:
                    {
                        if (VerificationCode == 3)
                        {
                            return VerifyMode.Pin;
                        }
                        else if (VerificationCode == 4)
                        {
                            return VerifyMode.RFID;
                        }
                        else if (VerificationCode == 15)
                        {
                            return VerifyMode.Face;
                        }
                        else
                        {
                            return VerifyMode.Unkown;
                        }
                    }
                case DeviceVersions.DeviceTypes.TFT:
                    {
                        if (VerificationCode == 0)
                        {
                            return VerifyMode.Pin;
                        }
                        else if (VerificationCode == 2)
                        {
                            return VerifyMode.RFID;
                        }
                        else if (VerificationCode == 1)
                        {
                            return VerifyMode.Finger;
                        }
                        else
                        {
                            return VerifyMode.Unkown;
                        }
                    }
                default:
                    return VerifyMode.Unkown;
            }
        }

        public String GetVerificationDesciption(VerifyMode verifymode)
        {
            switch (verifymode)
            {
                case VerifyMode.Face:
                    {
                        return VerificationDesc.Face;
                    }
                case VerifyMode.Finger:
                    {
                        return VerificationDesc.FingerPrint;
                    }
                case VerifyMode.RFID:
                    {
                        return VerificationDesc.RFIDCard;
                    }
                case VerifyMode.Pin:
                    {
                        return VerificationDesc.Pin;
                    }
                case VerifyMode.Unkown:
                    {
                        return VerificationDesc.Unknown;
                    }
                default:
                    {
                        return VerificationDesc.Unknown;
                    }
            }
        }

        public enum Standard
        { 
            //This validates if the time log recorded is standard for DTR or other access only.
            Yes,
            No
        }

        public bool IsStandard(Standard LogStandard)
        {
            switch (LogStandard)
            { 
                case Standard.Yes:
                    return true;
                case Standard.No:
                    return false;
                default:
                    return true;
            }
        }

        public class DeviceDetails
        {
            public string DeviceIpAddress;
            public int Administrators;
            public int RegisteredUsers;
            public int RegisteredFP;
            public int RegisteredFaces;
            public int AttendanceRecord;
            public int UserCapacity;
            public int FPCapacity;
            public int AttendanceCapacity;
            public int FaceCapacity;
            public string SerialNumber;
            public int isCardSupported;
            public string Algorithm;
        }
    }

   
}
