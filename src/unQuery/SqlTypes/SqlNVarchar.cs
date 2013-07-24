namespace unQuery.SqlTypes
{
	public class SqlNVarchar : ISqlType
	{
		public object Value { get; private set; }
		public int Length { get; private set; }

		public SqlNVarchar(string value, int length)
		{
			Value = value;
			Length = length;
		}

		public SqlNVarchar(string value)
			: this(value, value.Length)
		{ }
	}
}