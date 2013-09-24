using System.Transactions;
using NUnit.Framework;

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
					PersonID INT IDENTITY(1, 1) NOT NULL,
					Name NVARCHAR(128) NOT NULL,
					Age TINYINT NOT NULL,
					Sex CHAR(1) NOT NULL,
					SignedUp DATETIME NULL,
					CONSTRAINT PK_Persons PRIMARY KEY (PersonID ASC)
				);

				INSERT INTO
					dbo.Persons (Name, Age, Sex, SignedUp)
				VALUES
					('Stefanie Alexander', 55, 'F', NULL),
					('Lee Buckley', 37, 'M', NULL),
					('Daniel Gallagher', 25, 'M', '1997-11-15 21:03:54.000'),
					('Myra Lucero', 65, 'F', '2007-07-03 05:07:33.680'),
					('Annie Brennan', 23, 'M', '1984-01-07 13:24:42.110')
			");
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			ts.Dispose();
		}
	}
}