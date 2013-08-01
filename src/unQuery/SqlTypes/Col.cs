namespace unQuery.SqlTypes
{
	public static class Col
	{
		public static SqlBit Bit(bool? value)
		{
			return new SqlBit(value);
		}

		public static SqlTinyInt TinyInt(byte? value)
		{
			return new SqlTinyInt(value);
		}

		public static SqlSmallInt SmallInt(byte? value)
		{
			return new SqlSmallInt(value);
		}

		public static SqlInt Int(int? value)
		{
			return new SqlInt(value);
		}

		public static SqlBigInt BigInt(long? value)
		{
			return new SqlBigInt(value);
		}
	}
}