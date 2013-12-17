using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	internal interface ISqlType
	{
		SqlParameter GetParameter();
		object GetRawValue();
	}
}