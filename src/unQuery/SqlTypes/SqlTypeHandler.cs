using System;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class SqlTypeHandler
	{
		internal virtual SqlParameter CreateParamFromValue(string name, object value) => throw new NotImplementedException();
		internal virtual SqlMetaData CreateMetaData(string name) => throw new NotImplementedException();
	}
}