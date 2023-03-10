using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    [Serializable]
    ////reynard 20090108
    public class PayrollException : Exception
    {
        private bool logError;
        private Exception exception;
        public PayrollException() { }
        public PayrollException(string message) : this(message, false)
        { 
            
        }
        public PayrollException(string message, bool logError): base(message)
        {
            this.logError = logError;
        }
        public PayrollException(Exception e): this(e, false)
        {
        }
        public PayrollException(Exception e, bool logError): this(e.Message, logError)
        {
            this.exception = e;
        }
        public bool LogError
        {
            get
            {
                return logError;
            }
        }
    }    
    ////end
    public class ConnectionChangeException : Exception
    {
        private bool logError;
        private Exception exception;

        public ConnectionChangeException() { }
        public ConnectionChangeException(string message)
            : this(message, false)
        {

        }
        public ConnectionChangeException(string message, bool logError)
            : base(message)
        {
            this.logError = logError;
        }
        public ConnectionChangeException(Exception e)
            : this(e, false)
        {
        }
        public ConnectionChangeException(Exception e, bool logError)
            : this(e.Message, logError)
        {
            this.exception = e;
        }

        public bool LogError
        {
            get
            {
                return logError;
            }
        }
    }
    public class InventoryException : Exception
    {
        private bool logError;
        private Exception exception;

        public InventoryException() { }
        public InventoryException(string message)
            : this(message, false)
        {

        }
        public InventoryException(string message, bool logError)
            : base(message)
        {
            this.logError = logError;
        }
        public InventoryException(Exception e)
            : this(e, false)
        {
        }
        public InventoryException(Exception e, bool logError)
            : this(e.Message, logError)
        {
            this.exception = e;
        }

        public bool LogError
        {
            get
            {
                return logError;
            }
        }
    }
}
