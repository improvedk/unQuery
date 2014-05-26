using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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
			object value = null;

			if (values != null)
			{
				// If there are no values, but it's not null per se, the value should simply not be set.
				// Otherwise SQL Server chokes on a non-null but empty TVP. A TVP is empty by default, if
				// the value is not set.
				var valuesList = values.ToList();
				if (valuesList.Count > 0)
					value = new StructuredDynamicYielder(valuesList);
			}
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

		internal override SqlParameter CreateParamFromValue(string name, object value)
		{
			throw new InvalidOperationException("You're not meant to use nested structured parameters.");
		}

		internal override SqlMetaData CreateMetaData(string name)
		{
			throw new InvalidOperationException("You're not meant to use nested structured parameters.");
		}
	}
}