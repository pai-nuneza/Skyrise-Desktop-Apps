using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public class LookupField
    {
        private string _strDBFieldName;
        private string _strHeaderName;
        private bool _bIsReturnable;
        private bool _bIsFindText;

        public string strDBFieldName
        {
            get { return _strDBFieldName; }
            set { _strDBFieldName = value; }
        }

        public string strHeaderName
        {
            get { return _strHeaderName; }
            set { _strHeaderName = value; }
        }

        public bool bIsReturnable
        {
            get { return _bIsReturnable; }
            set { _bIsReturnable = value; }
        }

        public bool bIsFindText
        {
            get { return _bIsFindText; }
            set { _bIsFindText = value; }
        }
        

        public LookupField()
        {
        }

        public LookupField(string fieldName) : this(fieldName, false) {}

        public LookupField(string fieldName, bool isReturnable) : this(fieldName, fieldName, isReturnable) {}

        public LookupField(string fieldName, string headerName, bool isReturnable) : this(fieldName, headerName, isReturnable, false) {}

        public LookupField(string fieldName, string headerName, bool isReturnable, bool isFindText)
        {
            this.strDBFieldName = fieldName;
            this.strHeaderName = headerName;
            this.bIsReturnable = isReturnable;
            this.bIsFindText = isFindText;
        }
    }
}
