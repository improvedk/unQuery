using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public class SqlMoney : SqlType, ISqlType, ITypeHandler
	{
		private static readonly ITypeHandler typeHandler = new SqlMoney();

		private readonly decimal? value;
		private readonly bool hasValue;

		private SqlMoney()
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

			return new SqlMetaData(name, SqlDbType.Money);
		}

		public SqlMoney(decimal? value)
		{
			this.value = value;

			hasValue = true;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = SqlDbType.Money,
				Value = GetDBNullableValue(value)
			};
		}

		object ISqlType.GetRawValue()
		{
			return value;
		}
	}
}