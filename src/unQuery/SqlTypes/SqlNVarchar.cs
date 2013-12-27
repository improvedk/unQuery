using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlNVarChar : SqlType, ISqlType, ITypeHandler
	{
		private static readonly ITypeHandler typeHandler = new SqlNVarChar();

		private readonly string value;
		private readonly int maxLength;
		private readonly bool hasValue;

		private SqlNVarChar()
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

			return new SqlMetaData(name, SqlDbType.NVarChar, maxLength);
		}

		public SqlNVarChar(string value, int maxLength)
		{
			this.value = value;
			this.maxLength = maxLength;

			hasValue = true;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.NVarChar,
				Value = GetDBNullableValue(value),
				Size = maxLength
			};
		}

		object ISqlType.GetRawValue()
		{
			return value;
		}
	}
}