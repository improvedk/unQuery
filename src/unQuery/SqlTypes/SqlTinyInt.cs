using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlTinyInt : SqlType, ISqlType
	{
		private readonly byte? value;

		public SqlTinyInt(byte? value)
		{
			this.value = value;
		}

		public static explicit operator SqlTinyInt(long? value)
		{
			return new SqlTinyInt((byte?)value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value);
		}

		public object GetRawValue()
		{
			return value;
		}

		internal static SqlParameter GetParameter(byte? value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.TinyInt,
				Value = GetDBNullableValue(value)
			};
		}
	}

	internal class SqlTinyIntTypeHandler : ITypeHandler
	{
		private static readonly SqlTinyIntTypeHandler instance = new SqlTinyIntTypeHandler();

		internal static SqlTinyIntTypeHandler GetInstance()
		{
			return instance;
		}

		public SqlParameter CreateParamFromValue(object value)
		{
			return SqlTinyInt.GetParameter((byte?)value);
		}

		public SqlDbType GetSqlDbType()
		{
			return SqlDbType.TinyInt;
		}

		public void SetDataRecordValue(int ordinal, SqlDataRecord sdr, object value)
		{
			sdr.SetByte(ordinal, (byte)value);
		}

		public SqlMetaData CreateSqlMetaData(string name)
		{
			return new SqlMetaData(name, SqlDbType.TinyInt);
		}
	}
}