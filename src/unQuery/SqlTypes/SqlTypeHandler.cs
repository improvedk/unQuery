using Microsoft.SqlServer.Server;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class SqlTypeHandler
	{
		internal abstract SqlParameter CreateParamFromValue(string name, object value);
		internal abstract SqlMetaData CreateMetaData(string name);
	}
}