using System;
using System.Collections.Generic;
using System.Text;

namespace TimeKeepingManager.Con
{
    public class MSG
    {
        public static string Success = "Successful";
        public static string Failed = "Failed";
        public static string Err = "Error";
        public static string Info = "Info";

        public static string DeviceDisconnected = "Unable to connect to device.";
        public static string DBDisconnected = "Unable to connect to Database";
        public static string DeviceErrConnetion = "Reading data from terminal failed.";
        public static string DeviceDataUploadSuccess = "Uploading Database to Device was successful.\nTotal uploaded data : {0}.";
        public static string DeviceIndividualDataUploadSuccess = "Uploading Employee to Device was successful.\nTotal uploaded data : {0}.";
        public static string DeviceDataUploadSuccess2 = "Loading Database Finger Prints to Device was successful.";
        public static string DeviceLogsUploadSuccess = "Uploading device logs to Database was successful. \nTotal uploaded logs : {0} ";
        public static string DeviceLogAlreadyDownloaded = "Device logs are already downloaded.";
        public static string DeviceNoLogs = "There is no existing logs on the device or with in the specified date.";
        public static string DeviceDataDownloadSuccess = "Downloading Device data was successful.\nTotal downloaded data : {0}";
        public static string DeviceDataDownloadSuccess2 = "Downloading Device data was successful.";
        public static string DefaultSuccess = "Downloading Device data was successful.";
        public static string DeviceNoData = "There is no existing employee data on the device.";
        public static string DeviceCannotRefresh = "Unable to refresh data of the device. Please try again.";
        public static string AttLogsUploadSuccess = "Uploading AttLogs was successful. \nTotal uploaded logs : {0}. ";
        public static string AttLogsNoRetrieved = "There was no log retrieved between the specified date.";
        public static string AttLogsAlreadyUploaded = "Attendance logs already uploaded.";
        public static string MinMaxUploadSuccess = "Uploading filtered logs was successful. \nTotal uploaded logs : {0}. ";
        public static string DeviceDuplicateTmpl = "Employee {0} ,index {1} was not loaded. Template already exist.";
        public static string DeviceQuestion = "Proceed with loading {0} to Device {1}?";
        public static string DBQuestion = "Proceed with uploading Device {0} to {1}?";
        public static string DefaultErr = "Unable to process transaction";
        public static string CopyQuestion = "Proceed copying from {0} to {1}?";
        public static string IdenticalSourceTarget = "Select different target group.";
        public static string NoSelectedRow = "No selected row(s) to {0}.";
    }
}
