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

		internal ExplicitMaxLengthType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitMaxLengthType(TValue value, int? maxLength, SqlDbType dbType)
		{
			this.value = value;
			this.maxLength = maxLength;
			this.dbType = dbType;

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