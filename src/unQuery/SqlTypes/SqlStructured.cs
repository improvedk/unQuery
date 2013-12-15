using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlStructured : ISqlType
	{
		private readonly IEnumerable<dynamic> values;
		private readonly string typeName;

		public SqlStructured(string typeName, IEnumerable<dynamic> values)
		{
			this.typeName = typeName;
			this.values = values;
		}

		public SqlParameter GetParameter()
		{
			return GetParameter(typeName, values);
		}

		public SqlDbType GetDbType()
		{
			return SqlDbType.Structured;
		}

		public object GetRawValue()
		{
			throw new InvalidOperationException("You're not meant to use nested structured parameters.");
		}

		public static SqlParameter GetParameter(string typeName, IEnumerable<dynamic> values)
		{
			object value;

			if (values != null)
				value = new StructuredDynamicYielder(values);
			else
				value = DBNull.Value;

			return new SqlParameter {
				SqlDbType = SqlDbType.Structured,
				TypeName = typeName,
				Value = value
			};
		}
	}
}