using System.Data;

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
	}
}