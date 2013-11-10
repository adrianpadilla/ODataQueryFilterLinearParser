using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdrianPadilla.Net.OData.QueryFilterLinearParser
{
    /// <summary>
    /// The operator that is being applied for filtering.
    /// </summary>
    public enum FilteringOperator
    {
        Equals = 0,

        GreaterThan = 1,

        LessThan = 2
    }
}
