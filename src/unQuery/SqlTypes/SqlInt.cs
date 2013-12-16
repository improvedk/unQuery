using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlInt : ISqlType
	{
		private readonly int? value;

		public SqlInt(int? value)
		{
			this.value = value;
		}

		public static explicit operator SqlInt(long? value)
		{
			return new SqlInt((int?)value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value);
		}

		public SqlDbType GetSqlDbType()
		{
			return SqlDbType.Int;
		}

		public object GetRawValue()
		{
			return value;
		}

		public static SqlParameter GetParameter(int? value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.Int,
				Value = TypeHelper.GetDBNullableValue(value)
			};
		}
	}

	internal class SqlIntTypeHandler : ITypeHandler
	{
		private static readonly SqlIntTypeHandler instance = new SqlIntTypeHandler();

		internal static SqlIntTypeHandler GetInstance()
		{
			return instance;
		}

		public SqlParameter CreateParamFromValue(object value)
		{
			return SqlInt.GetParameter((int?)value);
		}

		public SqlDbType GetSqlDbType()
		{
			return SqlDbType.Int;
		}

		public void SetDataRecordValue(int ordinal, SqlDataRecord sdr, object value)
		{
			sdr.SetInt32(ordinal, (int)value);
		}

		public SqlMetaData CreateSqlMetaData(string name)
		{
			return new SqlMetaData(name, SqlDbType.Int);
		}
	}
}