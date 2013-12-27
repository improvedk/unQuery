using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlDecimal : SqlType, ISqlType, ITypeHandler
	{
		private static readonly ITypeHandler typeHandler = new SqlDecimal();

		private readonly decimal? value;
		private readonly byte precision;
		private readonly byte scale;
		private readonly bool hasValue;

		private SqlDecimal()
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

			return new SqlMetaData(name, SqlDbType.Decimal, precision, scale);
		}

		public SqlDecimal(decimal? value, byte precision, byte scale)
		{
			this.value = value;
			this.precision = precision;
			this.scale = scale;
			hasValue = true;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.Decimal,
				Precision = precision,
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