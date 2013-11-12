using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using Microsoft.Data.OData.Query;
using System.Linq;
using System.Text;
using System.Collections.Generic;

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

        private QueryInformation GetQueryInformation(string queryFilter, ODataQueryContext context, out QueryStringParser parser)
        {
            var filter = new FilterQueryOption(queryFilter, CompaniesContext);
            var info = new QueryInformation();
            parser = new QueryStringParser();
            parser.ParseFilterData(filter, info);

            return info;

        }

        private static void AssertNoWarnings(QueryStringParser parser)
        {
            Assert.IsNull(parser.Warnings, "The warning collection is not null as expected.");
        }

        private static void AssertNoExceptions(QueryStringParser parser)
        {
            Assert.IsNull(parser.Exceptions, "The exception collection is not null as expecteed.");
        }

        private static void AssertNoWarningsOrExceptions(QueryStringParser parser)
        {
            AssertNoWarnings(parser);
            AssertNoExceptions(parser);
        }

        [TestMethod]
        public void SimpleEqualsFilter()
        {
            // Arrange
            string query = "Name eq 'Microsoft'";
            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Name"), "The field collection doesn't contain an element with Field Name 'Name'.");
            var parameter = info.FilterParameters.Find(p => p.FieldName == "Name");
            Assert.AreEqual<string>("Microsoft", parameter.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<FilteringOperator>(FilteringOperator.Equal, parameter.FilteringOperator, "The filtering operator is not the expected.");
            AssertNoWarningsOrExceptions(parser);

        }

        [TestMethod]
        public void TwoSimpleEqualFilters()
        {
            // Arrange
            string query = "Name eq 'Microsoft' and Industry eq 'Computer Software'";
            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Name"), "The field collection doesn't contain an element with Field Name 'Name'.");
            var parameter = info.FilterParameters.Find(p => p.FieldName == "Name");
            Assert.AreEqual<string>("Microsoft", parameter.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<FilteringOperator>(FilteringOperator.Equal, parameter.FilteringOperator, "The filtering operator is not the expected.");

            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Industry"), "The field collection doesn't contain an element with Field Name 'Industry'.");
            var parameter2 = info.FilterParameters.Find(p => p.FieldName == "Industry");
            Assert.AreEqual<string>("Computer Software", parameter2.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<FilteringOperator>(FilteringOperator.Equal, parameter2.FilteringOperator, "The filtering operator is not the expected.");

            AssertNoWarningsOrExceptions(parser);

        }

        [TestMethod]
        public void ThreeSimpleEqualFilters()
        {
            // Arrange
            string query = "Name eq 'Microsoft' and Industry eq 'Computer Software' or Headquarters eq 'Redmond, WA'";
            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Name"), "The field collection doesn't contain an element with Field Name 'Name'.");
            var parameter = info.FilterParameters.Find(p => p.FieldName == "Name");
            Assert.AreEqual<string>("Microsoft", parameter.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<FilteringOperator>(FilteringOperator.Equal, parameter.FilteringOperator, "The filtering operator is not the expected.");

            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Industry"), "The field collection doesn't contain an element with Field Name 'Industry'.");
            var parameter2 = info.FilterParameters.Find(p => p.FieldName == "Industry");
            Assert.AreEqual<string>("Computer Software", parameter2.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<FilteringOperator>(FilteringOperator.Equal, parameter2.FilteringOperator, "The filtering operator is not the expected.");

            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Headquarters"), "The field collection doesn't contain an element with Field Name 'Headquarters'.");
            var parameter3 = info.FilterParameters.Find(p => p.FieldName == "Headquarters");
            Assert.AreEqual<string>("Redmond, WA", parameter3.StringValue, "The value of the parameter is not the one expected.");
            Assert.AreEqual<FilteringOperator>(FilteringOperator.Equal, parameter3.FilteringOperator, "The filtering operator is not the expected.");

            AssertNoWarningsOrExceptions(parser);

        }

        [TestMethod]
        public void NumericFilter()
        {
            // Arrange
            string query = "Revenue eq 5000";
            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Revenue"), "The field collection doesn't contain an element with Field Name 'Revenue'.");
            var parameter = info.FilterParameters.Find(p => p.FieldName == "Revenue");
            Assert.AreEqual<string>("5000", parameter.StringValue);
            AssertNoWarningsOrExceptions(parser);

        }

        [TestMethod]
        public void TwoNumericFilters()
        {
            // Arrange
            string query = "Revenue eq 500000 and NumberOfEmployees eq 100";
            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            Assert.IsTrue(info.FilterParameters.Exists(p => p.FieldName == "Revenue"), "The field collection doesn't contain an element with Field Name 'Revenue'.");
            var parameter = info.FilterParameters.Find(p => p.FieldName == "Revenue");
            Assert.AreEqual<string>("500000", parameter.StringValue);
            AssertNoWarningsOrExceptions(parser);

        }

        /// <summary>
        /// Tests for repeated parameters. Although OR or AND operators are not supported
        /// we need to support the query containing them. Whatever the source of data is must
        /// be deal with repeated parameters the way they decide.
        /// </summary>
        [TestMethod]
        public void RepeatedParameters()
        {
            // Arrange
            StringBuilder builder = new StringBuilder();
            builder.Append("Name eq 'Microsoft' "); 
            builder.Append("and Name eq 'Google' ");
            builder.Append("and Name eq 'Amazon'");
            string query = builder.ToString();

            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            AssertNoWarningsOrExceptions(parser);

            var parameters = (from p in info.FilterParameters
                             where p.FieldName == "Name"
                             select p).ToList<FilterParameterDefinition>();

            Assert.AreEqual<int>(3, parameters.Count<FilterParameterDefinition>(), "The expected number of parameter elements wasn't found.");
            Assert.IsTrue(parameters.Exists(p => p.StringValue == "Microsoft"), "The expected filter value 'Microsoft' wasn't found.'");
            Assert.IsTrue(parameters.Exists(p => p.StringValue == "Google"), "The expected filter value 'Google' wasn't found.'");
            Assert.IsTrue(parameters.Exists(p => p.StringValue == "Amazon"), "The expected filter value 'Amazon' wasn't found.'");          
        }

        /// <summary>
        /// Tests parameters with filter 'greater than' are parsed correctly.
        /// </summary>
        [TestMethod]
        public void GreaterThanParameter()
        {
            // Arrange
            string query = "Revenue gt 10000";

            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            Predicate<FilterParameterDefinition> matchRevenue = p => p.FieldName == "Revenue";
            AssertNoWarningsOrExceptions(parser);
            Assert.IsTrue(info.FilterParameters.Exists(matchRevenue));
            Assert.AreEqual<int>(1, info.FilterParameters.FindAll(matchRevenue).Count);
            var parameter = info.FilterParameters.Find(matchRevenue);
            Assert.AreEqual<FilteringOperator>(FilteringOperator.GreaterThan, parameter.FilteringOperator, "The filtering operator was not the type expected.");
        }

        /// <summary>
        /// Tests parameters with filter 'less than' are parsed correctly.
        /// </summary>
        [TestMethod]
        public void LessThanParameter()
        {
            // Arrange
            string query = "Revenue lt 10000";

            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            Predicate<FilterParameterDefinition> matchRevenue = p => p.FieldName == "Revenue";
            AssertNoWarningsOrExceptions(parser);
            Assert.IsTrue(info.FilterParameters.Exists(matchRevenue));
            Assert.AreEqual<int>(1, info.FilterParameters.FindAll(matchRevenue).Count);
            var parameter = info.FilterParameters.Find(matchRevenue);
            Assert.AreEqual<FilteringOperator>(FilteringOperator.LessThan, parameter.FilteringOperator, "The filtering operator was not the type expected.");
        }

        /// <summary>
        /// Tests parameters with filter 'greater or equal than' are parsed correctly.
        /// </summary>
        [TestMethod]
        public void GreaterOrEqualThanParameter()
        {
            // Arrange
            string query ="Revenue ge 10000";

            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            Predicate<FilterParameterDefinition> matchRevenue = p => p.FieldName == "Revenue";
            AssertNoWarningsOrExceptions(parser);
            Assert.IsTrue(info.FilterParameters.Exists(matchRevenue));
            Assert.AreEqual<int>(1, info.FilterParameters.FindAll(matchRevenue).Count);
            var parameter = info.FilterParameters.Find(matchRevenue);
            Assert.AreEqual<FilteringOperator>(FilteringOperator.GreaterThanOrEqual, parameter.FilteringOperator, "The filtering operator was not the type expected.");
        }

        /// <summary>
        /// Tests parameters with filter 'less or equal than' are parsed correctly.
        /// </summary>
        [TestMethod]
        public void LessOrEqualThanParameter()
        {
            // Arrange
            string query = "Revenue le 10000";

            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            Predicate<FilterParameterDefinition> matchRevenue = p => p.FieldName == "Revenue";
            AssertNoWarningsOrExceptions(parser);
            Assert.IsTrue(info.FilterParameters.Exists(matchRevenue));
            Assert.AreEqual<int>(1, info.FilterParameters.FindAll(matchRevenue).Count);
            var parameter = info.FilterParameters.Find(matchRevenue);
            Assert.AreEqual<FilteringOperator>(FilteringOperator.LessThanOrEqual, parameter.FilteringOperator, "The filtering operator was not the type expected.");
        }

        [TestMethod]
        public void ToLower()
        {
            // Arrange
            string query = "tolower(Name) eq 'microsoft'";

            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            Predicate<FilterParameterDefinition> matchRevenue = p => p.FieldName == "Name";
            AssertNoWarningsOrExceptions(parser);
            Assert.IsTrue(info.FilterParameters.Exists(matchRevenue), "The expected filter parameter doesnt exist.");
            Assert.AreEqual<int>(1, info.FilterParameters.FindAll(matchRevenue).Count);
            var parameter = info.FilterParameters.Find(matchRevenue);
            Assert.AreEqual<FilteringOperator>(FilteringOperator.Equal, parameter.FilteringOperator, "The filtering operator was not the type expected.");
            Assert.AreEqual<string>("tolower", parameter.Modifiers, "The modifier isn't the one expected.");

        }


        /// <summary>
        /// This scenario is not currently supported. We just need to ensure
        /// an exception is not thrown.
        /// </summary>
        [TestMethod]
        public void UsingToStringOfToLowerNoException()
        {
            // Arrange
            string query = "substringof('soft',tolower(Name)) eq true";

            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            // Exception wasn't thrown
        }


        /// <summary>
        /// This scenario is not currently supported. We just need to ensure
        /// this warning of this parameter being skipped is logged.
        /// </summary>
        [TestMethod]
        [TestCategory("NotResolved")]
        [Ignore]
        public void UsingToStringOfToLower()
        {
            // Arrange
            string query = "substringof('soft',tolower(Name)) eq true";

            QueryStringParser parser = null;

            // Act
            var info = GetQueryInformation(query, CompaniesContext, out parser);

            // Assert
            Assert.AreEqual<int>(1, parser.Warnings.Count, "The parser didn't file the warning as expected.");
            Assert.AreEqual<string>("The parser currently not supports 'substringof('soft',tolower(Name)) eq true'", parser.Warnings.ElementAt<string>(0), "The warning wasn't logged properly.");

        }


    }
}
