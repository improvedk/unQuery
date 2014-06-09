using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitMaxLengthType<TValue> : SqlType
	{
		protected readonly TValue InputValue;
		private readonly int? maxLength;
		private readonly bool hasValue;
		private readonly SqlDbType dbType;
		private readonly int? valueLength;
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

		internal ExplicitMaxLengthType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitMaxLengthType(TValue value, SqlDbType dbType, ParameterDirection direction, int? maxLength = null, int? valueLength = null)
		{
			this.InputValue = value;
			this.maxLength = maxLength;
			this.dbType = dbType;
			this.valueLength = valueLength;
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

				if (maxLength != null)
					Parameter.Size = maxLength.Value;
				else if (valueLength != null)
					Parameter.Size = GetAppropriateSizeFromLength(valueLength.Value);
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

			if (maxLength == null)
				throw new TypePropertiesMustBeSetExplicitlyException("maxLength");

			return new SqlMetaData(name, dbType, maxLength.Value);
		}
	}
}