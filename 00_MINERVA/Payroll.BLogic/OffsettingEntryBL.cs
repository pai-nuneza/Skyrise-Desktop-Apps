using System;
using System.Collections.Generic;
using System.Text;

using CommonLibrary;
using Payroll.DAL;
using System.Data;

namespace Payroll.BLogic
{
    public class OffsettingEntryBL:BaseBL
    {
        #region Override Functions

        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;

            string sqlQry = @"INSERT INTO T_Offset
                                    (Off_EmployeeId
                                    ,Off_ApplicableDate
                                    ,Off_DateWork
                                    ,Off_Type
                                    ,Off_Seqno
                                    ,Off_HoursWork 
                                    ,Off_HoursUsed
	                                ,Off_Served
                                    ,Usr_login
                                    ,Ludatetime)
                                VALUES
                                    (@Off_EmployeeId
                                    ,@Off_ApplicableDate
                                    ,@Off_DateWork
                                    ,@Off_Type
                                    ,@Off_Seqno
                                    ,@Off_HoursWork 
                                    ,@Off_HoursUsed
                                    ,@Off_Served
                                    ,@Usr_login
                                    ,Getdate())
                                ";

            ParameterInfo[] paramInfo = new ParameterInfo[9];
            paramInfo[0] = new ParameterInfo("@Off_EmployeeId", row["Off_EmployeeId"]);
            paramInfo[1] = new ParameterInfo("@Off_ApplicableDate", row["Off_ApplicableDate"]);
            paramInfo[2] = new ParameterInfo("@Off_DateWork", row["Off_DateWork"]);
            paramInfo[3] = new ParameterInfo("@Off_Type", row["Off_Type"]);
            paramInfo[4] = new ParameterInfo("@Off_Seqno", row["Off_Seqno"]);
            paramInfo[5] = new ParameterInfo("@Off_HoursWork", row["Off_HoursWork"]);
            paramInfo[6] = new ParameterInfo("@Off_HoursUsed", row["Off_HoursUsed"]);
            paramInfo[7] = new ParameterInfo("@Off_Served", row["Off_Served"]);
            paramInfo[8] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQry, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retVal;
        }

        public override int Update(System.Data.DataRow row)
        {
            int retVal = 0;
            
            string sqlQry = @"
                            UPDATE T_Offset 
                            SET Off_HoursWork = @Off_HoursWork
                            ,Off_HoursUsed = @Off_HoursUsed
                            ,Off_Served = @Off_Served
                            ,Off_ApplicableDate = @Off_ApplicableDate
                            ,Usr_login = @Usr_Login
                            ,Ludatetime = Getdate()
                            WHERE Off_EmployeeId = @Off_EmployeeId
                                AND Off_DateWork = @Off_DateWork
                                AND Off_Type = @Off_Type
                                AND Off_Seqno = @Off_Seqno
                            ";

            ParameterInfo[] paramInfo = new ParameterInfo[9];
            paramInfo[0] = new ParameterInfo("@Off_EmployeeId", row["Off_EmployeeId"],SqlDbType.Char);
            paramInfo[1] = new ParameterInfo("@Off_ApplicableDate", row["Off_ApplicableDate"],SqlDbType.DateTime);
            paramInfo[2] = new ParameterInfo("@Off_DateWork", row["Off_DateWork"],SqlDbType.DateTime);
            paramInfo[3] = new ParameterInfo("@Off_Type", row["Off_Type"],SqlDbType.Char);
            paramInfo[4] = new ParameterInfo("@Off_Seqno", row["Off_Seqno"],SqlDbType.Char);
            paramInfo[5] = new ParameterInfo("@Off_HoursWork", row["Off_HoursWork"],SqlDbType.Decimal);
            paramInfo[6] = new ParameterInfo("@Off_HoursUsed", row["Off_HoursUsed"],SqlDbType.Decimal);
            paramInfo[7] = new ParameterInfo("@Off_Served", row["Off_Served"],SqlDbType.Bit);
            paramInfo[8] = new ParameterInfo("@Usr_Login", row["Usr_Login"],SqlDbType.Char);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQry, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retVal;
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Delete(string code, string userLogin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Functions Defined

        public DataSet FetchMe(string code, string Off_ApplicableDate)
        {
            DataSet ds = new DataSet();

            string sqlquery = @"SELECT 
                                     Off_EmployeeId
                                    , Convert(Char(10), Off_DateWork, 101) as Off_DateWork 
                                    , Off_Type
                                    , Off_Seqno
                                    , Off_HoursWork
                                    , Off_HoursUsed
	                                , Off_Served
                                    , Convert(Char(10), Off_ApplicableDate, 101) as Off_ApplicableDate
                                    , Convert(Char(10), Off_ApplicableDate, 101) as ProcessDate
                                FROM T_Offset
                                WHERE Off_EmployeeId = @Off_EmployeeId
                                --And ( Off_HoursUsed = 0  Or Off_ApplicableDate = @Off_ApplicableDate )";

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("Off_EmployeeId", code);
                //paramCollection[1] = new ParameterInfo("Off_ApplicableDate", Off_ApplicableDate);

                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlquery, CommandType.Text, paramCollection);

                dal.CloseDB();
            }
            return ds;
        }

