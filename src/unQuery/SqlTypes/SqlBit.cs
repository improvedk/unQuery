using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlBit : ISqlType
	{
		private readonly bool? value;

		public SqlBit(bool? value)
		{
			this.value = value;
		}

		public static explicit operator SqlBit(bool? value)
		{
			return new SqlBit(value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value);
		}

		public SqlDbType GetSqlDbType()
		{
			return SqlDbType.Bit;
		}

		public object GetRawValue()
		{
			return value;
		}

		public static SqlParameter GetParameter(bool? value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.Bit,
				Value = TypeHelper.GetDBNullableValue(value)
			};
		}
	}

	internal class SqlBitTypeHandler : ITypeHandler
	{
		private static readonly SqlBitTypeHandler instance = new SqlBitTypeHandler();

		internal static SqlBitTypeHandler GetInstance()
		{
			return instance;
		}

		public SqlParameter CreateParamFromValue(object value)
		{
			return SqlBit.GetParameter((bool?)value);
		}

		public SqlDbType GetSqlDbType()
		{
			return SqlDbType.Bit;
		}

		public void SetDataRecordValue(int ordinal, SqlDataRecord sdr, object value)
		{
			sdr.SetBoolean(ordinal, (bool)value);
		}

		public SqlMetaData CreateSqlMetaData(string name)
		{
			return new SqlMetaData(name, SqlDbType.Bit);
		}
	}
}