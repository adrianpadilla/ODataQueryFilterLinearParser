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
    public class QueryStringParser
    {

        public ConcurrentBag<string> Warnings { get; set; }

        public ConcurrentBag<Exception> Exceptions { get; set; }

        public QueryInformation Parse(ODataQueryOptions options)
        {
            QueryInformation info = new QueryInformation();

            ParseFilterData(options.Filter, info);

            return info;

        }

        public QueryInformation ParseFilterData(FilterQueryOption filter, QueryInformation info)
        {
            try
            {
                if (filter == null)
                {
                    return info;
                }

                info.FilterParameters = new ODataFilterParameterCollection();

                FilterClause clause = filter.FilterClause;
                BinarySearchForNodes(clause.Expression, info);

                return info;
            }
            catch (Exception ex)
            {

                LogException(ex);
                return info;
            }
        }

        private void BinarySearchForNodes(QueryNode node, QueryInformation info)
        {

            try
            {
                var binaryNode = node as BinaryOperatorNode;
                if (binaryNode != null)
                {
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

        private void LogException(Exception ex)
        {
            if (Exceptions == null)
            {
                Exceptions = new ConcurrentBag<Exception>();
            }

            Exceptions.Add(ex);
        }

        private void LogWarning(string message)
        {
            if (Warnings == null)
            {
                Warnings = new ConcurrentBag<string>();
            }

            Warnings.Add(message);
        }

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

        private void ProcessLeafNode(QueryInformation info, BinaryOperatorNode binaryNode, ConstantNode constantNode)
        {
            try
            {

                FilterParameterDefinition filterdefinition = new FilterParameterDefinition();

                
                filterdefinition.FilteringOperator = binaryNode.OperatorKind;
                filterdefinition.StringValue = constantNode.Value.ToString();

                SingleValuePropertyAccessNode propertyAccessNode = binaryNode.Left as SingleValuePropertyAccessNode;
                SingleValueFunctionCallNode functioncall = binaryNode.Left as SingleValueFunctionCallNode;

                if (functioncall != null)
                {
                    StringBuilder builder = new StringBuilder();

                    var argumentEnumerator = functioncall.Arguments.GetEnumerator();
                    filterdefinition.Modifiers =  functioncall.Name;
                    argumentEnumerator.MoveNext();
                    propertyAccessNode = argumentEnumerator.Current as SingleValuePropertyAccessNode;
                }

                if (propertyAccessNode != null)
                {
                    //it is a simple equals
                    filterdefinition.FieldName = propertyAccessNode.Property.Name;

                    info.FilterParameters.Add(filterdefinition);
                }    
            }
            catch (Exception ex)
            {

                LogException(ex);
            }
        }
    }
}
