using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace unQuery.Tests
{
	public class MapReaderRowToObjectTests : TestFixture
	{
		[Test]
		public void MapReaderRowToObjectTests_AllColumnTypes()
		{
			var result = DB.GetRow(@"
				SELECT
					CAST(1 AS tinyint) AS TinyInt,
					CAST(NULL AS tinyint) AS NullTinyInt,
					CAST(2 AS smallint) AS SmallInt,
					CAST(3 AS int) AS Int,
					CAST(4 AS bigint) AS BigInt,
					CAST(0 AS bit) AS FalseBool,
					CAST(1 AS bit) AS TrueBool,
					CAST('AB' AS nvarchar) AS NVarchar,
					CAST('A9F04A09-4358-4F31-B815-EC704EE32047' AS UNIQUEIDENTIFIER) AS UniqueIdentifier,
					CAST('CD' AS varchar) AS Varchar,
					CAST('EF' AS char(2)) AS Char,
					CAST('GH' AS nchar(2)) AS NChar,
					CAST(12.345 AS decimal(5, 3)) AS Decimal,
					CAST(23.456 AS money) AS Money,
					CAST(23.456 AS smallmoney) AS SmallMoney,
					CAST(34.567 AS numeric(5, 3)) AS Numeric,
					CAST('<root>Hello</root>' AS xml) AS Xml,
					CAST(123 AS rowversion) AS RowVersion,
					CAST(456 AS timestamp) AS Timestamp,
					CAST('IJ' AS ntext) AS NText,
					CAST('KL' AS text) AS Text,
					CAST('MN' AS image) AS Image,
					CAST('OP' AS binary(2)) AS Binary,
					CAST('QR' AS varbinary(2)) AS VarBinary,
					CAST('12:34:54.1237' AS time(4)) AS Time,
					CAST('1955-12-13 12:43:00' AS smalldatetime) AS SmallDateTime,
					CAST('1912-10-25 12:24:32 +10:0' AS datetimeoffset(3)) AS DateTimeOffset,
					CAST('2007-05-08 12:35:29.1234567' AS datetime2(7)) AS DateTime2,
					CAST('1912-10-25' AS date) AS Date,
					CAST(123.456 AS float) AS Float,
					CAST(789.012 AS real) AS Real
			");
			
			// Verify column count
			Assert.AreEqual(31, ((IDictionary<string, object>)result).Count);

			// Verify column types & values
			Assert.AreEqual(1, result.TinyInt);
			Assert.AreEqual(typeof(byte), result.TinyInt.GetType());

			Assert.AreEqual(null, result.NullTinyInt);

			Assert.AreEqual(2, result.SmallInt);
			Assert.AreEqual(typeof(short), result.SmallInt.GetType());

			Assert.AreEqual(3, result.Int);
			Assert.AreEqual(typeof(int), result.Int.GetType());

			Assert.AreEqual(false, result.FalseBool);
			Assert.AreEqual(typeof(bool), result.FalseBool.GetType());

			Assert.AreEqual(true, result.TrueBool);
			Assert.AreEqual(typeof(bool), result.TrueBool.GetType());

			Assert.AreEqual("AB", result.NVarchar);
			Assert.AreEqual(typeof(string), result.NVarchar.GetType());

			Assert.AreEqual(new Guid("A9F04A09-4358-4F31-B815-EC704EE32047"), result.UniqueIdentifier);
			Assert.AreEqual(typeof(Guid), result.UniqueIdentifier.GetType());

			Assert.AreEqual("CD", result.Varchar);
			Assert.AreEqual(typeof(string), result.Varchar.GetType());

			Assert.AreEqual("EF", result.Char);
			Assert.AreEqual(typeof(string), result.Char.GetType());

			Assert.AreEqual("GH", result.NChar);
			Assert.AreEqual(typeof(string), result.NChar.GetType());

			Assert.AreEqual(12.345m, result.Decimal);
			Assert.AreEqual(typeof(decimal), result.Decimal.GetType());

			Assert.AreEqual(23.456m, result.Money);
			Assert.AreEqual(typeof(decimal), result.Money.GetType());

			Assert.AreEqual(23.456m, result.SmallMoney);
			Assert.AreEqual(typeof(decimal), result.SmallMoney.GetType());

			Assert.AreEqual(34.567, result.Numeric);
			Assert.AreEqual(typeof(decimal), result.Numeric.GetType());

			Assert.AreEqual("<root>Hello</root>", result.Xml);
			Assert.AreEqual(typeof(string), result.Xml.GetType());

			Assert.AreEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 123 }, result.RowVersion);
			Assert.AreEqual(typeof(byte[]), result.RowVersion.GetType());

			Assert.AreEqual(new byte[] { 0, 0, 0, 0, 0, 0, 1, 200 }, result.Timestamp);
			Assert.AreEqual(typeof(byte[]), result.Timestamp.GetType());

			Assert.AreEqual("IJ", result.NText);
			Assert.AreEqual(typeof(string), result.NText.GetType());

			Assert.AreEqual("KL", result.Text);
			Assert.AreEqual(typeof(string), result.Text.GetType());

			Assert.AreEqual(new byte[] { 77, 78 }, result.Image);
			Assert.AreEqual(typeof(byte[]), result.Image.GetType());

			Assert.AreEqual(new byte[] { 79, 80 }, result.Binary);
			Assert.AreEqual(typeof(byte[]), result.Image.GetType());

			Assert.AreEqual(new byte[] { 81, 82 }, result.VarBinary);
			Assert.AreEqual(typeof(byte[]), result.Image.GetType());

			Assert.AreEqual(TimeSpan.FromTicks(452941237000), result.Time);
			Assert.AreEqual(typeof(TimeSpan), result.Time.GetType());

			Assert.AreEqual(Convert.ToDateTime("1955-12-13 12:43:00"), result.SmallDateTime);
			Assert.AreEqual(typeof(DateTime), result.SmallDateTime.GetType());

			Assert.AreEqual(new DateTimeOffset(1912, 10, 25, 12, 24, 32, 0, TimeSpan.FromHours(10)), result.DateTimeOffset);
			Assert.AreEqual(typeof(DateTimeOffset), result.DateTimeOffset.GetType());

			Assert.AreEqual(Convert.ToDateTime("2007-05-08 12:35:29.1234567"), result.DateTime2);
			Assert.AreEqual(typeof(DateTime), result.DateTime2.GetType());

			Assert.AreEqual(Convert.ToDateTime("1912-10-25"), result.Date);
			Assert.AreEqual(typeof(DateTime), result.Date.GetType());

			Assert.AreEqual(123.456, result.Float);
			Assert.AreEqual(typeof(double), result.Float.GetType());

			Assert.AreEqual(789.012f, result.Real);
			Assert.AreEqual(typeof(float), result.Real.GetType());
		}
	}
}