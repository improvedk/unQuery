using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitMaxLengthType<TValue> : SqlType, ISqlType, ITypeHandler
	{
		protected readonly TValue Value;
		private readonly int? maxLength;
		private readonly bool hasValue;
		private readonly SqlDbType dbType;
		private readonly int? valueLength;

		internal ExplicitMaxLengthType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitMaxLengthType(TValue value, SqlDbType dbType, int? maxLength = null, int? valueLength = null)
		{
			this.Value = value;
			this.maxLength = maxLength;
			this.dbType = dbType;
			this.valueLength = valueLength;

			hasValue = true;
		}

		public abstract void SetDataRecordValue(SqlDataRecord record, int ordinal);

		object ISqlType.GetRawValue()
		{
			return Value;
		}

		SqlParameter ISqlType.GetParameter()
		{
			var param = new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(Value)
			};

			if (maxLength != null)
				param.Size = maxLength.Value;
			else if (valueLength != null)
				param.Size = GetAppropriateSizeFromLength(valueLength.Value);

			return param;
		}

		SqlParameter ITypeHandler.CreateParamFromValue(object value)
		{
			throw new TypeCannotBeUsedAsAClrTypeException();
		}

		SqlMetaData ITypeHandler.CreateMetaData(string name)
		{
			if (!hasValue)
				throw new TypeCannotBeUsedAsAClrTypeException();

			if (maxLength == null)
				throw new TypePropertiesMustBeSetExplicitlyException("maxLength");

			return new SqlMetaData(name, dbType, maxLength.Value);
		}
	}
}