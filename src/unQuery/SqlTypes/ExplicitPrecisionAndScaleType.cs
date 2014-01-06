using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitPrecisionAndScaleType<TValue> : SqlType
	{
		protected readonly TValue Value;
		private readonly byte? scale;
		private readonly byte? precision;
		private readonly bool hasValue;
		private readonly SqlDbType dbType;

		internal ExplicitPrecisionAndScaleType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitPrecisionAndScaleType(TValue value, byte? precision, byte? scale, SqlDbType dbType)
		{
			this.Value = value;
			this.precision = precision;
			this.scale = scale;
			this.dbType = dbType;

			hasValue = true;
		}

		internal override object GetRawValue()
		{
			return Value;
		}

		internal override SqlParameter GetParameter()
		{
			var param = new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(Value)
			};

			if (precision != null && scale != null)
			{
				param.Precision = precision.Value;
				param.Scale = scale.Value;
			}

			return param;
		}

		internal override SqlParameter CreateParamFromValue(object value)
		{
			throw new TypeCannotBeUsedAsAClrTypeException();
		}

		internal override SqlMetaData CreateMetaData(string name)
		{
			if (!hasValue)
				throw new TypeCannotBeUsedAsAClrTypeException();

			if (precision == null || scale == null)
				throw new TypePropertiesMustBeSetExplicitlyException("precision & scale");

			return new SqlMetaData(name, dbType, precision.Value, scale.Value);
		}
	}
}