using System;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using System.Net.Http;
using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;
using System.Collections.Concurrent;
using System.Text;

namespace AdrianPadilla.Net.OData.QueryFilterLinearParser
{
    /// <summary>
    /// Parses different pieces of the ODataQueryOptions object, 
    /// to be exposed to places that doesn't reference the http OData Api assemblies.
    /// </summary>
    public class QueryStringParser
    {
        /// <summary>
        /// List of warnings generated during the parsing.
        /// </summary>
        public ConcurrentBag<string> Warnings { get; set; }

        /// <summary>
        /// List of exceptions generated during the parsing.
        /// </summary>
        /// <remarks>
        /// We need to avoid crashes generated from unsupported query options, to the service keeps working.
        /// This is not a good practice, but it is expected in the scenario I need to address.
        /// </remarks>
        public ConcurrentBag<Exception> Exceptions { get; set; }

        /// <summary>
        /// Parses an ODataQueryOptions objects exposing its features in a custom object.
        /// </summary>
        /// <param name="options">ODataQueryOptions given by the ASP.NET Web Odata API controller.</param>
        /// <returns>A QueryInformation containing the parsing results.</returns>
        /// <remarks>
        /// We currently support only the Filter aspects of the ODataQueryOptions object.
        /// </remarks>
        public QueryInformation Parse(ODataQueryOptions options)
        {
            QueryInformation info = new QueryInformation();

            ParseFilterData(options.Filter, info);

            return info;
        }

        /// <summary>
        /// Parses the Filter portion of the ODataQueryOptions object and adds its details to the used QueryInformation.
        /// </summary>
        /// <param name="filter">FilterQueryOption object contained in the ODataQueryOptions from the OData controller.</param>
        /// <param name="info">The QueryInformation object that is maintained to give back the results.</param>
        public void ParseFilterData(FilterQueryOption filter, QueryInformation info)
        {
            try
            {
                // If the filter comes null, there is no work to be done by this method.
                if (filter == null)
                {
                    return;
                }

                // There is at least one parameter so we instantiate the collection in the info object.
                info.FilterParameters = new ODataFilterParameterCollection();

                // The abstract syntax tree of the expression is a binary tree, we perform it here from the root.
                BinarySearchForNodes(filter.FilterClause.Expression, info);

                return;
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Performs a Binary search on the abstract syntax tree of the filter expression. 
        /// </summary>
        /// <param name="node">The node that needs to be searched on.</param>
        /// <param name="info">The QueryInformation where we are accumulating the filter definitions. </param>
        private void BinarySearchForNodes(QueryNode node, QueryInformation info)
        {

            try
            {
                var binaryNode = node as BinaryOperatorNode;

                if (binaryNode != null)
                {
                    // We look for constant nodes that would mean a leaf have been reached.
                    ConstantNode constantNode = GetConstantNode(binaryNode);

                    if (constantNode != null)
                    {
                        ProcessLeafNode(info, binaryNode, constantNode);

                    }
                    else
                    {
                        BinarySearchForNodes(binaryNode.Left, info);
                        BinarySearchForNodes(binaryNode.Right, info);
                    }
                }
                else
                {
                    LogWarning(string.Format("Node wasn't casted as binary node. Type is {0}", node.GetType().FullName));
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Gets the constant node of a node, either directly or from a function modifier
        /// </summary>
        /// <param name="binaryNode">Node to be evaluated.</param>
        /// <returns>A ConstantNode or null if the node does not contain a node that can be resolved to a constant.</returns>
        private static ConstantNode GetConstantNode(BinaryOperatorNode binaryNode)
        {
            ConvertNode convertNode = null;
            ConstantNode constantNode = null;

            convertNode = binaryNode.Right as ConvertNode;
            if (convertNode != null)
            {
                constantNode = convertNode.Source as ConstantNode;
            }
            else
            {
                constantNode = binaryNode.Right as ConstantNode;
            }

            return constantNode;
        }

        /// <summary>
        /// Extracts the filter definition once a leaf is found.
        /// </summary>
        /// <param name="info">QueryInformation being filled.</param>
        /// <param name="binaryNode">BinaryOperatorNode the leaf belongs to.</param>
        /// <param name="constantNode">ConstantNode with the parameter definition.</param>
        private void ProcessLeafNode(QueryInformation info, BinaryOperatorNode binaryNode, ConstantNode constantNode)
        {
            try
            {
                // Necesary variables
                SingleValuePropertyAccessNode propertyAccessNode = binaryNode.Left as SingleValuePropertyAccessNode;
                SingleValueFunctionCallNode functioncall = binaryNode.Left as SingleValueFunctionCallNode;

                // The filter definition that is going to be added.
                FilterParameterDefinition filterdefinition = new FilterParameterDefinition();

                // Populating common values.
                filterdefinition.FilteringOperator = binaryNode.OperatorKind;
                filterdefinition.StringValue = constantNode.Value.ToString();

                
                if (functioncall != null)
                {
                    // it contains a function call
                    propertyAccessNode = ParseFunctionCall(propertyAccessNode, functioncall, filterdefinition);
                }

                if (propertyAccessNode != null)
                {
                    //it is a simple equals
                    ParsePropertyAccessNode(info, propertyAccessNode, filterdefinition);
                }    
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Parses a Property access node.
        /// </summary>
        /// <param name="info">QueryInformation being filled.</param>
        /// <param name="propertyAccessNode">Property access node to be parsed.</param>
        /// <param name="filterdefinition">Filter definition being filled.</param>
        private static void ParsePropertyAccessNode(QueryInformation info, SingleValuePropertyAccessNode propertyAccessNode, FilterParameterDefinition filterdefinition)
        {
            filterdefinition.FieldName = propertyAccessNode.Property.Name;

            info.FilterParameters.Add(filterdefinition);
        }

        /// <summary>
        /// Gets the SingleValuePropertyAccessNode from a function call.
        /// </summary>
        /// <param name="propertyAccessNode">Property access node the function call belongs to.</param>
        /// <param name="functioncall">Function call being parsed.</param>
        /// <param name="filterdefinition">Filter definition being filled.</param>
        /// <returns>A SingleValuePropertyAccessNode to be parsed.</returns>
        private static SingleValuePropertyAccessNode ParseFunctionCall(SingleValuePropertyAccessNode propertyAccessNode, SingleValueFunctionCallNode functioncall, FilterParameterDefinition filterdefinition)
        {
            StringBuilder builder = new StringBuilder();

            var argumentEnumerator = functioncall.Arguments.GetEnumerator();
            filterdefinition.Modifiers = functioncall.Name;
            argumentEnumerator.MoveNext();
            propertyAccessNode = argumentEnumerator.Current as SingleValuePropertyAccessNode;
            return propertyAccessNode;
        }

        /// <summary>
        /// Logs an expection in the collection.
        /// </summary>
        /// <param name="ex">Exception to log.</param>
        /// <remarks>
        /// We need to avoid any crash in the service.
        /// </remarks>
        private void LogException(Exception ex)
        {
            if (Exceptions == null)
            {
                Exceptions = new ConcurrentBag<Exception>();
            }

            Exceptions.Add(ex);
        }

        /// <summary>
        /// Logs a warning encountered during the parsing.
        /// </summary>
        /// <param name="message">Warning message to log.</param>
        private void LogWarning(string message)
        {
            if (Warnings == null)
            {
                Warnings = new ConcurrentBag<string>();
            }

            Warnings.Add(message);
        }
    }
}
