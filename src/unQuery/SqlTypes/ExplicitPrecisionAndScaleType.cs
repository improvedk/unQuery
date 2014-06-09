using System;
using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitPrecisionAndScaleType<TValue> : SqlType
	{
		protected readonly TValue InputValue;
		private readonly byte? scale;
		private readonly byte? precision;
		private readonly bool hasValue;
		private readonly SqlDbType dbType;
		private readonly ParameterDirection direction;

		public TValue Value
		{
			get
			{
				if (Parameter == null)
					throw new CannotAccessParameterValueBeforeExecutingQuery();

				if (Parameter.Value == DBNull.Value)
					return default(TValue);

				return (TValue)Parameter.Value;
			}
		}

		internal ExplicitPrecisionAndScaleType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitPrecisionAndScaleType(TValue value, byte? precision, byte? scale, SqlDbType dbType, ParameterDirection direction)
		{
			this.InputValue = value;
			this.precision = precision;
			this.scale = scale;
			this.dbType = dbType;
			this.direction = direction;

			hasValue = true;
		}

		internal override object GetRawValue()
		{
			return InputValue;
		}

		internal override SqlParameter GetParameter()
		{
			if (Parameter == null)
			{
				Parameter = new SqlParameter {
					SqlDbType = dbType,
					Value = GetDBNullableValue(InputValue),
					Direction = direction
				};

				if (precision != null && scale != null)
				{
					Parameter.Precision = precision.Value;
					Parameter.Scale = scale.Value;
				}
			}

			return Parameter;
		}

		internal override SqlParameter CreateParamFromValue(string name, object value)
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