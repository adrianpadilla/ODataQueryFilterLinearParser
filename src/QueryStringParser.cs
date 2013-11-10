using System;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using System.Net.Http;
using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;

namespace AdrianPadilla.Net.OData.QueryFilterLinearParser
{
    public static class QueryStringParser
    {
        public static QueryInformation Parse(ODataQueryOptions options)
        {
            QueryInformation info = new QueryInformation();

            ParseFilterData(options.Filter, info);

            return info;

        }

        public static QueryInformation ParseFilterData(FilterQueryOption filter, QueryInformation info)
        {
            if (filter == null)
            {
                return info;
            }
            FilterClause clause = filter.FilterClause;
            BinarySearchForNodes(clause.Expression, info);

            return info;
        }

        private static void BinarySearchForNodes(QueryNode node, QueryInformation info)
        {
            ConvertNode convertNode = null;
            ConstantNode constantNode = null;

            var binaryNode = node as BinaryOperatorNode;
            if (binaryNode != null)
            {
                convertNode = binaryNode.Right as ConvertNode;
                if (convertNode != null)
                {
                    constantNode = convertNode.Source as ConstantNode;
                }
                else
                {
                    constantNode = binaryNode.Right as ConstantNode;
                }
                
                if (constantNode != null)
                {
                    string value = constantNode.Value.ToString();
                    var propertyAccessNode = binaryNode.Left as SingleValuePropertyAccessNode;
                    var operationKind = binaryNode.OperatorKind;

                    if (info.FilterParameters == null)
                    {
                        info.FilterParameters = new ODataFilterParameterCollection();
                    }
                    FilterParameterDefinition filterdefinition = new FilterParameterDefinition();
                    filterdefinition.FieldName = propertyAccessNode.Property.Name;
                    filterdefinition.FilteringOperator = operationKind;
                    filterdefinition.StringValue = value;

                    info.FilterParameters.Add(filterdefinition);

                }
                else
                {
                    BinarySearchForNodes(binaryNode.Left, info);
                    BinarySearchForNodes(binaryNode.Right, info);
                }
            }
        }
    }
}
