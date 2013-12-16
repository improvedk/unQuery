using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	internal interface ITypeHandler
	{
		SqlParameter CreateParamFromValue(object value);
		SqlDbType GetSqlDbType();
		void SetDataRecordValue(int ordinal, SqlDataRecord sdr, object value);
		SqlMetaData CreateSqlMetaData(string name);
	}
}