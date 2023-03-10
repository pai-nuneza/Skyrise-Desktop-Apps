using System;
using System.Collections.Generic;
using System.Text;

namespace CommonPostingLibrary
{
    public class CommonEnum
    {
        public enum Version
        {
            [StringValue("1.01")]
            SystemVer = 0
        }

        public enum ConsoleArguments
        {
            [StringValue("MASTERFILES")]
            MASTERFILES = 0,
            [StringValue("CHIPSDOWNLOAD")]
            CHIPSDOWNLOAD = 1,
            [StringValue("PlanningUpload")]
            PlanningUpload = 2,
            [StringValue("INVENTORY")]
            INVENTORY = 3,
            [StringValue("TransactionFiles")]
            TransactionFiles = 4,
            [StringValue("RAW")]
            RAW = 5,
            [StringValue("GIP")]
            GIP = 6,
            [StringValue("GRV")]
            GRV = 7,
            [StringValue("BAL")]
            BAL = 8,
            [StringValue("FGS")]
            FGS = 9,
            [StringValue("FGO")]
            FGO = 10,
            [StringValue("RMU")]
            RMU = 11,
            [StringValue("PRA")]
            PRA = 12,
            [StringValue("PUR")]
            PUR = 13,
            [StringValue("PRQ")]
            PRQ = 14,
            [StringValue("MMU")]
            MMU = 15,
            [StringValue("MRP")]
            MRP = 16,
            [StringValue("PlanningDownload")]
            PlanningDownload = 17
        }

        public enum DocSumColumnIndex
        {
            [StringValue("1")]
            RAW = 0,
            [StringValue("5")]
            GIP = 1,
            [StringValue("4")]
            GRV = 2,
            [StringValue("1")]
            FGS = 3,
            [StringValue("3")]
            PUR = 4,
            [StringValue("3")]
            PRQ = 5,
            [StringValue("5")]
            MMU = 6
        }

        public enum DocDefinition
        {
            [StringValue("Current Stock of Raw Materials")]
            RAW = 0,
            [StringValue("Goods Issue to Production")]
            GIP = 1,
            [StringValue("Goods Receipt from Vendor  (Non CHIP raw materials)")]
            GRV = 2,
            [StringValue("Finished Goods Stock")]
            FGS = 3,
            [StringValue("Open Purchase Order")]
            PUR = 4,
            [StringValue("Open Purchase Request")]
            PRQ = 5,
            [StringValue("Material Master Update")]
            MMU = 6
        }

        public enum LocatorTypeMB
        {
            [StringValue("B")]
            BOX = 0,
            [StringValue("M")]
            MACHINE = 1
        }
        public enum DefectCategory
        {
            [StringValue("T")]
            TRUE_DEFECT = 0,
            [StringValue("S")]
            SALES_SAMPLE = 1,
            [StringValue("E")]
            ENGINEERING_SAMPLE = 2,
            [StringValue("R")]
            RECYCABLE_PRODUCT = 3
        }

        public enum SPOStatus
        {
            [StringValue("N")]
            NOPO = 0,
            [StringValue("G")]
            GENERATED = 1,
            [StringValue("F")]
            FULLFILLED = 2,
            [StringValue("A")]
            APPROVED = 3
        }

