using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdrianPadilla.Net.OData.QueryFilterLinearParser.Test
{
    /// <summary>
    /// Represents an entity for the test model. A Company entry.
    /// </summary>
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        public string Name { get; set; }

        public string Industry { get; set; }

        public string Founders { get; set; }

        public DateTime FoundingDate { get; set; }

        public string Headquarters { get; set; }

        public decimal Revenue { get; set; }

        public int NumberOfEmployees { get; set; }

    }
}
