using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdrianPadilla.Net.OData.QueryFilterLinearParser
{
    /// <summary>
    /// Collection of filter parameters parsed from an OData querystring
    /// </summary>
    public class ODataFilterParameterCollection : List<FilterParameterDefinition> //Dictionary<string, FilterParameterDefinition>
    {
        // This was originally a dictionary, but in order to support OR filters containing the same
        // name, I switched it to a list -- APV
    }
}
