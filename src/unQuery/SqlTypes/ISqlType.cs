using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public interface ISqlType
	{
		SqlParameter GetParameter();
		SqlDbType GetSqlDbType();
		object GetRawValue();
	}
}