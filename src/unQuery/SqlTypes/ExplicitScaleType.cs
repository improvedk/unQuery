using Microsoft.Data.SqlClient.Server;
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitScaleType<TValue> : SqlType
	{
		protected readonly TValue InputValue;
		private readonly byte? scale;
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

		internal ExplicitScaleType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitScaleType(TValue value, byte? scale, SqlDbType dbType, ParameterDirection direction)
		{
			this.InputValue = value;
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

				if (scale != null)
					Parameter.Scale = scale.Value;
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

			if (scale == null)
				throw new TypePropertiesMustBeSetExplicitlyException("scale");

			return new SqlMetaData(name, dbType, 0, scale.Value);
		}
	}
}