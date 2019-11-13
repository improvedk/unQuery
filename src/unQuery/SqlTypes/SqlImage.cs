using Microsoft.Data.SqlClient.Server;
using System.Data;

namespace unQuery.SqlTypes
{
	public class SqlImage : ExplicitMaxLengthType<byte[]>
	{
		private SqlImage() :
			base(SqlDbType.Image)
		{ }

		internal SqlImage(byte[] value, ParameterDirection direction) :
			base(value, SqlDbType.Image, direction, maxLength: -1)
		{ }

		private static readonly SqlTypeHandler typeHandler = new SqlImage();
		internal static SqlTypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		internal override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (InputValue == null)
				record.SetDBNull(ordinal);
			else
				record.SetBytes(ordinal, 0, InputValue, 0, InputValue.Length);
		}
	}
}