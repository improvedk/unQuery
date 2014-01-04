using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitMaxLengthType<TValue> : SqlType, ISqlType, ITypeHandler
	{
		private readonly TValue value;
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
			this.value = value;
			this.maxLength = maxLength;
			this.dbType = dbType;
			this.valueLength = valueLength;

			hasValue = true;
		}

		object ISqlType.GetRawValue()
		{
			return value;
		}

		SqlParameter ISqlType.GetParameter()
		{
			var param = new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(value)
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