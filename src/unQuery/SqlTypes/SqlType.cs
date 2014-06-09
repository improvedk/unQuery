using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlClient;

namespace unQuery.SqlTypes
{
	public abstract class SqlType : SqlTypeHandler
	{
		protected SqlParameter Parameter;

		protected object GetDBNullableValue(object value)
		{
			return value ?? DBNull.Value;
		}

		protected int GetAppropriateSizeFromLength(int length)
		{
			if (length <= 64)
				return 64;

			if (length <= 256)
				return 256;

			if (length <= 1024)
				return 1024;

			if (length <= 4096)
				return 4096;

			return 8000;
		}

		internal abstract SqlParameter GetParameter();
		internal abstract object GetRawValue();
		internal abstract void SetDataRecordValue(SqlDataRecord record, int ordinal);
	}
}