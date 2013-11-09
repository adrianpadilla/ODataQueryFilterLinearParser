ODataQueryFilterLinearParser
============================

Parses a QueryOptions object from a Web API EntitySetController into a dictionary of filters.

While experimenting with different sources for exposing data from an OData Web Api, I didn’t find a way to extract a set of filters in a key/value fashion, to be used for running a SQL stored procedure for example.
The QueryOptions object just exposes the Raw filter definition, or an expression tree, but not a dictionary of parameters, obviously because OData supports complex filter grouping and expand filters.
This parser just supports extracting parameters in a linear way (hence the name), because it is all I need to convert them to parameters so I can expose the result of a regular method or a stored procedure through OData.
Maybe doesn’t have many practical uses but is a good theoric practice for understanding Microsoft’s OData AST’s.

-	Adrian
-	
Current state: uploading bits.
Next: adding some unit tests.


