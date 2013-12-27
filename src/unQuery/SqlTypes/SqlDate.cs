using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlDate : SqlType, ISqlType, ITypeHandler
	{
		private static readonly ITypeHandler typeHandler = new SqlDate();

		private readonly DateTime? value;
		private readonly bool hasValue;

		private SqlDate()
		{ }

		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		SqlParameter ITypeHandler.CreateParamFromValue(object value)
		{
			throw new TypeCannotBeUsedAsAClrTypeException();
		}

		SqlMetaData ITypeHandler.CreateMetaData(string name)
		{
			if (!hasValue)
				throw new TypeCannotBeUsedAsAClrTypeException();

			return new SqlMetaData(name, SqlDbType.Date);
		}

		public SqlDate(DateTime? value)
		{
			this.value = value;
			hasValue = true;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.Date,
				Value = GetDBNullableValue(value)
			};
		}

		object ISqlType.GetRawValue()
		{
			return value;
		}
	}
}