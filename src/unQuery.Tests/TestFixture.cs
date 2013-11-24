using NUnit.Framework;
using System.Transactions;

namespace unQuery.Tests
{
	[TestFixture]
	public abstract class TestFixture
	{
		protected unQuery DB = new TestDB();
		private TransactionScope ts;

		[TestFixtureSetUp]
		public void SetUp()
		{
			ts = new TransactionScope();

			DB.Execute(@"
				IF OBJECT_ID('Persons') IS NOT NULL DROP TABLE Persons

				CREATE TABLE dbo.Persons (
					PersonID int NOT NULL,
					Name nvarchar(128) NOT NULL,
					Age tinyint NOT NULL,
					Sex char(1) NOT NULL,
					SignedUp datetime NULL,
					CONSTRAINT PK_Persons PRIMARY KEY (PersonID ASC)
				);

				INSERT INTO
					dbo.Persons (PersonID, Name, Age, Sex, SignedUp)
				VALUES
					(1, 'Stefanie Alexander', 55, 'F', NULL),
					(2, 'Lee Buckley', 37, 'M', NULL),
					(3, 'Daniel Gallagher', 25, 'M', '1997-11-15 21:03:54.000'),
					(4, 'Myra Lucero', 65, 'F', '2007-07-03 05:07:33.680'),
					(5, 'Annie Brennan', 23, 'M', '1984-01-07 13:24:42.110')
			");
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			ts.Dispose();
		}
	}
}