        public enum MenuCode
        {
            [StringValue("BOMAltStkMr")]
            BOMAltStkMr = 0,
            [StringValue("BarPrtLotCode")]
            BarPrtLotCode = 1,
            [StringValue("RRBarPrtMat")]
            RRBarPrtMat = 2,
            [StringValue("BOMMr")]
            BOMMr = 4,
            [StringValue("CanvassList")]
            CanvassList = 5,
            [StringValue("CanvassListCSM")]
            CanvassListCSM = 6,
            [StringValue("TACCurMstr")]
            TACCurMstr = 7,
            [StringValue("TACFrxRt")]
            TACFrxRt = 8,
            [StringValue("CostCntrMr")]
            CostCntrMr = 9,
            [StringValue("CSMRequestHold")]
            CSMRequestHold = 10,
            [StringValue("CustDRHold")]
            CustDRHold = 11,
            [StringValue("DelAns")]
            DelAns = 13,
            [StringValue("Dept")]
            Dept = 14,
            [StringValue("Div")]
            Div = 15,
            [StringValue("DocCont")]
            DocCont = 16,
            [StringValue("DocSigDef")]
            DocSigDef = 17,
            [StringValue("DRApproval")]
            DRApproval = 19,
            [StringValue("DRecApproval")]
            DRecApproval = 21,
            [StringValue("DREntry")]
            DREntry = 22,
            [StringValue("DRHdrEntryCSM")]
            DRHdrEntryCSM = 23,
            [StringValue("DRPrinting")]
            DRPrinting = 24,
            [StringValue("DRPrintingCSM")]
            DRPrintingCSM = 25,
            [StringValue("InvAdjPost")]
            InvAdjPost = 26,
            [StringValue("InvApproval")]
            InvApproval = 27,
            [StringValue("InvCountEntry")]
            InvCountEntry = 28,
            [StringValue("InvDataGen")]
            InvDataGen = 29,
            [StringValue("InvDetailEntry")]
            InvDetailEntry = 30,
            [StringValue("InvHeaderEntry")]
            InvHeaderEntry = 31,
            [StringValue("InvPrinting")]
            InvPrinting = 32,
            [StringValue("InvTagsPrinting")]
            InvTagsPrinting = 33,
            [StringValue("ISAdjRetSup")]
            ISAdjRetSup = 34,
            [StringValue("ISAdjRetWhs")]
            ISAdjRetWhs = 35,
            [StringValue("ISRequisition")]
            ISRequisition = 36,
            [StringValue("ISWithdraw")]
            ISWithdraw = 37,
            [StringValue("WhsMr")]
            WhsMr = 38,
            [StringValue("MatAdjRetSup")]
            MatAdjRetSup = 39,
            [StringValue("MatAdjRetWhs")]
            MatAdjRetWhs = 40,
            [StringValue("MatHoldTag")]
            MatHoldTag = 41,
            [StringValue("MatRequi")]
            MatRequi = 42,
            [StringValue("MatSCAREntry")]
            MatSCAREntry = 43,
            [StringValue("MWMatWithdraw")]
            MWMatWithdraw = 44,
            [StringValue("PackAss")]
            PackAss = 45,
            [StringValue("PayTerms")]
            PayTerms = 46,
            [StringValue("MWPickList")]
            MWPickList = 47,
            [StringValue("PRApproval")]
            PRApproval = 48,
            [StringValue("PrefSupEntry")]
            PrefSupEntry = 49,
            [StringValue("PrefSupEntryCSM")]
            PrefSupEntryCSM = 50,
            [StringValue("PREntry")]
            PREntry = 51,
            [StringValue("PRHold")]
            PRHold = 52,
            [StringValue("PRInquiry")]
            PRInquiry = 53,
            [StringValue("Proc")]
            Proc = 54,
            [StringValue("PRPrinting")]
            PRPrinting = 55,
            [StringValue("RRApproval")]
            RRApproval = 56,
            [StringValue("RRChkList")]
            RRChkList = 57,
            [StringValue("RREntryRD")]
            RREntryRD = 58,
            [StringValue("RREntryRE")]
            RREntryRE = 59,
            [StringValue("RREntryRX")]
            RRENTRYRX = 60,
            [StringValue("RRPrintMat")]
            RRPrintMat = 61,
            [StringValue("RRPrintSup")]
            RRPrintSup = 62,
            [StringValue("RRHold")]
            RRHold = 63,
            [StringValue("RRPrep")]
            RRPrep = 64,
            [StringValue("RRPrint")]
            RRPrint = 65,
            [StringValue("Sec")]
            Sec = 66,
            [StringValue("SkipPicklist")]
            SkipPicklist = 67,
            [StringValue("SPOApproval")]
            SPOApproval = 68,
            [StringValue("SPOEntry")]
            SPOEntry = 69,
            [StringValue("SPOGen")]
            SPOGen = 70,
            [StringValue("SPOHold")]
            SPOHold = 71,
            [StringValue("SPOPrint")]
            SPOPrint = 72,
            [StringValue("SPOReprint")]
            SPOReprint = 73,
            [StringValue("StkCntSeries")]
            StkCntSeries = 74,
            [StringValue("StkMr")]
            StkMr = 75,
            [StringValue("StkPriceCSM")]
            StkPriceCSM = 76,
            [StringValue("PrcStkPriceMr")]
            PrcStkPriceMr = 77,
            [StringValue("StkStrgCond")]
            StkStrgCond = 78,
            [StringValue("StkTypeMr")]
            StkTypeMr = 79,
            [StringValue("ProdSubProc")]
            ProdSubProc = 80,
            [StringValue("SubSec")]
            SubSec = 81,
            [StringValue("SupContPer")]
            SupContPer = 82,
            [StringValue("SupInst")]
            SupInst = 83,
            [StringValue("SupMr")]
            SupMr = 84,
            [StringValue("SupMrCSM")]
            SupMrCSM = 85,
            [StringValue("SuppInvHold")]
            SuppInvHold = 86,
            [StringValue("SysCtrl")]
            SysCtrl = 87,
            [StringValue("StkUnitMeas")]
            StkUnitMeas = 88,
            [StringValue("UnitMeas")]
            UPACCNTINVADJ = 89,
            UPACCNTRRDATA = 90,
            [StringValue("UserGrant")]
            UserGrant = 91,
            [StringValue("UserGrp")]
            UserGrp = 92,
            [StringValue("UserMr")]
            UserMr = 93,
            [StringValue("ViewDRDelAns")]
            ViewDRDelAns = 94,
            [StringValue("WHSIssuanceType")]
            WHSIssuanceType = 95,
            [StringValue("RRWHSLocTag")]
            RRWHSLocTag = 96,
            [StringValue("WHSMr")]
            WHSMr = 97,
            [StringValue("WRISEntry")]
            WRISEntry = 98,
            [StringValue("WRISManFulfill")]
            WRISManFulfill = 99,
            [StringValue("StkPCSM")]
            StkPCSM = 101,
            [StringValue("DRCSM")]
            DRCSM = 102,
            [StringValue("SeparateNCLot")]
            SeparateNCLot = 106,
            [StringValue("DReceiptDEntry")]
            DReceiptDEntry = 107,
            [StringValue("RRPackAss")]
            RRPackAss = 108,
            [StringValue("TACPayTerms")]
            TACPayTerms = 109,
            [StringValue("RREntry")]
            RREntry = 110,
            [StringValue("PRBalanceInq")]
            PRBalanceInq = 111,
            [StringValue("CSMTransactInq")]
            CSMTransactInq = 112,
            [StringValue("CSMBalanceInq")]
            CSMBalanceInq = 113,
            [StringValue("RRSDEntry")]
            RRSDEntry = 116,
            [StringValue("RRRMEntry")]
            RRRMEntry = 118,
            [StringValue("NCEntry")]
            NCEntry = 122,
            [StringValue("SCAREntry")]
            SCAREntry = 123,
            [StringValue("SCARPrint")]
            SCARPrint = 123,
            [StringValue("WTWEntry")]
            WTWEntry = 125,
            [StringValue("WTWReport")]
            WTWReport = 126,
            [StringValue("InvtyConSeries")]
            InvtyConSeries = 129,
            [StringValue("SupContPerCSM")]
            SupContPerCSM = 130,
            [StringValue("StkMrSupp")]
            StkMrSupp = 131,
            [StringValue("SalesPrice")]
            SalesPrice = 132,
            [StringValue("BOMPrint")]
            BOMPrint = 133,
            [StringValue("DocSigSectHeadSig")]
            DocSigSectHeadSig = 134,
            [StringValue("ExpenseSumSect")]
            ExpenseSumSect = 135,
            [StringValue("MWMatRequest")]
            MWMatRequest = 136,
            [StringValue("MWKitTagPrint")]
            MWKitTagPrint = 137,
            [StringValue("TACDelTerms")]
            TACDelTerms = 138,
            [StringValue("SysCalendar")]
            SysCalendar = 139,
            [StringValue("PRReview")]
            PRReview = 140,
            [StringValue("SPOReview")]
            SPOReview = 141,
            [StringValue("RTSReport")]
            RTSReport = 142,
            [StringValue("MWRTCReport")]
            MWRTCReport = 143,
            [StringValue("RTSEntry")]
            RTSEntry = 144,
            [StringValue("RTCEntry")]
            RTCEntry = 145,
            [StringValue("MWMatApproval")]
            MWMatApproval = 146,
            [StringValue("CustMr")]
            CustMr = 147,
            [StringValue("CustEndCustMr")]
            CustEndCustMr = 147,
            [StringValue("Bak")]
            Bak = 148,
            [StringValue("CusCon")]
            CusCon = 149,
            [StringValue("CPOEntry")]
            CPOEntry = 150,
            [StringValue("CPOApproval")]
            CPOApproval = 151,
            [StringValue("PTMWEntry")]
            PTMWEntry = 152,
            [StringValue("CustContPerson")]
            CustContPerson = 153,
            [StringValue("CustPartNumb")]
            CustPartNumb = 154,
            [StringValue("IPInvtyData")]
            IPInvtyData = 155,
            [StringValue("QADefCodeMr")]
            QADefCodeMr = 156,
            [StringValue("QACCDefCodeMr")]
            QACCDefCodeMr = 157,
            [StringValue("TACBankMaster")]
            TACBankMaster = 159,
            [StringValue("CANCELOFISSUED")]
            CANCELOFISSUED = 160,
            [StringValue("APPROVALC")]
            APPROVALC = 161,
            [StringValue("ProdInputReq")]
            ProdInputReq = 162,
            [StringValue("BOMApp")]
            BOMApp = 163,
            [StringValue("ProdMaster")]
            ProdMaster = 164,
            [StringValue("ProdPSG")]
            ProdPSG = 165,
            [StringValue("PPFGEndorsed")]
            PPFGEndorsed = 166,
            [StringValue("BuildMaterials")]
            BuildMaterials = 167,
            [StringValue("RPSPOBalances")]
            RPSPOBalances = 168,
            [StringValue("RREntryRF")]
            RREntryRF = 169,
            [StringValue("RRPrepFG")]
            RRPrepFG = 170,
            [StringValue("MWPICKLISTPRINT")]
            MWPICKLISTPRINT = 171,
            [StringValue("ProformaInvoice")]
            ProformaInvoice = 172,
            [StringValue("DR")]
            DR = 173,
            [StringValue("RPExpSumRep")]
            RPExpSumRep = 174,
            [StringValue("QtyAdjustments")]
            QtyAdjustments = 175,
            [StringValue("MWMATADJ")]
            MWMATADJ = 176,
            [StringValue("ProdPackingMr")]
            ProdPackingMr = 177,
            [StringValue("WhsLandTrans")]
            WhsLandTrans = 178,
            [StringValue("DelTrack")]
            DelTrack = 179,
            [StringValue("DELRPRINTING")]
            DELRPRINTING = 180,
            [StringValue("PROFORMAINVPRNT")]
            PROFORMAINVPRNT = 181,
            [StringValue("PACKINGINVLIST")]
            PACKINGINVLIST = 182,
            [StringValue("SIApproval")]
            SIApproval = 183,
            [StringValue("SPPackAssignment")]
            SPPackAssignment = 184,
            [StringValue("SPLocaTagging")]
            SPLocaTagging = 185,
            [StringValue("SPRequest")]
            SPRequest = 186,
            [StringValue("SPApproval")]
            SPApproval = 187,
            [StringValue("SPGenPicklist")]
            SPGenPicklist = 188,
            [StringValue("SPPickList")]
            SPPickList = 189,
            [StringValue("SPKittingTag")]
            SPKittingTag = 190,
            [StringValue("SPIssueEntry")]
            SPIssueEntry = 191,
            [StringValue("SPIssueCancel")]
            SPIssueCancel = 192,
            [StringValue("SPApprovalCancel")]
            SPApprovalCancel = 193,
            [StringValue("IncomingInsp")]
            IncomingInsp = 194,
            [StringValue("InvLabelPrn")]
            InvLabelPrn = 195,
            [StringValue("ProdLineDefSet")]
            ProdLineDefSet = 196,
            [StringValue("DelComApp")]
            DelComApp = 197,
            [StringValue("DelComEntry")]
            DelComEntry = 198,
            [StringValue("DelComIssue")]
            DelComIssue = 199,
            [StringValue("DelComPcklst")]
            DelComPcklst = 200,
            [StringValue("ProdLineDefApproval")]
            ProdLineDefApproval = 201,
            [StringValue("JobOrder")]
            JobOrder = 202,
            [StringValue("ProdSched")]
            ProdSched = 203,
            [StringValue("RPStocksMonitoring")]
            RPStocksMonitoring = 204,
            [StringValue("MatStckTrnsfr")]
            MatStckTrnsfr = 205,
            [StringValue("ProdCCDowntimeMr")]
            ProdCCDowntimeMr = 206,
            [StringValue("ProdRouteMr")]
            ProdRouteMr = 207,
            [StringValue("ProdDowntimeMr")]
            ProdDowntimeMr = 208,
            [StringValue("IWHTRANSPRINT")]
            IWHTRANSPRINT = 209,
            [StringValue("ProdConverMr")]
            ProdConverMr = 210,
            [StringValue("ProdDiesetMr")]
            ProdDiesetMr = 211,
            [StringValue("ProdMachMr")]
            ProdMachMr = 212,
            [StringValue("ProdManpwrMr")]
            ProdManpwrMr = 213,
            [StringValue("CPOPrint")]
            CPOPrint = 214,
            [StringValue("DRAppr")]
            DRAppr = 214,
            [StringValue("SalesInvoice")]
            SalesInvoice = 215,
            [StringValue("SIBACKTRACK")]
            SIBACKTRACK = 216,
            [StringValue("SpDelCom")]
            SpDelCom = 217,
            [StringValue("RetWHApp")]
            RetWHApp = 218,
            [StringValue("RPExpSumSect")]
            RPExpSumSect = 219,
            [StringValue("RPExpSumSupp")]
            RPExpSumSupp = 220,
            [StringValue("RRApprovalOfNC")]
            RRApprovalOfNC = 221,
            [StringValue("TravelogPrep")]
            TravelogPrep = 222,
            [StringValue("PPTRAVELOGPRINT")]
            PPTRAVELOGPRINT = 223,
            [StringValue("CPOBalance")]
            CPOBalance = 224,
            [StringValue("PRODSCHEDREPORT")]
            PRODSCHEDREPORT = 225,
            [StringValue("ACCOUNTMASTER")]
            ACCOUNTMASTER = 226,
            [StringValue("ACCOUNTLINK")]
            ACCOUNTLINK = 227,
            [StringValue("RPRMonitoring")]
            RPRMonitoring = 228,
            [StringValue("CPOPriceTrl")]
            CPOPriceTrl = 229,
            [StringValue("PPMETravelog")]
            PPMETravelog = 230,
            [StringValue("CCCFStockMR")]
            CCCFStockMR = 231,
            [StringValue("TShortage")]
            TShortage = 232,
            [StringValue("TStockMovApp")]
            TStockMovApp = 233,
            [StringValue("TStockMov")]
            TStockMov = 234,
            [StringValue("PPTTravSplit")]
            PPTTravSplit = 235,
            [StringValue("TStockMoveRep")]
            TStockMoveRep = 236,
            [StringValue("TStockMovPrnt")]
            TStockMovPrnt = 237,
            [StringValue("TStckShrtPrnt")]
            TStckShrtPrnt = 238,
            [StringValue("RPMaterialAging")]
            RPMaterialAging = 239,
            [StringValue("PPTTravelogHold")]
            PPTTravelogHold = 240,
            [StringValue("RPExpensePayable")]
            RPExpensePayable = 241,
            [StringValue("RPPurchaseDynamic")]
            RPPurchaseDynamic = 242,
            [StringValue("RPDPR")]
            RPDPR = 243,
            //[StringValue("SuppInvHold")]
            //SuppInvHold = 244,
            [StringValue("RRIncomingInsp")]
            RRIncomingInsp = 244,
            [StringValue("RRBarPrtMatFG")]
            RRBarPrtMatFG = 245,
            [StringValue("PPTPTravelMPros")]
            PPTPTravelMPros = 246,
            [StringValue("PPTPTravelSPros")]
            PPTPTravelSPros = 247,
            [StringValue("InvHold")]
            InvHold = 248,
            [StringValue("WhsLocMr")]
            WhsLocMr = 249,
            [StringValue("AssetTransfer")]
            AssetTransfer = 250,
            [StringValue("TargetIssuance")]
            TargetIssuance = 251,
            [StringValue("CPSRECTEMPDR")]
            CPSRECTEMPDR = 252,
            [StringValue("CPSDELSCHED")]
            CPSDELSCHED = 253,
            [StringValue("CDeliveryStatus")]
            CDeliveryStatus = 254,
            //CDeliveryStatus
            [StringValue("InvReconcile")]
            InvReconcile = 255,
            [StringValue("PODRCreation")]
            PODRCreation = 256,
            [StringValue("CCProdInfo")]
            CCProdInfo = 257,
            [StringValue("PrintMaeKotie")]
            PrintMaeKotie = 258,
            [StringValue("CPSSPOENTRY")]
            CPSSPOENTRY = 259,
            [StringValue("TRAssyLot")]
            TRAssyLot = 260,
            [StringValue("RRENTRYCO")]
            RRENTRYCO = 261,
            //DailyInputPlan
            [StringValue("DailyInputPlan")]
            DailyInputPlan = 262,
            [StringValue("TTRAVELOGReproc")]
            TTRAVELOGReproc = 263,
            [StringValue("TTRAVELOGWSPLIT")]
            TTRAVELOGWSPLIT = 264,
            [StringValue("DelCodeMaster")]
            DelCodeMaster = 265,
            [StringValue("ProcessMaterialComp")]
            ProcessMaterialComp = 266,
            [StringValue("TTAVDetailInfo")]
            TTRAVDetailInfo = 267,
            [StringValue("RPMATCONTLEDGER")]
            RPMATCONTLEDGER = 268,
            [StringValue("RPRMStockReport")]
            RPRMStockReport = 269,
            [StringValue("RPSummaryofRR")]
            RPSummaryofRR = 270,
            [StringValue("RRDUMMY")]
            RRDUMMY = 271,
            [StringValue("SPOPRPO")]
            SPOPRPO = 272,
            [StringValue("RRApprovalWafer")]
            RRApprovalWafer = 273
        }

