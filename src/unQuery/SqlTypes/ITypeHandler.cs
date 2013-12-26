using Microsoft.SqlServer.Server;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	internal interface ITypeHandler
	{
		SqlParameter CreateParamFromValue(object value);
		SqlMetaData CreateMetaData(string name);
	}
}