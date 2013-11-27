﻿using System;
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

		public static SqlParameter GetParameter(object value)
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.Bit,
				Value = value ?? DBNull.Value
			};
		}
	}
}