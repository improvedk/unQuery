namespace unQuery.SqlTypes
{
	public static class Col
	{
		public static SqlBit Bit(bool? value)
		{
			return new SqlBit(value);
		}
	}
}