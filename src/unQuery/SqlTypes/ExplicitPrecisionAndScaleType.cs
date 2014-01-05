using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitPrecisionAndScaleType<TValue> : SqlType, ISqlType, ITypeHandler
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

			if (precision != null && scale != null)
			{
				param.Precision = precision.Value;
				param.Scale = scale.Value;
			}

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

			if (precision == null || scale == null)
				throw new TypePropertiesMustBeSetExplicitlyException("precision & scale");

			return new SqlMetaData(name, dbType, precision.Value, scale.Value);
		}
	}
}