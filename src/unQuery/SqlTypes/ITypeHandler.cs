using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	internal interface ITypeHandler
	{
		SqlParameter CreateParamFromValue(object value);
		SqlDbType GetSqlDbType();
		SqlMetaData CreateSqlMetaData(string name);
	}
}