        public enum FormState
        {
            [StringValue("NORMAL_STATE")]
            NORMAL_STATE = 0,
            [StringValue("NEW_STATE")]
            NEW_STATE = 1,
            [StringValue("MODIFY_STATE")]
            MODIFY_STATE = 2
        }

        public enum ConditionalOperators
        {
            [StringValue("=")]
            EQUAL = 0,
            [StringValue("<>")]
            NOT_EQUAL = 1,
            [StringValue(">")]
            GREATER_THAN = 2,
            [StringValue(">=")]
            GREATER_OR_EQUAL = 3,
            [StringValue("<")]
            LESS_THAN = 4,
            [StringValue("<=")]
            LESS_OR_EQUAL = 5,
            [StringValue("LIKE")]
            LIKE = 6
        }

        public enum LogicalOperators
        {
            [StringValue("AND")]
            AND = 1,
            [StringValue("OR")]
            OR = 2,
            [StringValue("NOT")]
            NOT = 3
        }

        public enum ControlTables
        {
            [StringValue("Scm_SupplierPO")]
            SUPPLIER_PO = 0,
            [StringValue("Scm_CustomerPO")]
            CUSTOMER_PO = 1,
            [StringValue("Scm_JobOrderControl")]
            JOB_ORDER = 2,
            [StringValue("Scm_WRISControl")]
            WRIS = 3,
            [StringValue("Scm_RRControl")]
            RR = 4,
            [StringValue("Scm_DelReturnControl")]
            DELIVERY_RETURN = 5,
            [StringValue("Scm_QAInspection")]
            QA_INSPECTION = 6,
            [StringValue("Scm_QAClaim")]
            QA_CLAIM = 7,
            [StringValue("Scm_StockControl")]
            STOCKCODE_DISPLAY = 8,
            //Date:08/04/2006 2:00PM
            //Author:Eugene C. Biton
            //Start            
            [StringValue("Scm_InvoiceControl")]
            INVOICE_CONTROL = 9,
            [StringValue("Scm_ProformaInvoice")]
            PROFORMA_INVOICE = 10,
            [StringValue("Scm_LotSetting")]
            LOTSETTING_CONTROL = 11,
            //End
            [StringValue("Scm_StockTransferWH")]
            STOCKTRANSFER_WH = 12,
            [StringValue("Scm_StockTransferCust")]
            STOCKTRANSFER_CUSTOMER = 13
        }

