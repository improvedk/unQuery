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

		public static SqlParameter GetParameter(object value)
		{
			object realValue = value;
			if (realValue == null)
				realValue = DBNull.Value;

			return new SqlParameter {
				SqlDbType = SqlDbType.UniqueIdentifier,
				Value = realValue
			};
		}
	}
}