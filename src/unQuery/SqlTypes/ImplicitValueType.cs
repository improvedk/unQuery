using Microsoft.SqlServer.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ImplicitValueType<TValue> : SqlType
	{
		protected readonly TValue InputValue;
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

		internal ImplicitValueType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ImplicitValueType(TValue value, SqlDbType dbType, ParameterDirection direction)
		{
			this.InputValue = value;
			this.dbType = dbType;
			this.direction = direction;
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
			return new SqlParameter {
				SqlDbType = dbType,
				Value = GetDBNullableValue(value),
				ParameterName = "@" + name,
				Direction = direction
			};
		}

		internal override SqlMetaData CreateMetaData(string name)
		{
			return new SqlMetaData(name, dbType);
		}
	}
}