        public enum Status
        {
            [StringValue("A")]
            ACTIVE = 0,
            [StringValue("C")]
            CANCELLED = 1,
            [StringValue("U")]
            ONHOLD = 2,
            [StringValue("F")]
            FULFILLED = 3,
            [StringValue("N")]
            NEW = 4,
            [StringValue("A")]
            APPROVED = 5,
            [StringValue("G")]
            GENERATED = 6,
            [StringValue("A")]
            AMENDED = 7,
            [StringValue("F")]
            FOR_POSTING = 8,
            [StringValue("P")]
            POSTED = 9,
            [StringValue("L")]
            LOADED = 10,
            [StringValue("I")]
            INTRANSIT = 11,
            [StringValue("R")]
            REVIEWED = 12,
            [StringValue("X")]
            EXPAND = 13
        }

        public enum InspectionCriteria
        {
            [StringValue("A", "Inspection")]
            INSPECTION = 0,
            [StringValue("B", "Non-Inspection")]
            NON_INSPECTION = 1,
            [StringValue("C", "Re-Inspection")]
            REINSPECTION = 2
        }

        public enum Origin
        {
            [StringValue("L")]
            LOCAL = 0,
            [StringValue("E")]
            FOREIGN = 1
        }

        public enum OriginCSM
        {
            [StringValue("C")]
            CSM = 0
        }

