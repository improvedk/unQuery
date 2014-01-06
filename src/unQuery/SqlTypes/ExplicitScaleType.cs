using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitScaleType<TValue> : SqlType
	{
		protected readonly TValue Value;
		private readonly byte? scale;
		private readonly bool hasValue;
		private readonly SqlDbType dbType;

		internal ExplicitScaleType(SqlDbType dbType)
		{
			this.dbType = dbType;
		}

		internal ExplicitScaleType(TValue value, byte? scale, SqlDbType dbType)
		{
			this.Value = value;
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

			if (scale != null)
				param.Scale = scale.Value;

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

			if (scale == null)
				throw new TypePropertiesMustBeSetExplicitlyException("scale");

			return new SqlMetaData(name, dbType, 0, scale.Value);
		}
	}
}