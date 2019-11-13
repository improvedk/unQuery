using Microsoft.Data.SqlClient.Server;
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitValueType<TValue> : SqlType
	{
		protected readonly TValue InputValue;
		private readonly SqlDbType dbType;
		private readonly bool hasValue;
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

		internal ExplicitValueType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitValueType(TValue value, SqlDbType dbType, ParameterDirection direction)
		{
			this.InputValue = value;
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
			return Parameter ?? (Parameter = new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(InputValue),
				Direction = direction
			});
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