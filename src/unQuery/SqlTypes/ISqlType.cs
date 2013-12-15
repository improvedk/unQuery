using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public interface ISqlType
	{
		SqlParameter GetParameter();
		SqlDbType GetDbType();
		object GetRawValue();
	}
}