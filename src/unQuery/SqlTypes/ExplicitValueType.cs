using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitValueType<TValue> : SqlType, ISqlType, ITypeHandler
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

		public abstract void SetDataRecordValue(SqlDataRecord record, int ordinal);

		object ISqlType.GetRawValue()
		{
			return Value;
		}

		SqlParameter ISqlType.GetParameter()
		{
			return new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(Value)
			};
		}

		SqlParameter ITypeHandler.CreateParamFromValue(object value)
		{
			throw new TypeCannotBeUsedAsAClrTypeException();
		}

		SqlMetaData ITypeHandler.CreateMetaData(string name)
		{
			if (!hasValue)
				throw new TypeCannotBeUsedAsAClrTypeException();

			return new SqlMetaData(name, dbType);
		}
	}
}