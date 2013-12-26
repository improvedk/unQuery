using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlBit : SqlType, ISqlType, ITypeHandler
	{				   
		private static readonly ITypeHandler typeHandler = new SqlBit();

		private readonly bool? value;

		private SqlBit()
		{ }

		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		SqlParameter ITypeHandler.CreateParamFromValue(object value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.Bit,
				Value = GetDBNullableValue(value)
			};
		}

		SqlMetaData ITypeHandler.CreateMetaData(string name)
		{
			return new SqlMetaData(name, SqlDbType.Bit);
		}

		public SqlBit(bool? value)
		{
			this.value = value;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.Bit,
				Value = GetDBNullableValue(value)
			};
		}

		object ISqlType.GetRawValue()
		{
			return value;
		}
	}
}