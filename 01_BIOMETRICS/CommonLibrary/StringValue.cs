using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public class StringValueAttribute: System.Attribute
    {
        private string value;
        private string display;

        public StringValueAttribute(string value) : this(value, "") { }

        public StringValueAttribute(string value, string display)
        {
            this.value = value;
            this.display = display;
        }

        public string Value
        {
            get
            {
                return this.value;
            }
        }

        public string Display
        {
            get
            {
                return this.display;
            }
        }
    }
   
}
