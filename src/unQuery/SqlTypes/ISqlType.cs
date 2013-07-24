namespace unQuery.SqlTypes
{
	public interface ISqlType
	{
		object Value { get; }
		int Length { get; }
	}
}