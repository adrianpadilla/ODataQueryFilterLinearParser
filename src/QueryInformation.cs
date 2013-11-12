using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdrianPadilla.Net.OData.QueryFilterLinearParser
{
    /// <summary>
    /// Represents the parsed information about an OData query string.
    /// </summary>
    public class QueryInformation
    {
        /// <summary>
        /// Collection of parsed parameter definitions.
        /// </summary>
        public ODataFilterParameterCollection FilterParameters { get; set; }
    }
}
