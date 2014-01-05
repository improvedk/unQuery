using System.Data.SqlClient;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	internal interface ISqlType
	{
		SqlParameter GetParameter();
		object GetRawValue();
		void SetDataRecordValue(SqlDataRecord record, int ordinal);
	}
}