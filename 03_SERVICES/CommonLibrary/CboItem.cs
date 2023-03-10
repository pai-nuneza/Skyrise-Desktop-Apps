using System;
using System.Collections.Generic;
using System.Text;

namespace CommonPostingLibrary
{
    public class CboItem
    {
        private string displayMember;
        private string valueMember;

        public string DisplayMember
        {
            get
            {
                return this.displayMember;
            }
            set
            {
                this.displayMember = value;
            }
        }

        public string ValueMember
        {
            get
            {
                return this.valueMember;
            }
            set
            {
                this.valueMember = value;
            }
        }
    }
}