        public enum OriginAPI
        {
            [StringValue("L", "LOCAL")]
            LOCAL = 0,
            [StringValue("E", "EXTERNAL")]
            EXTERNAL = 1,
            [StringValue("A", "PROD'N OF API")]
            AIKAWA = 2,
            [StringValue("S", "SUBCON")]
            SUBCON = 3
        }
        public enum UsageType
        {
            [StringValue("C")]
            COMMON = 0,
            [StringValue("D")]
            DEDICATED = 1
        }

        public enum CriticalBalanceCategory
        {
            [StringValue("L")]
            LOW = 0,
            [StringValue("M")]
            MID = 1,
            [StringValue("H")]
            HIGH = 2
        }

        public enum Shipping
        {
            [StringValue("AIR", "AIR")]
            AIR = 0,
            [StringValue("LAND", "LAND")]
            LAND = 1,
            [StringValue("SEA", "SEA")]
            SEA = 2,
            [StringValue("HAND-CARRY", "HAND-CARRY")]
            HAND_CARRY = 3
        }

        public enum PartsGen
        {
            [StringValue("T", "GENERATED")]
            GENERATED = 0,
            [StringValue("N", "NOT YET GENERATED")]
            NOT_YET_GENERATED = 1
        }

        public enum GenericType
        {
            [StringValue("H", "HUMIDITY")]
            HUMIDITY = 0,
            [StringValue("P", "PILING HEIGHT")]
            PILING_HEIGHT = 1,
            [StringValue("S", "SI Unit")]
            Si_Unit = 2,
            [StringValue("B", "SB Unit")]
            Sb_Unit = 3,
            [StringValue("W", "WARM")]
            WARM = 4
        }

