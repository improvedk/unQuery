using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitValueType<TValue> : SqlType
	{
		protected readonly TValue Value;
		private readonly SqlDbType dbType;
		private readonly bool hasValue;

		internal ExplicitValueType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitValueType(TValue value, SqlDbType dbType)
		{
			this.Value = value;
			this.dbType = dbType;

			hasValue = true;
		}

		internal override object GetRawValue()
		{
			return Value;
		}

		internal override SqlParameter GetParameter()
		{
			return new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(Value)
			};
		}

		internal override SqlParameter CreateParamFromValue(string name, object value)
		{
			throw new TypeCannotBeUsedAsAClrTypeException();
		}

		internal override SqlMetaData CreateMetaData(string name)
		{
			if (!hasValue)
				throw new TypeCannotBeUsedAsAClrTypeException();

			return new SqlMetaData(name, dbType);
		}
	}
}