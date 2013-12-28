using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlNChar : SqlType, ISqlType, ITypeHandler
	{
		private static readonly ITypeHandler typeHandler = new SqlNChar();

		private readonly string value;
		private readonly int maxLength;
		private readonly bool hasValue;

		private SqlNChar()
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

			return new SqlMetaData(name, SqlDbType.NChar, maxLength);
		}

		public SqlNChar(string value, int maxLength)
		{
			this.value = value;
			this.maxLength = maxLength;

			hasValue = true;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.NChar,
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