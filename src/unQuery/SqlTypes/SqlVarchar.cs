namespace unQuery.SqlTypes
{
	public class SqlVarchar : ISqlType
	{
		public object Value { get; private set; }
		public int Length { get; private set; }

		public SqlVarchar(string value, int length)
		{
			Value = value;
			Length = length;
		}

		public SqlVarchar(string value)
			: this(value, value.Length)
		{ }
	}
}