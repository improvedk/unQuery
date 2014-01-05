﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlStructured : ISqlType
	{
		private readonly IEnumerable<object> values;
		private readonly string typeName;

		public SqlStructured(string typeName, IEnumerable<object> values)
		{
			this.typeName = typeName;
			this.values = values;
		}

		SqlParameter ISqlType.GetParameter()
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

		public void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			throw new InvalidOperationException("You're not meant to use nested structured parameters.");
		}

		object ISqlType.GetRawValue()
		{
			throw new InvalidOperationException("You're not meant to use nested structured parameters.");
		}
	}
}