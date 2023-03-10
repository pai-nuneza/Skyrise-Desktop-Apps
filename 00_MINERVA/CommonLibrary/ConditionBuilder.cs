using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public class ConditionBuilder
    {
        StringBuilder conBuilder;

        public ConditionBuilder()
        {
            conBuilder = new StringBuilder();
        }

        #region public

        public void AddCondition(string fieldName, string value, bool includeQuotesInValue, CommonEnum.ConditionalOperators conditionOperator)
        {
            this.conBuilder.Append(fieldName);
            this.conBuilder.Append(" ");
            this.conBuilder.Append(StringEnum.GetStringValue(conditionOperator));
            this.conBuilder.Append(" ");

            if (conditionOperator == CommonEnum.ConditionalOperators.LIKE)
            {
                this.conBuilder.Append("'%");
                this.conBuilder.Append(value);
                this.conBuilder.Append("%'");
            }
            else if (includeQuotesInValue)
            {
                this.conBuilder.Append("'");
                this.conBuilder.Append(value);
                this.conBuilder.Append("'");
            }
            else
                this.conBuilder.Append(value);
        }

        public void AddLogicalOperator(CommonEnum.LogicalOperators logicalOperator)
        {
            this.conBuilder.Append(" ");
            this.conBuilder.Append(StringEnum.GetStringValue(logicalOperator));
            this.conBuilder.Append(" ");
        }

        public void AddGroupBegin()
        {
            this.conBuilder.Append(" ");
            this.conBuilder.Append("(");
        }

        public void AddGroupEnd()
        {
            this.conBuilder.Append(")");
            this.conBuilder.Append(" ");
        }

        public string Generate()
        {
            string finalCondition = conBuilder.ToString();

            if (finalCondition.EndsWith(" AND ") || finalCondition.EndsWith(" OR "))
                finalCondition = finalCondition.Substring(0, finalCondition.Length - 4);
            
            return finalCondition;
        }

        #endregion//public
    }
}
