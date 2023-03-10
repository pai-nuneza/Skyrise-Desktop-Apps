using System;
using System.Data;
using System.Collections.Specialized;

namespace CommonPostingLibrary
{
    public class BOMDataHelper
    {
        private DataRow excelDataRow;
        public BOMDataHelper(DataRow exDr)
        {
            excelDataRow = exDr;
        }

        /* Notes:
                                                                                 * F1 = Level Number + Process Code/Stock Code
                                                                                 * F2 = Nothing by default but sometimes would contain some continuation text of the Process/Stock Code
                                                                                 * F3 = Decription
                                                                                 * F4 = Nothing by default but sometimes would contain some continuation text of the Description
                                                                                 * F5 = Quantity
                                                                                 * F6 = Unit of Measure
                                                                                 * F7 = Ignore
                                                                                 * F8 = I for Stock / J for Process
                                                                                 * F9 = Ignore
                                                                                 * F10 = Ignore
                                                                                 * F11 = Ignore
                                                                                 * F12 = P is for packing
                                                                                 * 
                                                                                */

        public NameValueCollection  GenerateBOMData()
        {
            NameValueCollection nvCollections = new NameValueCollection();
            string sLevel = GetLevel().Trim().ToString();
            string sProcessCode = "";
            string sStockCode = "";
            string sProductCode = "";
            string sType = GetBOMType().Trim().ToString();
            if (sType == "I")
                sStockCode = GetProcessOrStockCode().Trim().ToString();
            else
                if (sType == "J")
                    sProcessCode = GetProcessOrStockCode().Trim().ToString();
                else
                    if (sType == string.Empty)
                        sProductCode = GetProcessOrStockCode().Trim().ToString();

            string sDescription = GetDescription().Trim().ToString();
            string sQuantity = GetQuantity().Trim().ToString();
            string sUnitOfMeasure = GetUnitOfMeasure().Trim().ToString();
            string sPacking = GetPacking().Trim().ToString();

            nvCollections.Add("Level", sLevel);
            nvCollections.Add("ProductCode", sProductCode);
            nvCollections.Add("ProcessCode", sProcessCode);
            nvCollections.Add("StockCode", sStockCode);
            nvCollections.Add("Type", sType);
            nvCollections.Add("Description", sDescription);
            nvCollections.Add("Quantity", sQuantity);
            nvCollections.Add("UnitOfMeasure", sUnitOfMeasure);
            nvCollections.Add("Packing", sPacking);

            return nvCollections;
        }

        public string ContructTreeofBOM(string sLevel)
        {
            string retVal = "";
            int intLevel = Convert.ToInt16(sLevel);

            for (int x = 0; x < intLevel; x++)
            {
                if (retVal != "")
                    retVal += "|";
                retVal += (x + 1).ToString();
            }

            return retVal == string.Empty ? "0" : "0|" + retVal;
        }

        private string GetLevel()
        {
            string retVal = "";
            string sLevel = excelDataRow["F1"].ToString();
            retVal = sLevel.Split(' ')[0];
            return retVal;
        }

        private string GetProcessOrStockCode()
        {
            string retVal = "";
            retVal = (excelDataRow["F1"].ToString() +  excelDataRow["F2"].ToString()).Split(' ')[1].ToString();
            return retVal ;
        }

        private string GetDescription()
        {
            string retVal = "";
            string[] strF1Split = (excelDataRow["F1"].ToString() +  excelDataRow["F2"].ToString()).Split(' ');
            if (strF1Split.Length > 2)
            {
                for (int x = 3; x < strF1Split.Length; x++)
                    retVal = strF1Split[x].Trim().ToString();
            }
            retVal += excelDataRow["F3"].ToString() + excelDataRow["F4"].ToString();
            return retVal;
        }

        private string GetQuantity()
        {
            string retVal = "";
            retVal = excelDataRow["F5"].ToString();
            return retVal;
        }

        private string GetUnitOfMeasure()
        {
            string retVal = "";
            retVal = excelDataRow["F6"].ToString();
            return retVal;
        }

        private string GetBOMType()
        {
            string retVal = "";
            retVal = excelDataRow["F8"].ToString();
            return retVal;
        }

        private string GetPacking()
        {
            string retVal = "";
            retVal = excelDataRow["F12"].ToString();
            return retVal;
        }
    }
}