        #endregion

        #region Added delete option

        private void UnpostOffsetMin(string Ttr_ForOffsetMin, string Ttr_IDNo, string Ttr_Date, DALHelper DalUp)
        {
            int retVal = 0;

            #region query

            string qString = @"Update T_EmpTimeRegister
                                Set Ttr_ForOffsetMin = @Ttr_ForOffsetMin
	                                ,Usr_Login = @Usr_Login
	                                ,Ludatetime = Getdate()
                                Where Ttr_IDNo = @Ttr_IDNo
                                And Ttr_Date = @Ttr_Date";

            #endregion

            #region parameters

            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[0] = new ParameterInfo("@Ttr_ForOffsetMin", Ttr_ForOffsetMin);
            paramInfo[1] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);
            paramInfo[2] = new ParameterInfo("@Ttr_IDNo", Ttr_IDNo);
            paramInfo[3] = new ParameterInfo("@Ttr_Date", Ttr_Date);

            #endregion

            retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
        }

        private void DeleteOffsetRecord(string Off_SeqNo
                                      , string Off_EmployeeId
                                      , string Off_DateWork
                                      , string Off_Type
                                      , DALHelper DalUp)
        {
            int retVal = 0;

            #region query

            string qString = @"DELETE FROM T_Offset
                                WHERE Off_SeqNo = @Off_SeqNo
                                    AND Off_EmployeeId = @Off_EmployeeId
                                    AND Off_DateWork = @Off_DateWork
                                    AND Off_Type = @Off_Type";

            #endregion

            #region parameters

            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[0] = new ParameterInfo("@Off_SeqNo", Off_SeqNo);
            paramInfo[1] = new ParameterInfo("@Off_EmployeeId", Off_EmployeeId);
            paramInfo[2] = new ParameterInfo("@Off_DateWork", Off_DateWork);
            paramInfo[3] = new ParameterInfo("@Off_Type", Off_Type);

            #endregion

            retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
        }

        public void DeleteRecord(string Off_SeqNo
                                      , string IDNumber
                                      , string Off_DateWork
                                      , string Off_Type
                                      , string Ttr_ForOffsetMin
                                      , string Ttr_Date)
        {
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    this.DeleteOffsetRecord(Off_SeqNo, IDNumber, Off_DateWork, Off_Type, dal);
                    if (Ttr_Date.Trim() != string.Empty)
                        this.UnpostOffsetMin(Ttr_ForOffsetMin, IDNumber, Ttr_Date, dal);                  
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        #endregion

        #region added by Kevin for c1report
        public DataSet GetCompanyInfo()
        {
            DataSet ds = new DataSet();
            string sql = @"
                        declare @processflag1 as bit
                        set @processflag1 = (select Tsc_SetFlag from T_SettingControl
                        where Tsc_SystemCode = 'PERSONNEL' and Tsc_SettingCode = 'VWNICKNAME')

                        declare @processflag2 as bit
                        set @processflag2 = (select Tsc_SetFlag from T_SettingControl
                        where Tsc_SystemCode = 'PERSONNEL' and Tsc_SettingCode = 'DSPIDCODE')

                        Select Mcm_CompanyName
                              ,Mcm_CompanyAddress1 + ' ' + Mcm_CompanyAddress2 + ' ' + Mcd_Name as Address
                              ,'TEL NO. ' + Mcm_TelNo + ' FAX NO. ' + Mcm_FaxNo as Contacts
                              ,Mcm_CompanyLogo
                              ,case @processflag1 when 1 then 
                            case @processflag2 when 1 then 'ID Code'
						                       when 0 then 'Nick Name'
							end
						    when 0 then 'MI'
                            End as [MIHeader]
                        From M_Company
                        Inner Join M_CodeDtl on Mcm_CompanyAddress3 = Mcd_Code and Mcd_CodeType='ZIPCODE'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql);
                dal.CloseDB();
            }
            return ds;
        }
        public DataSet GetOffSetListingByWorkDate(string idnum, string type, string datestart, string dateend,
            string allid, string served, string allday)
        {
            DataSet ds;
            ParameterInfo[] param = new ParameterInfo[7];
            param[0] = new ParameterInfo("@IDNUM", idnum);
            param[1] = new ParameterInfo("@TYPE", type);
            param[2] = new ParameterInfo("@ls_dateStart", datestart);
            param[3] = new ParameterInfo("@ls_dateEnd", dateend);
            param[4] = new ParameterInfo("@AllID", allid);
            param[5] = new ParameterInfo("@Served", served);
            param[6] = new ParameterInfo("@AllDay", allday);

            #region query
            string sql = @"
DECLARE @ls_sql as char(8000)
DECLARE @ls_where as char(100)
If @TYPE = 'A'   
 SET @ls_where = ' and ((' + Cast(@AllDay as char(1)) + ' = 0 and Off_ApplicableDate between ''' + @ls_dateStart + ''' and ''' + @ls_dateEnd + ''') or
  ' + Cast(@AllDay as char(1)) + ' = 1) '  
ELSE  
 SET @ls_where = ' and ((' + Cast(@AllDay as char(1)) + ' = 0 and Off_DateWork between ''' + @ls_dateStart + ''' and ''' + @ls_dateEnd + ''') or
  ' + Cast(@AllDay as char(1)) + ' = 1) ' 


SET @ls_sql = 'select 
Off_EmployeeId  as ''ID Number''
, Mem_LastName as ''Last Name''      
, Mem_FirstName as ''First Name''               
, left(Mem_MiddleName , 1) as ''MI''  
, Convert(char(10),Off_DateWork,101) as ''Date Work''     
, Case when Off_Type = ''E'' then ''Earned''
when Off_Type = ''U'' then ''Unearned''
else '''' End as ''Type''
, Off_Seqno as ''Seq No''
, Off_HoursWork as ''Hours Work''
, Off_HoursUsed as ''Hours Used''
, Case when Off_Served = 1 then ''Yes'' else ''No'' End as ''Served''
, Convert(char(10),Off_ApplicableDate,101) as ''Applicable Date''
From dbo.T_OffSet
Inner join M_Employee on Mem_IDNo = Off_EmployeeId
Where ((Off_EmployeeId = '''+ @IDNUM + ''' and 0=' + Convert(char(1),@AllID) +') or 1 = ' + 
+ Convert(char(1),@AllID) +') and Off_Served = ' + Convert(char(1),@Served)  + RTrim(@ls_where) 
+ 'order by Mem_LastName,Mem_FirstName'

exec (@ls_sql)";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                dal.CloseDB();
            }

            if (!(ds.Tables[0].Rows.Count > 0))
                throw new PayrollException("No records retrieved in the report.");

            return ds;
        }

        public DataSet GetOffsetListingByAppDate(string idnum, string type, string datestart, string dateend,
            string allid, string served, string allday)
        {
            DataSet ds;
            ParameterInfo[] param = new ParameterInfo[7];
            param[0] = new ParameterInfo("@IDNUM", idnum);
            param[1] = new ParameterInfo("@TYPE", type);
            param[2] = new ParameterInfo("@ls_dateStart", datestart);
            param[3] = new ParameterInfo("@ls_dateEnd", dateend);
            param[4] = new ParameterInfo("@AllID", allid);
            param[5] = new ParameterInfo("@Served", served);
            param[6] = new ParameterInfo("@AllDay", allday);

            #region query
            string sql = @"
DECLARE @ls_sql as char(8000)
DECLARE @ls_where as char(100)
If @TYPE = 'A'   
 SET @ls_where = ' and ((' + Cast(@AllDay as char(1)) + ' = 0 and Off_ApplicableDate between ''' + @ls_dateStart + ''' and ''' + @ls_dateEnd + ''') or
  ' + Cast(@AllDay as char(1)) + ' = 1)'  
ELSE  
 SET @ls_where = ' and ((' + Cast(@AllDay as char(1)) + ' = 0 and Off_DateWork between ''' + @ls_dateStart + ''' and ''' + @ls_dateEnd + ''') or
  ' + Cast(@AllDay as char(1)) + ' = 1)' 


SET @ls_sql = 'select 
Off_EmployeeId  as ''ID Number''
, Mem_LastName as ''Last Name''      
, Mem_FirstName as ''First Name''               
, left(Mem_MiddleName , 1) as ''MI''  
, Convert(char(10),Off_DateWork,101) as ''Date Work''     
, Case when Off_Type = ''E'' then ''Earned''
when Off_Type = ''U'' then ''Unearned''
else '''' End as ''Type''
, Off_Seqno as ''Seq No''
, Off_HoursWork as ''Hours Work''
, Off_HoursUsed as ''Hours Used''
, Case when Off_Served = 1 then ''Yes'' else ''No'' End as ''Served''
, Convert(char(10),Off_ApplicableDate,101) as ''Applicable Date''
From dbo.T_OffSet
Inner join M_Employee on Mem_IDNo = Off_EmployeeId
Where ((Off_EmployeeId = '''+ @IDNUM + ''' and 0=' + Convert(char(1),@AllID) +') or 1 = ' + 
+ Convert(char(1),@AllID) +') and Off_Served = ' + Convert(char(1),@Served)  + RTrim(@ls_where) 
+ 'order by Mem_LastName,Mem_FirstName'
exec (@ls_sql)";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                dal.CloseDB();
            }

            if (!(ds.Tables[0].Rows.Count > 0))
                throw new PayrollException("No records retrieved in the report.");

            return ds;
        }

        public DataSet GetOffsetListingByEmployee(string idnum, string type, string datestart, string dateend,
            string allid, string served, string allday)
        {
            DataSet ds;
            ParameterInfo[] param = new ParameterInfo[7];
            param[0] = new ParameterInfo("@IDNUM", idnum);
            param[1] = new ParameterInfo("@TYPE", type);
            param[2] = new ParameterInfo("@ls_dateStart", datestart);
            param[3] = new ParameterInfo("@ls_dateEnd", dateend);
            param[4] = new ParameterInfo("@AllID", allid);
            param[5] = new ParameterInfo("@Served", served);
            param[6] = new ParameterInfo("@AllDay", allday);
            
            #region query
            string sql = @"
DECLARE @ls_sql as char(8000)
DECLARE @ls_where as char(100)
If @TYPE = 'A'   
 SET @ls_where = ' and ((' + Cast(@AllDay as char(1)) + ' = 0 and Off_ApplicableDate between ''' + @ls_dateStart + ''' and ''' + @ls_dateEnd + ''') or
  ' + Cast(@AllDay as char(1)) + ' = 1)'  
ELSE  
 SET @ls_where = ' and ((' + Cast(@AllDay as char(1)) + ' = 0 and Off_DateWork between ''' + @ls_dateStart + ''' and ''' + @ls_dateEnd + ''') or
  ' + Cast(@AllDay as char(1)) + ' = 1)' 


SET @ls_sql = 'select 
Off_EmployeeId  as ''ID Number''
, Mem_LastName as ''Last Name''      
, Mem_FirstName as ''First Name''               
, left(Mem_MiddleName , 1) as ''MI''  
, Convert(char(10),Off_DateWork,101) as ''Date Work''     
, Case when Off_Type = ''E'' then ''Earned''
when Off_Type = ''U'' then ''Unearned''
else '''' End as ''Type''
, Off_Seqno as ''Seq No''
, Off_HoursWork as ''Hours Work''
, Off_HoursUsed as ''Hours Used''
, Case when Off_Served = 1 then ''Yes'' else ''No'' End as ''Served''
, Convert(char(10),Off_ApplicableDate,101) as ''Applicable Date''
From dbo.T_OffSet
Inner join M_Employee on Mem_IDNo = Off_EmployeeId
Where ((Off_EmployeeId = '''+ @IDNUM + ''' and 0=' + Convert(char(1),@AllID) +') or 1 = ' + 
+ Convert(char(1),@AllID) +') and Off_Served = ' + Convert(char(1),@Served)  + RTrim(@ls_where) 
+ 'order by Mem_LastName,Mem_FirstName'
exec (@ls_sql)";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                dal.CloseDB();
            }

            if (!(ds.Tables[0].Rows.Count > 0))
                throw new PayrollException("No records retrieved in the report.");

            return ds;
        }
        #endregion
    }
}
