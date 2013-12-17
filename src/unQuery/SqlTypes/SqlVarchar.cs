using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlVarChar : ISqlType
	{
		private readonly string value;
		private readonly int? size;

		public SqlVarChar(string value)
		{
			this.value = value;
		}

		public SqlVarChar(string value, int size)
		{
			this.value = value;
			this.size = size;
		}

		public static explicit operator SqlVarChar(string value)
		{
			return new SqlVarChar(value);
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
				SqlDbType = SqlDbType.VarChar,
				Value = TypeHelper.GetDBNullableValue(value)
			};

			if (size != null)
				param.Size = size.Value;

			return param;
		}
	}

	internal class SqlVarCharTypeHandler : ITypeHandler
	{
		private static readonly SqlVarCharTypeHandler instance = new SqlVarCharTypeHandler();

		internal static SqlVarCharTypeHandler GetInstance()
		{
			return instance;
		}

		public SqlParameter CreateParamFromValue(object value)
		{
			return SqlVarChar.GetParameter((string)value);
		}

		public SqlDbType GetSqlDbType()
		{
			return SqlDbType.VarChar;
		}

		public void SetDataRecordValue(int ordinal, SqlDataRecord sdr, object value)
		{
			sdr.SetString(ordinal, (string)value);
		}

		public SqlMetaData CreateSqlMetaData(string name)
		{
			return new SqlMetaData(name, SqlDbType.VarChar, -1);
		}
	}
}