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
        Add = 8,

        And = 1,

        Divide = 11,

        Equal = 2,

        GreaterThan = 4,

        GreaterThanOrEqual = 5,

        LessThan = 6,

        LessThanOrEqual = 7,

        Modulo = 12,

        Multiply = 10,

        NotEqual = 3,

        Or = 0,

        Substract = 9
    }
}
