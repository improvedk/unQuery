using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlBinary : SqlType, ISqlType, ITypeHandler
	{
		private static readonly ITypeHandler typeHandler = new SqlBinary();

		private readonly byte[] value;
		private readonly int maxLength;
		private readonly bool hasValue;

		private SqlBinary()
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

			return new SqlMetaData(name, SqlDbType.Binary, maxLength);
		}

		public SqlBinary(byte[] value, int maxLength)
		{
			this.value = value;
			this.maxLength = maxLength;

			hasValue = true;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.Binary,
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