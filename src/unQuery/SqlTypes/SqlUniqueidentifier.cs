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

		public static SqlParameter GetParameter(Guid? value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.UniqueIdentifier,
				Value = TypeHelper.GetDBNullableValue(value)
			};
		}
	}
}