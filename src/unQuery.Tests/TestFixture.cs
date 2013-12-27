using NUnit.Framework;
using System.Transactions;

namespace unQuery.Tests
{
	[TestFixture]
	public abstract class TestFixture
	{
		private TransactionScope ts;

		[TestFixtureSetUp]
		public void SetUp()
		{
			// Non-transactional setup
			DB.Execute(@"
				IF TYPE_ID('ListOfTinyInts') IS NOT NULL DROP TYPE ListOfTinyInts
				CREATE TYPE ListOfTinyInts AS Table (A tinyint NULL)

				IF TYPE_ID('ListOfSmallInts') IS NOT NULL DROP TYPE ListOfSmallInts
				CREATE TYPE ListOfSmallInts AS Table (A smallint NULL)

				IF TYPE_ID('ListOfInts') IS NOT NULL DROP TYPE ListOfInts
				CREATE TYPE ListOfInts AS Table (A int NULL)

				IF TYPE_ID('ListOfBigInts') IS NOT NULL DROP TYPE ListOfBigInts
				CREATE TYPE ListOfBigInts AS Table (A bigint NULL)

				IF TYPE_ID('ListOfBinary') IS NOT NULL DROP TYPE ListOfBinary
				CREATE TYPE ListOfBinary AS Table (A binary(2) NULL)

				IF TYPE_ID('ListOfChars') IS NOT NULL DROP TYPE ListOfChars
				CREATE TYPE ListOfChars AS Table (A char(10) NULL)

				IF TYPE_ID('ListOfDates') IS NOT NULL DROP TYPE ListOfDates
				CREATE TYPE ListOfDates AS Table (A date NULL)

				IF TYPE_ID('ListOfBits') IS NOT NULL DROP TYPE ListOfBits
				CREATE TYPE ListOfBits AS Table (A bit NULL)

				IF TYPE_ID('ListOfDecimals') IS NOT NULL DROP TYPE ListOfDecimals
				CREATE TYPE ListOfDecimals AS Table (A decimal(10, 5) NULL)

				IF TYPE_ID('ListOfNVarChars') IS NOT NULL DROP TYPE ListOfNVarChars
				CREATE TYPE ListOfNVarChars AS Table (A nvarchar(256) NULL)

				IF TYPE_ID('ListOfVarChars') IS NOT NULL DROP TYPE ListOfVarChars
				CREATE TYPE ListOfVarChars AS Table (A varchar(256) NULL)

				IF TYPE_ID('ListOfUniqueIdentifiers') IS NOT NULL DROP TYPE ListOfUniqueIdentifiers
				CREATE TYPE ListOfUniqueIdentifiers AS Table (A uniqueidentifier NULL)
			");

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