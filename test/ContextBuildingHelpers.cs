using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData.Builder;

namespace AdrianPadilla.Net.OData.QueryFilterLinearParser.Test
{
    public static class ContextBuildingHelpers
    {
        public static IEdmModel GetCompaniesModel()
        {
            ODataModelBuilder model = new ODataModelBuilder();

            RegisterCompanyEntity(model);

            RegiterCompaniesEntitySet(model);

            return model.GetEdmModel();
        }

        private static void RegisterCompanyEntity(ODataModelBuilder model)
        {
            var company = model.Entity<Company>();

            company.HasKey<int>(c => c.CompanyId);
            company.Property(c => c.Name);
            company.Property(c => c.Industry);
            company.Property(c => c.Founders);
            company.Property(c => c.FoundingDate);
            company.Property(c => c.Headquarters);
            company.Property(c => c.Revenue);
            company.Property(c => c.NumberOfEmployees);
        }


        private static void RegiterCompaniesEntitySet(ODataModelBuilder model)
        {
            model.EntitySet<Company>("Companies");
        }
    }

    
}
