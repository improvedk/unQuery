using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlSmallInt : ISqlType
	{
		private readonly short? value;

		public SqlSmallInt(short? value)
		{
			this.value = value;
		}

		public static explicit operator SqlSmallInt(long? value)
		{
			return new SqlSmallInt((short?)value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value);
		}

		public object GetRawValue()
		{
			return value;
		}

		internal static SqlParameter GetParameter(short? value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.SmallInt,
				Value = TypeHelper.GetDBNullableValue(value)
			};
		}
	}

	internal class SqlSmallIntTypeHandler : ITypeHandler
	{
		private static readonly SqlSmallIntTypeHandler instance = new SqlSmallIntTypeHandler();

		internal static SqlSmallIntTypeHandler GetInstance()
		{
			return instance;
		}

		public SqlParameter CreateParamFromValue(object value)
		{
			return SqlSmallInt.GetParameter((short?)value);
		}

		public SqlDbType GetSqlDbType()
		{
			return SqlDbType.SmallInt;
		}

		public void SetDataRecordValue(int ordinal, SqlDataRecord sdr, object value)
		{
			sdr.SetInt16(ordinal, (short)value);
		}

		public SqlMetaData CreateSqlMetaData(string name)
		{
			return new SqlMetaData(name, SqlDbType.SmallInt);
		}
	}
}