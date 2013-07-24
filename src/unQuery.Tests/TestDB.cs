namespace unQuery.Tests
{
	public class TestDB : unQuery
	{
		protected override string ConnectionString
		{
			get { return @"Server=.\SQL2012;Database=unQuery;Trusted_Connection=True;"; }
		}
	}
}