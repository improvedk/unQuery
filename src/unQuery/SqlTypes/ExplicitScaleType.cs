using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class ExplicitScaleType<TValue> : SqlType, ISqlType, ITypeHandler
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

			if (scale != null)
				param.Scale = scale.Value;

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

			if (scale == null)
				throw new TypePropertiesMustBeSetExplicitlyException("scale");

			return new SqlMetaData(name, dbType, 0, scale.Value);
		}
	}
}