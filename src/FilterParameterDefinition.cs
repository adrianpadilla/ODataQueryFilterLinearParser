using Microsoft.Data.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdrianPadilla.Net.OData.QueryFilterLinearParser
{
    /// <summary>
    /// Represents one filtering unit parsed from an Odata query.
    /// </summary>
    public class FilterParameterDefinition
    {
        /// <summary>
        /// The name of the field the user is trying to filter on.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// String representation of the value that is being looked for.
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// The operator of the filtering or comparison being done by this parameter.
        /// </summary>
        public FilteringOperator FilteringOperator { get; set; }

        /// <summary>
        /// Modifier applied to the field, such as toUpper, etc.
        /// </summary>
        public string Modifiers { get; set; }

    }
}
