using System.Data;
using Microsoft.SqlServer.Server;

namespace unQuery.SqlTypes
{
	public class SqlImage : ExplicitMaxLengthType<byte[]>
	{
		private SqlImage() :
			base(SqlDbType.Image)
		{ }

		public SqlImage(byte[] value) :
			base(value, SqlDbType.Image, maxLength: -1)
		{ }

		private static readonly ITypeHandler typeHandler = new SqlImage();
		internal static ITypeHandler GetTypeHandler()
		{
			return typeHandler;
		}

		public override void SetDataRecordValue(SqlDataRecord record, int ordinal)
		{
			if (Value == null)
				record.SetDBNull(ordinal);
			else
				record.SetBytes(ordinal, 0, Value, 0, Value.Length);
		}
	}
}