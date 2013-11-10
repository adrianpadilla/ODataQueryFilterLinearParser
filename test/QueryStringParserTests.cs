using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using Microsoft.Data.OData.Query;

namespace AdrianPadilla.Net.OData.QueryFilterLinearParser.Test
{
    [TestClass]
    public class QueryStringParserTests
    {

        public static ODataQueryContext CompaniesContext { get; private set; }

        [ClassInitialize]
        public static void PrepareContexts(TestContext context)
        {
            CompaniesContext = new ODataQueryContext(ContextBuildingHelpers.GetCompaniesModel(),
                typeof(Company));

        }

        private QueryInformation GetQueryInformation(string queryFilter, ODataQueryContext context)
        {
            var filter = new FilterQueryOption(queryFilter, CompaniesContext);
            var info = new QueryInformation();
            QueryStringParser.ParseFilterData(filter, info);

            return info;

        }

        [TestMethod]
        public void SimpleEqualsFilter()
        {
            // Arrange
            string query = "Name eq 'Microsoft'";

            // Act
            var info = GetQueryInformation(query, CompaniesContext);

            // Assert
            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Name"), "The field collection doesn't contain an element with Field Name 'Name'.");
            var parameter = info.FilterParameters.Find(p => p.FieldName == "Name");
            Assert.AreEqual<string>("Microsoft", parameter.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<BinaryOperatorKind>(BinaryOperatorKind.Equal, parameter.FilteringOperator, "The filtering operator is not the expected.");

        }

        [TestMethod]
        public void TwoSimpleEqualFilters()
        {
            // Arrange
            string query = "Name eq 'Microsoft' and Industry eq 'Computer Software'";

            // Act
            var info = GetQueryInformation(query, CompaniesContext);

            // Assert
            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Name"), "The field collection doesn't contain an element with Field Name 'Name'.");
            var parameter = info.FilterParameters.Find(p => p.FieldName == "Name");
            Assert.AreEqual<string>("Microsoft", parameter.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<BinaryOperatorKind>(BinaryOperatorKind.Equal, parameter.FilteringOperator, "The filtering operator is not the expected.");

            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Industry"), "The field collection doesn't contain an element with Field Name 'Industry'.");
            var parameter2 = info.FilterParameters.Find(p => p.FieldName == "Industry");
            Assert.AreEqual<string>("Computer Software", parameter2.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<BinaryOperatorKind>(BinaryOperatorKind.Equal, parameter2.FilteringOperator, "The filtering operator is not the expected.");

        }

        [TestMethod]
        public void ThreeSimpleEqualFilters()
        {
            // Arrange
            string query = "Name eq 'Microsoft' and Industry eq 'Computer Software' or Headquarters eq 'Redmond, WA'";

            // Act
            var info = GetQueryInformation(query, CompaniesContext);

            // Assert
            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Name"), "The field collection doesn't contain an element with Field Name 'Name'.");
            var parameter = info.FilterParameters.Find(p => p.FieldName == "Name");
            Assert.AreEqual<string>("Microsoft", parameter.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<BinaryOperatorKind>(BinaryOperatorKind.Equal, parameter.FilteringOperator, "The filtering operator is not the expected.");

            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Industry"), "The field collection doesn't contain an element with Field Name 'Industry'.");
            var parameter2 = info.FilterParameters.Find(p => p.FieldName == "Industry");
            Assert.AreEqual<string>("Computer Software", parameter2.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<BinaryOperatorKind>(BinaryOperatorKind.Equal, parameter2.FilteringOperator, "The filtering operator is not the expected.");

            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Headquarters"), "The field collection doesn't contain an element with Field Name 'Headquarters'.");
            var parameter3 = info.FilterParameters.Find(p => p.FieldName == "Headquarters");
            Assert.AreEqual<string>("Redmond, WA", parameter3.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<BinaryOperatorKind>(BinaryOperatorKind.Equal, parameter3.FilteringOperator, "The filtering operator is not the expected.");
        }

        [TestMethod]
        public void NumericFilter()
        {
            // Arrange
            string query = "Revenue eq 5000";

            // Act
            var info = GetQueryInformation(query, CompaniesContext);

            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Revenue"), "The field collection doesn't contain an element with Field Name 'Revenue'.");
            var parameter = info.FilterParameters.Find(p => p.FieldName == "Revenue");
            Assert.AreEqual<string>("5000", parameter.StringValue);

        }
    }
}
