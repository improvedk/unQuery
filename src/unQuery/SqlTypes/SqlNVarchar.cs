using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlNVarChar : ISqlType
	{
		private readonly string value;
		private readonly int? size;

		public SqlNVarChar(string value)
		{
			this.value = value;
		}

		public SqlNVarChar(string value, int size)
		{
			this.value = value;
			this.size = size;
		}

		public static explicit operator SqlNVarChar(string value)
		{
			return new SqlNVarChar(value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value, size);
		}

		public object GetRawValue()
		{
			return value;
		}

		internal static SqlParameter GetParameter(string value)
		{
			return GetParameter(value, null);
		}

		internal static SqlParameter GetParameter(string value, int? size)
		{
			var param = new SqlParameter {
				SqlDbType = SqlDbType.NVarChar,
				Value = TypeHelper.GetDBNullableValue(value)
			};

			if (size != null)
				param.Size = size.Value;

			return param;
		}
	}

	internal class SqlNVarCharTypeHandler : ITypeHandler
	{
		private static readonly SqlNVarCharTypeHandler instance = new SqlNVarCharTypeHandler();

		internal static SqlNVarCharTypeHandler GetInstance()
		{
			return instance;
		}

		public SqlParameter CreateParamFromValue(object value)
		{
			return SqlNVarChar.GetParameter((string)value);
		}

		public SqlDbType GetSqlDbType()
		{
			return SqlDbType.NVarChar;
		}

		public void SetDataRecordValue(int ordinal, SqlDataRecord sdr, object value)
		{
			sdr.SetString(ordinal, (string)value);
		}

		public SqlMetaData CreateSqlMetaData(string name)
		{
			return new SqlMetaData(name, SqlDbType.NVarChar, -1);
		}
	}
}