using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlDateTime2 : SqlType, ISqlType, ITypeHandler
	{
		private static readonly ITypeHandler typeHandler = new SqlDateTime2();

		private readonly DateTime? value;
		private readonly byte scale;
		private readonly bool hasValue;

		private SqlDateTime2()
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

			return new SqlMetaData(name, SqlDbType.DateTime2, 0, scale);
		}

		public SqlDateTime2(DateTime? value, byte scale)
		{
			this.value = value;
			this.scale = scale;

			hasValue = true;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.DateTime2,
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