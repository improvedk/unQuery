using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlUniqueIdentifier : ISqlType
	{
		private readonly Guid? value;

		public SqlUniqueIdentifier(Guid? value)
		{
			this.value = value;
		}

		public static explicit operator SqlUniqueIdentifier(Guid? value)
		{
			return new SqlUniqueIdentifier(value);
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(value);
		}

		public object GetRawValue()
		{
			return value;
		}

		internal static SqlParameter GetParameter(Guid? value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.UniqueIdentifier,
				Value = TypeHelper.GetDBNullableValue(value)
			};
		}
	}

	internal class SqlUniqueIdentifierTypeHandler : ITypeHandler
	{
		private static readonly SqlUniqueIdentifierTypeHandler instance = new SqlUniqueIdentifierTypeHandler();

		internal static SqlUniqueIdentifierTypeHandler GetInstance()
		{
			return instance;
		}

		public SqlParameter CreateParamFromValue(object value)
		{
			return SqlUniqueIdentifier.GetParameter((Guid?)value);
		}

		public SqlDbType GetSqlDbType()
		{
			return SqlDbType.UniqueIdentifier;
		}

		public void SetDataRecordValue(int ordinal, SqlDataRecord sdr, object value)
		{
			sdr.SetGuid(ordinal, (Guid)value);
		}

		public SqlMetaData CreateSqlMetaData(string name)
		{
			return new SqlMetaData(name, SqlDbType.UniqueIdentifier);
		}
	}
}