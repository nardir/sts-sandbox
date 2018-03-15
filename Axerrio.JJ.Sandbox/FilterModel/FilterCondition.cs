using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.JJ.Sandbox
{
    public class FilterCondition
    {
        public FilterCondition(string propertyName, string @operator, params object[] filterValues)
        {
            PropertyName = propertyName;
            Operator  = @operator;
            FilterValues = filterValues;
        }
        public string PropertyName { get; set; }    //FilterProperty.Name
        public string Operator { get; set; }        //FilterOperator.Expression oid.
        public object[] FilterValues { get; set; }

    }
}
