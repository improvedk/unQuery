using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlStructured : SqlType
	{
		private readonly IEnumerable<object> values;
		private readonly string typeName;

		public SqlStructured(string typeName, IEnumerable<object> values)
		{
			this.typeName = typeName;
			this.values = values;
		}

		internal override SqlParameter GetParameter()
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

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			throw new InvalidOperationException("You're not meant to use nested structured parameters.");
		}

		internal override object GetRawValue()
		{
			throw new InvalidOperationException("You're not meant to use nested structured parameters.");
		}

		internal override SqlParameter CreateParamFromValue(object value)
		{
			throw new InvalidOperationException("You're not meant to use nested structured parameters.");
		}

		internal override SqlMetaData CreateMetaData(string name)
		{
			throw new InvalidOperationException("You're not meant to use nested structured parameters.");
		}
	}
}