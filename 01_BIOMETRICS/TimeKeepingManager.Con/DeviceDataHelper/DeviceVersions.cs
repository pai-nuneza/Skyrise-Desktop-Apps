using System;
using System.Collections.Generic;
using System.Text;

namespace TimeKeepingManager.Con
{
    public class DeviceVersions
    {
        public static string Veriface = "VFC - Veriface [Version 3.0]";
        public static string VFCV = "3.0";
        public static string Colored = "TFT - Colored [Version 2.0]";
        public static string TFTV = "2.0";
        public static string BlackandWhite = "B&W - Black & White [Version 1.0]";
        public static string BWV = "1.0";
        public static string Unknown = "Unknown";

        public static DeviceTypes DeviceType;

        public enum DeviceTypes
        { 
            TFT,
            Veriface,
            BlackandWhite
        }

        public static DeviceTypes GetDeviceType(string Version)
        {
            if (Version.Trim() == BWV)
            {
                return DeviceTypes.BlackandWhite;
            }
            else if (Version.Trim() == TFTV)
            {
                return DeviceTypes.TFT;
            }
            else if (Version.Trim() == VFCV)
            {
                return DeviceTypes.Veriface;
            }
            else
            {
                return DeviceTypes.TFT;
            }
        }

        public static String GetDeviceVersionCode(string VersionDesc)
        {
            if (VersionDesc.Trim().Equals(Veriface, StringComparison.CurrentCultureIgnoreCase))
                return VFCV;
            else if (VersionDesc.Trim().Equals(Colored, StringComparison.CurrentCultureIgnoreCase))
                return TFTV;
            else if (VersionDesc.Trim().Equals(BlackandWhite, StringComparison.CurrentCultureIgnoreCase))
                return BWV;
            else
                return TFTV;

        }

        public static String GetDeviceVersionDesc(DeviceTypes devicetype)
        {
            switch (devicetype)
            {
                case DeviceTypes.BlackandWhite:
                    {
                        return "Standalone B&W";
                    }
                case DeviceTypes.TFT:
                    {
                        return "Standalone Colored";
                    }
                case DeviceTypes.Veriface:
                    {
                        return "Standalone Face";
                    }
                default:
                    {
                        return "Unknown";
                    }

            }
        }

        public static bool IsBlackAndWhiteType
        {
            get
            {
                if (DeviceType == DeviceTypes.BlackandWhite)
                    return true;
                else
                    return false;
            }
        }

        public static bool IsTFT
        {
            get
            {
                if (DeviceType == DeviceTypes.TFT)
                    return true;
                else
                    return false;
            }
        }

        public static bool IsVeriface
        {
            get
            {
                if (DeviceType == DeviceTypes.Veriface)
                    return true;
                else
                    return false;
            }
        }
    }
}