        public enum CorrectiveActConfirm
        {
            [StringValue("1")]
            Accepted = 1,
            [StringValue("0")]
            Rejected = 0
        }

        public enum CorrectiveActRequire
        {
            [StringValue("0")]
            ImmediateCorrectiveAction = 0,
            [StringValue("1")]
            FinalCorrectiveAction = 1
        }

        public enum BOMType
        {
            [StringValue("S", "SALES PRODUCT")]
            S = 0,
            [StringValue("M", "MANUFACTURING PROCESS")]
            M = 1,
            [StringValue("J", "PROCESS WITH OUTPUT STOCK")]
            J = 2,
            [StringValue("C", "COMPONENT")]
            C = 3,
            [StringValue("I", "INVENTORIALBLE MATERIAL")]
            I = 4,
            [StringValue("B", "COMPONENT OR SALES PRODUCT")]
            B = 5
        }

        public enum Terms
        {
            [StringValue("15days")]
            FifteenDays = 0,
            [StringValue("30days")]
            ThirtyDays = 1,
            [StringValue("45days")]
            FortyFiveDays = 2,
            [StringValue("60days")]
            SixtyDays = 3
        }

        public enum GenericCBType
        {
            [StringValue("USER_MASTER")]
            USER_MASTER = 0,
            [StringValue("CURRENCY")]
            CURRENCY = 1,
            [StringValue("UNIT_OF_MEASURE")]
            UNIT_OF_MEASURE = 2,
            [StringValue("STOCK_TYPE")]
            STOCK_TYPE = 3,
            [StringValue("STATUS")]
            STATUS = 4,
            [StringValue("STATUS_NC")]
            STATUS_NC = 5,
            [StringValue("STATUS_AC")]
            STATUS_AC = 6,
            [StringValue("STATUS_AOC")]
            STATUS_AOC = 7,
            [StringValue("SHIPPING")]
            SHIPPING = 8,
            [StringValue("COR_ACT_STAT")]
            COR_ACT_STAT = 9,
            [StringValue("ORIGIN")]
            ORIGIN = 10,
            [StringValue("USAGE_TYPE")]
            USAGE_TYPE = 11,
            [StringValue("BALANCE_CATEGORY")]
            BALANCE_CATEGORY = 12,
            [StringValue("DAYS")]
            DAYS = 13,
            [StringValue("CORRECTIVE_ACTION")]
            CORRECTIVE_ACTION = 14,
            [StringValue("CRITERIA")]
            CRITERIA = 15,
            [StringValue("BOM_TYPE")]
            BOM_TYPE = 16,
            [StringValue("GENERIC_TYPE")]
            GENERIC_TYPE = 17,
            [StringValue("PARTS_GEN")]
            PARTS_GEN = 18,
            [StringValue("STATUS_NFOC")]
            STATUS_NFOC = 19,
            [StringValue("STATUS_ANC")]
            STATUS_ANC = 20,
            [StringValue("STATUS_AOCF")]
            STATUS_AOCF = 21,
            [StringValue("PRODUCTS")]
            PRODUCTS = 22,
            [StringValue("TERMS")]
            TERMS = 23,
            [StringValue("STATUS")]
            STATUS1 = 24,
            [StringValue("ACCT_STATUS")]
            ACCT_STATUS = 25,
            [StringValue("STATUS_INV")]
            STATUS_INV = 26,
            [StringValue("STATUS_STKTRNS")]
            STATUS_STKTRNS = 27,
            [StringValue("INVENTORY_CLASS")]
            INVENTORY_CLASS = 28,
            [StringValue("LOCATOR_TYPE")]
            LOCATOR_TYPE = 29,
            [StringValue("LOCATOR_AREA")]
            LOCATOR_AREA = 30,
            [StringValue("PRSTATUS")]
            PRSTATUS = 31,
            [StringValue("ASSET_TYPE")]
            ASSET_TYPE = 32,
            [StringValue("STOCK_STORAGE")]
            STOCK_STORAGE = 33,
            [StringValue("SPO_STATUS")]
            SPO_STATUS = 34,
            [StringValue("RR_TYPE")]
            RR_TYPE = 35,
            [StringValue("RR_TYPE_SC")]
            RR_TYPE_SC = 36,
            [StringValue("RR_TYPE_EDX")]
            RR_TYPE_EDX = 37,
            [StringValue("FEED_TYPE")]
            FEED_TYPE = 38,
            [StringValue("STATUS_NCAF")]
            STATUS_NCAF = 39,
            [StringValue("STATUS_NCAU")]
            STATUS_NCAU = 40,
            [StringValue("Material_Status")]
            Material_Status = 41,
            [StringValue("ORIGINCSM")]
            ORIGINCSM = 42,
            [StringValue("ORIGINAPI")]
            ORIGINAPI = 43,
            [StringValue("AFCO_STATUS")]
            AFCO_STATUS = 44,
            [StringValue("SALES_TYPE")]
            SALES_TYPE = 45,
            [StringValue("CUST_TYPE")]
            CUST_TYPE = 46,
            [StringValue("TRANSFER_TYPE")]
            TRANSFER_TYPE = 47,
            [StringValue("Material_Type")]
            Material_Type = 48,
            [StringValue("Product_Category")]
            Product_Category = 49,
            [StringValue("Source_Product")]
            Source_Product = 50,
            [StringValue("CPO_POST_STATUS")]
            CPO_POST_STATUS = 51,
            [StringValue("PACK_TYPE")]
            PACK_TYPE = 52,
            [StringValue("STATUS_AFC")]
            STATUS_AFC = 53,
            [StringValue("PS_GROUP")]
            PS_GROUP = 54,
            [StringValue("ADJUSTMENT_REASON")]
            ADJUSTMENT_REASON = 55,
            [StringValue("PAYMENT_TERMS")]
            PAYMENT_TERMS = 56,
            [StringValue("DELIVERY_TERMS")]
            DELIVERY_TERMS = 57,
            [StringValue("TRANSPORT_CATEGORY")]
            TRANSPORT_CATEGORY = 58,
            [StringValue("STATUS_AF")]
            STATUS_AF = 59,
            [StringValue("CRITICAL_BALANCE")]
            CRITICAL_BALANCE = 60,
            [StringValue("JOType")]
            JOType = 61,
            [StringValue("JOSource")]
            JOSource = 62,
            [StringValue("Lead_Time_Unit")]
            Lead_Time_Unit = 63,
            [StringValue("Down_Time")]
            Down_Time = 64,
            //ken            
            [StringValue("DEFECTS_CATEGORY")]
            DEFECTS_CATEGORY = 65,
            [StringValue("LOCATOR_TYPE_MB")]
            LOCATOR_TYPE_MB = 66
        }

