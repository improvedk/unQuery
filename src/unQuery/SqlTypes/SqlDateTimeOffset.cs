using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlDateTimeOffset : SqlType, ISqlType, ITypeHandler
	{
		private static readonly ITypeHandler typeHandler = new SqlDateTimeOffset();

		private readonly DateTimeOffset? value;
		private readonly byte scale;
		private readonly bool hasValue;

		private SqlDateTimeOffset()
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

			return new SqlMetaData(name, SqlDbType.DateTimeOffset, 0, scale);
		}

		public SqlDateTimeOffset(DateTimeOffset? value, byte scale)
		{
			this.value = value;
			this.scale = scale;

			hasValue = true;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.DateTimeOffset,
				Scale = scale,
				Value = GetDBNullableValue(value)
			};
		}

		object ISqlType.GetRawValue()
		{
			return value;
		}
	}
}