        public enum LeadTimeUnit
        {
            [StringValue("H", "HOUR/S")]
            HOURS = 0,
            [StringValue("D", "DAY/S")]
            DAY = 1
        }

        public enum JOType
        {
            [StringValue("R")]
            REGULAR = 0,
            [StringValue("S")]
            SPECIAL = 1
        }
        public enum JOSource
        {
            [StringValue("A", "FROM ACTUAL BUILDABLE")]
            FROM_ACTUAL_BUILDABLE = 0,
            [StringValue("P", "FROM PLAN BUILDABLE")]
            FROM_PLAN_BUILDABLE = 1
        }

        public enum RRType
        {
            [StringValue("RS")]
            FROM_SUPPLIER = 0,
            [StringValue("RC")]
            FROM_CUSTOMER = 1,
            [StringValue("RE")]
            EXCESS_MATERIAL = 2,
            [StringValue("RD")]
            DISMANTLED_MATERIAL = 3,
            [StringValue("RX")]
            SALEABLE_SCRAP = 4,
            [StringValue("RF")]
            FINISH_GOODS = 5,
            [StringValue("RO")]
            OEM_RR = 6
        }

        public enum RRTypeFuji
        {
            [StringValue("RC")]
            FOR_CHIPS = 0,
            [StringValue("RO")]
            CHIPS_OEM = 1,
            [StringValue("RS")]
            RAW_MATERIALS = 2
        }

        public enum InventoryClass
        {
            [StringValue("W")]
            WAREHOUSING = 0,
            [StringValue("N")]
            NON_WAREHOUSING = 1
        }
        public enum ASSET_TYPE
        {
            [StringValue("F")]
            FIXED_ASSET = 0,
            [StringValue("N")]
            NON_FIXED_ASSET = 1
        }

        public enum BOMNodeType
        {
            [StringValue("PRODUCT")]
            PRODUCT = 0,
            [StringValue("PROCESS")]
            PROCESS = 1,
            [StringValue("STOCK")]
            STOCK = 2
        }

        public enum LoginFormType
        {
            NORMAL_LOGIN = 0,
            LOG_OUT = 1,
            LOCK = 2
        }

        public enum LocatorType
        {
            [StringValue("R")]
            RACK = 0,
            [StringValue("P")]
            PALLET = 1,
            [StringValue("S")]
            STOCKROOM = 2,
            [StringValue("C")]
            CABINET = 3,
            [StringValue("M")]
            MACHINE = 4
        }

        public enum Material_Status
        {
            [StringValue("G")]
            GENERATING = 0,
            [StringValue("N")]
            NOT_GENERATING = 1
        }

        public enum LocatorArea
        {
            [StringValue("C")]
            CART = 0,
            [StringValue("P")]
            PRODUCTION = 1,
            [StringValue("W")]
            WAREHOUSE = 2
        }

        public enum CodeConcatenation
        {
            [StringValue("-")]
            HYPHEN = 0
        }
        public enum FeedType
        {
            [StringValue("PF", "PER PIECE FEED")]
            PF = 0,
            [StringValue("CF", "CONTINOUS FEED")]
            CF = 1
        }

        public enum SupplierType
        {
            [StringValue("API")]
            API = 0,
            [StringValue("CSM")]
            CSM = 1
        }

        public enum WRISType
        {
            [StringValue("08", "Return To Supplier")]
            RetToSup = 08,
            [StringValue("09", "Return To Customer")]
            RetToCust = 09
        }

        public enum SalesType
        {
            [StringValue("D", "DIRECT SALES")]
            D = 0,
            [StringValue("T", "TRIANGULAR SALES")]
            T = 1,
            [StringValue("M", "MOTHER COMPANY SALES")]
            M = 2
            //end
            ,
        }

        public enum CustType
        {
            [StringValue("L", "LOCAL")]
            L = 0,
            [StringValue("E", "EXPORT")]
            E = 1
        }

        public enum MaterialType
        {
            [StringValue("M", "RAW MATERIAL")]
            M = 0,
            [StringValue("P", "PACKING MATERIAL")]
            P = 1
        }

        public enum CPOType
        {
            [StringValue("C")]
            OFFICIAL_PO = 0,
            [StringValue("N")]
            NOTIFICATION_PO = 1
        }

        public enum Product_Category
        {
            [StringValue("M")]
            Mass_Production = 0,
            [StringValue("P")]
            Pre_Production = 1
        }

        public enum Source_Product
        {
            [StringValue("M")]
            Manufacturing = 0,
            [StringValue("S")]
            Scrap = 1
        }

        public enum TRANSFERTYPE
        {
            [StringValue("RS", "RETURN OF STOCKS TO SUPPLIER")]
            RS = 0,
            [StringValue("SP", "STOP PRODUCTION")]
            SP = 1,
            [StringValue("WW", "TRANSFER TO ASSIGNED WAREHOUSE")]
            WW = 2
        }
        public enum TransferCode
        {
            [StringValue("TW")]
            TW = 0,
            [StringValue("ST")]
            ST = 1
        }

        public enum PackType
        {
            [StringValue("B", "Box")]
            B = 0,
            [StringValue("T", "Tray")]
            T = 1,
            [StringValue("P", "Plastic")]
            P = 2
        }

        public enum AdjustmentReason
        {
            [StringValue("L", "Lacking Quantity")]
            L = 0,
            [StringValue("D", "Defective Product")]
            D = 1
        }

        public enum TransportCategory
        {
            [StringValue("C")]
            COMPANY = 0,
            [StringValue("R")]
            RENTAL = 1
        }

        public enum CriticalBalance
        {
            [StringValue("ALL", "ALL")]
            ALL = 0,
            [StringValue("LESS", "Lesser than (< the Critical Balance [Locator - Hold Qty])")]
            LESS = 1,
            [StringValue("LESSEQUAL", "Lesser than or equal (<= the Critical Balance [Locator - Hold Qty])")]
            LESSEQUAL = 2
        }

        public enum DefualtLocator
        {
            [StringValue("000000")]
            MAINWAREHOUSE = 0,
            [StringValue("000000")]
            SUMITRANS = 1,
            [StringValue("FWH000")]
            FABRICATION = 2,
            [StringValue("EWH000")]
            ENGINEERING = 3
        }

        public enum Warehouse
        {
            [StringValue("000")]
            MAINWAREHOUSE = 0,
            [StringValue("001")]
            SUMITRANS = 1,
            [StringValue("002")]
            FABRICATION = 2,
            [StringValue("003")]
            ENGINEERING = 3
        }

        public enum DownTime
        {
            [StringValue("MC")]
            MACHINE = 0,
            [StringValue("ME")]
            METHOD = 1,
            [StringValue("MA")]
            MATERIAL = 2
        }

        public enum TravelogProcessDetails
        {
            DETAIL_DOWNTIME = 0,
            DETAIL_DEFECTS = 1,
            DETAIL_MANPOWER = 2,
            DETAIL_MACHINE = 3,
            DETAIL_DIESET = 4,
            DETAIL_MATERIAL = 5
        }
    }
}
