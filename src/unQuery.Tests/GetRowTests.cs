using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using unQuery.SqlTypes;

namespace unQuery.Tests.OtherNamespace
{
	public class Typed_PrivateSetter
	{
		public int A { get; private set; }
	}

	public class Typed_InternalProperty
	{
		internal int A { get; private set; }
	}
}

namespace unQuery.Tests
{
	public class GetRowTests : TestFixture
	{
		private class SimpleType
		{
			public int A { get; set; }
			public string B { get; set; }
		}

		private class SimpleTypeReverseOrder
		{
			public string B { get; set; }
			public int A { get; set; }
		}

		private class NoProperties
		{ }

		private class MixedPropertiesAndFields
		{
			public int A { get; set; }
			public string B;
		}

		private class Types_ByteArray
		{
			public byte[] Value { get; set; }
			public byte[] Nullable { get; set; }
		}

		private class Types_String
		{
			public string Value { get; set; }
			public string Nullable { get; set; }
		}

		private class Types_Long
		{
			public long Value { get; set; }
			public long? Nullable { get; set; }
			public long? NullableWithValue { get; set; }
		}
		
		private class Types_Bool
		{
			public bool Value { get; set; }
			public bool? Nullable { get; set; }
			public bool? NullableWithValue { get; set; }
		}

		private class Types_DateTime
		{
			public DateTime Value { get; set; }
			public DateTime? Nullable { get; set; }
			public DateTime? NullableWithValue { get; set; }
		}

		private class Types_DateTimeOffset
		{
			public DateTimeOffset Value { get; set; }
			public DateTimeOffset? Nullable { get; set; }
			public DateTimeOffset? NullableWithValue { get; set; }
		}

		private class Types_Decimal
		{
			public decimal Value { get; set; }
			public decimal? Nullable { get; set; }
			public decimal? NullableWithValue { get; set; }
		}

		private class Types_Float
		{
			public float Value { get; set; }
			public float? Nullable { get; set; }
			public float? NullableWithValue { get; set; }
		}

		private class Types_Double
		{
			public double Value { get; set; }
			public double? Nullable { get; set; }
			public double? NullableWithValue { get; set; }
		}

		private class Types_Int
		{
			public int Value { get; set; }
			public int? Nullable { get; set; }
			public int? NullableWithValue { get; set; }
		}

		private class Types_Short
		{
			public short Value { get; set; }
			public short? Nullable { get; set; }
			public short? NullableWithValue { get; set; }
		}

		private class Types_Byte
		{
			public byte Value { get; set; }
			public byte? Nullable { get; set; }
			public byte? NullableWithValue { get; set; }
		}

		private class Types_Guid
		{
			public Guid Value { get; set; }
			public Guid? Nullable { get; set; }
			public Guid? NullableWithValue { get; set; }
		}

		private class Types_TimeSpan
		{
			public TimeSpan Value { get; set; }
			public TimeSpan? Nullable { get; set; }
			public TimeSpan? NullableWithValue { get; set; }
		}

		public class Types_PrivateSetter
		{
			public int A { get; private set; }
		}

		[Test]
		public void StoredProcedure()
		{
			DB.Execute("CREATE PROCEDURE usp_Test @A varchar(10) AS SELECT @A AS A");

			var row = DB.GetRow("usp_Test", new {
				A = Col.VarChar("A", 10)
			}, new QueryOptions {
				CommandType = CommandType.StoredProcedure
			});

			Assert.AreEqual("A", row.A);
		}

		[Test]
		public void GetRow_Typed_NullNotSupported()
		{
			var result = DB.GetRow<SimpleType>("SELECT CAST(NULL AS int) AS A");

			Assert.AreEqual(0, result.A);
		}

		[Test]
		public void GetRow_Typed_PrivateSetter()
		{
			var result = DB.GetRow<Types_PrivateSetter>("SELECT 7 AS A");

			Assert.AreEqual(7, result.A);
		}

		[Test]
		public void GetRow_Typed_InternalProperty()
		{
			Assert.Throws<ObjectHasNoPropertiesException>(() => DB.GetRow<OtherNamespace.Typed_InternalProperty>("SELECT 7 AS A"));
		}

		[Test]
		public void GetRow_Typed_PrivateSetter_OtherNamespace()
		{
			var result = DB.GetRow<OtherNamespace.Typed_PrivateSetter>("SELECT 7 AS A");

			Assert.AreEqual(7, result.A);
		}

		[Test]
		public void GetRow_Typed_Time()
		{
			var result = DB.GetRow<Types_TimeSpan>(@"SELECT
				CAST('12:13:14.123' AS time(4)) AS Value,
				CAST(NULL AS time) AS Nullable,
				CAST('13:14:15.234' AS time(3)) AS NullableWithValue");
			
			Assert.AreEqual(new TimeSpan(0, 12, 13, 14, 123), result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(new TimeSpan(0, 13, 14, 15, 234), result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_UniqueIdentifier()
		{
			var result = DB.GetRow<Types_Guid>(@"SELECT
				CAST('787FA919-EFA0-49E5-9EA9-1C354B94AF3E' AS uniqueidentifier) AS Value,
				CAST(NULL AS uniqueidentifier) AS Nullable,
				CAST('6E21F2D7-21EE-4CF8-BA1E-082E60F04DC4' AS uniqueidentifier) AS NullableWithValue");

			Assert.AreEqual(new Guid("787FA919-EFA0-49E5-9EA9-1C354B94AF3E"), result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(new Guid("6E21F2D7-21EE-4CF8-BA1E-082E60F04DC4"), result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_Int()
		{
			var result = DB.GetRow<Types_Int>(@"SELECT
				CAST(1 AS int) AS Value,
				CAST(NULL AS int) AS Nullable,
				CAST(2 AS int) AS NullableWithValue");

			Assert.AreEqual(1, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(2, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_SmallInt()
		{
			var result = DB.GetRow<Types_Short>(@"SELECT
				CAST(1 AS smallint) AS Value,
				CAST(NULL AS smallint) AS Nullable,
				CAST(2 AS smallint) AS NullableWithValue");

			Assert.AreEqual(1, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(2, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_TinyInt()
		{
			var result = DB.GetRow<Types_Byte>(@"SELECT
				CAST(1 AS tinyint) AS Value,
				CAST(NULL AS tinyint) AS Nullable,
				CAST(2 AS tinyint) AS NullableWithValue");

			Assert.AreEqual(1, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(2, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_NChar()
		{
			var result = DB.GetRow<Types_String>(@"SELECT
				CAST('ABC' AS nchar(3)) AS Value,
				CAST(NULL AS nchar) AS Nullable");

			Assert.AreEqual("ABC", result.Value);
			Assert.AreEqual(null, result.Nullable);
		}

		[Test]
		public void GetRow_Typed_NVarChar()
		{
			var result = DB.GetRow<Types_String>(@"SELECT
				CAST('ABC' AS nvarchar) AS Value,
				CAST(NULL AS nvarchar) AS Nullable");

			Assert.AreEqual("ABC", result.Value);
			Assert.AreEqual(null, result.Nullable);
		}

		[Test]
		public void GetRow_Typed_Xml()
		{
			var result = DB.GetRow<Types_String>(@"SELECT
				CAST('<root />' AS xml) AS Value,
				CAST(NULL AS xml) AS Nullable");

			Assert.AreEqual("<root />", result.Value);
			Assert.AreEqual(null, result.Nullable);
		}

		[Test]
		public void GetRow_Typed_VarChar()
		{
			var result = DB.GetRow<Types_String>(@"SELECT
				CAST('ABC' AS varchar) AS Value,
				CAST(NULL AS varchar) AS Nullable");

			Assert.AreEqual("ABC", result.Value);
			Assert.AreEqual(null, result.Nullable);
		}

		[Test]
		public void GetRow_Typed_NText()
		{
			var result = DB.GetRow<Types_String>(@"SELECT
				CAST('ABC' AS ntext) AS Value,
				CAST(NULL AS ntext) AS Nullable");

			Assert.AreEqual("ABC", result.Value);
			Assert.AreEqual(null, result.Nullable);
		}

		[Test]
		public void GetRow_Typed_Text()
		{
			var result = DB.GetRow<Types_String>(@"SELECT
				CAST('ABC' AS text) AS Value,
				CAST(NULL AS text) AS Nullable");

			Assert.AreEqual("ABC", result.Value);
			Assert.AreEqual(null, result.Nullable);
		}

		[Test]
		public void GetRow_Typed_Float()
		{
			var result = DB.GetRow<Types_Double>(@"SELECT
				CAST(5.237 AS float) AS Value,
				CAST(NULL AS float) AS Nullable,
				CAST(102.2 AS float) AS NullableWithValue");

			Assert.AreEqual(5.237d, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(102.2d, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_Real()
		{
			var result = DB.GetRow<Types_Float>(@"SELECT
				CAST(5.237 AS real) AS Value,
				CAST(NULL AS real) AS Nullable,
				CAST(102.2 AS real) AS NullableWithValue");

			Assert.AreEqual(5.237f, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(102.2f, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_Binary()
		{
			var result = DB.GetRow<Types_ByteArray>(@"SELECT
				CAST(0x2522 AS binary(2)) AS Value,
				CAST(NULL AS binary) AS Nullable");

			Assert.AreEqual(new byte[] { 0x25, 0x22 }, result.Value);
			Assert.AreEqual(null, result.Nullable);
		}

		[Test]
		public void GetRow_Typed_Image()
		{
			var result = DB.GetRow<Types_ByteArray>(@"SELECT
				CAST(0x2522 AS image) AS Value,
				CAST(NULL AS image) AS Nullable");

			Assert.AreEqual(new byte[] { 0x25, 0x22 }, result.Value);
			Assert.AreEqual(null, result.Nullable);
		}

		[Test]
		public void GetRow_Typed_VarBinary()
		{
			var result = DB.GetRow<Types_ByteArray>(@"SELECT
				CAST(0x2522 AS varbinary) AS Value,
				CAST(NULL AS varbinary) AS Nullable");

			Assert.AreEqual(new byte[] { 0x25, 0x22 }, result.Value);
			Assert.AreEqual(null, result.Nullable);
		}

		[Test]
		public void GetRow_Typed_Char()
		{
			var result = DB.GetRow<Types_String>(@"SELECT
				CAST('ABC' AS char(3)) AS Value,
				CAST(NULL AS char) AS Nullable");

			Assert.AreEqual("ABC", result.Value);
			Assert.AreEqual(null, result.Nullable);
		}

		[Test]
		public void GetRow_Typed_BigInt()
		{
			var result = DB.GetRow<Types_Long>(@"SELECT
				CAST(1 AS bigint) AS Value,
				CAST(NULL AS bigint) AS Nullable,
				CAST(2 AS bigint) AS NullableWithValue");
				   
			Assert.AreEqual(1, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(2, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_Decimal()
		{
			var result = DB.GetRow<Types_Decimal>(@"SELECT
				CAST(5.273 AS decimal(4,3)) AS Value,
				CAST(NULL AS decimal) AS Nullable,
				CAST(493123.23 AS decimal(10,2)) AS NullableWithValue");
				   
			Assert.AreEqual(5.273m, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(493123.23, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_Numeric()
		{
			var result = DB.GetRow<Types_Decimal>(@"SELECT
				CAST(5.273 AS numeric(4,3)) AS Value,
				CAST(NULL AS numeric) AS Nullable,
				CAST(493123.23 AS numeric(10,2)) AS NullableWithValue");
				   
			Assert.AreEqual(5.273m, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(493123.23, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_Money()
		{
			var result = DB.GetRow<Types_Decimal>(@"SELECT
				CAST(5.273 AS money) AS Value,
				CAST(NULL AS money) AS Nullable,
				CAST(493123.23 AS money) AS NullableWithValue");
				   
			Assert.AreEqual(5.273m, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(493123.23, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_SmallMoney()
		{
			var result = DB.GetRow<Types_Decimal>(@"SELECT
				CAST(5.273 AS smallmoney) AS Value,
				CAST(NULL AS smallmoney) AS Nullable,
				CAST(493.23 AS smallmoney) AS NullableWithValue");
				   
			Assert.AreEqual(5.273m, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(493.23m, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_Bit()
		{
			var result = DB.GetRow<Types_Bool>(@"SELECT
				CAST(1 AS bit) AS Value,
				CAST(NULL AS bit) AS Nullable,
				CAST(0 AS bit) AS NullableWithValue");

			Assert.AreEqual(true, result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(false, result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_Date()
		{
			var result = DB.GetRow<Types_DateTime>(@"SELECT
				CAST('2013-11-27' AS date) AS Value,
				CAST(NULL AS date) AS Nullable,
				CAST('2014-01-22' AS date) AS NullableWithValue");

			Assert.AreEqual(new DateTime(2013, 11, 27), result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(new DateTime(2014, 01, 22), result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_SmallDateTime()
		{
			var result = DB.GetRow<Types_DateTime>(@"SELECT
				CAST('2013-11-27 14:15:16' AS smalldatetime) AS Value,
				CAST(NULL AS smalldatetime) AS Nullable,
				CAST('2014-01-22 15:16:17' AS smalldatetime) AS NullableWithValue");

			Assert.AreEqual(new DateTime(2013, 11, 27, 14, 15, 00), result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(new DateTime(2014, 01, 22, 15, 16, 00), result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed_DateTimeOffset()
		{
			var result = DB.GetRow<Types_DateTimeOffset>(@"SELECT
				CAST('2012-11-10 1:2:3:003 +02:00' AS datetimeoffset) AS Value,
				CAST(NULL AS datetimeoffset) AS Nullable,
				CAST('2013-12-11 2:3:4:006 +03:00' AS datetimeoffset) AS NullableWithValue");

			Assert.AreEqual(new DateTimeOffset(new DateTime(2012, 11, 10, 1, 2, 3, 003), TimeSpan.FromHours(2)), result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(new DateTimeOffset(new DateTime(2013, 12, 11, 2, 3, 4, 006), TimeSpan.FromHours(3)), result.NullableWithValue);
		}
		
		[Test]
		public void GetRow_Typed_DateTime()
		{
			var result = DB.GetRow<Types_DateTime>(@"SELECT
				CAST('2013-11-27 14:15:16:123' AS datetime) AS Value,
				CAST(NULL AS datetime) AS Nullable,
				CAST('2014-01-22 10:11:12:997' AS datetime) AS NullableWithValue");

			Assert.AreEqual(new DateTime(2013, 11, 27, 14, 15, 16, 123), result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(new DateTime(2014, 01, 22, 10, 11, 12, 997), result.NullableWithValue);
		}
		
		[Test]
		public void GetRow_Typed_DateTime2()
		{
			var result = DB.GetRow<Types_DateTime>(@"SELECT
				CAST('2013-11-27 14:15:16:123' AS datetime2) AS Value,
				CAST(NULL AS datetime2) AS Nullable,
				CAST('2014-01-22 10:11:12:997' AS datetime2) AS NullableWithValue");

			Assert.AreEqual(new DateTime(2013, 11, 27, 14, 15, 16, 123), result.Value);
			Assert.AreEqual(null, result.Nullable);
			Assert.AreEqual(new DateTime(2014, 01, 22, 10, 11, 12, 997), result.NullableWithValue);
		}

		[Test]
		public void GetRow_Typed()
		{
			var result = DB.GetRow<SimpleType>("SELECT 2 AS A, 'Hello' AS B");
			
			Assert.AreEqual(2, result.A);
			Assert.AreEqual("Hello", result.B);
		}

		[Test]
		public void GetRow_Typed_SpacesInColumnName()
		{
			var result = DB.GetRow<SimpleType>("SELECT 2 AS A, 5 AS [A Spaced Out Name], 'Hello' AS B");
			Assert.AreEqual(2, result.A);
			Assert.AreEqual("Hello", result.B);
		}

		[Test]
		public void GetRow_Typed_NonUniqueColumnNames()
		{
			var result = DB.GetRow<SimpleType>("SELECT 2 AS A, 5 AS X, 7 AS X");
			Assert.AreEqual(2, result.A);
			Assert.AreEqual(null, result.B);

			Assert.Throws<DuplicateColumnException>(() => DB.GetRow<SimpleType>("SELECT 3 AS A, 4 AS A, 'Hello' AS B"));
		}

		[Test]
		public void GetRow_Typed_UnnamedColumns()
		{
			var result = DB.GetRow<SimpleType>("SELECT 2 AS A, 'Hello' AS B, 5, 'x'");
			Assert.AreEqual(2, result.A);
			Assert.AreEqual("Hello", result.B);

			result = DB.GetRow<SimpleType>("SELECT 5, 2, 'Hello 2' AS B");
			Assert.AreEqual(0, result.A);
			Assert.AreEqual("Hello 2", result.B);

			result = DB.GetRow<SimpleType>("SELECT NULL");
			Assert.AreEqual(0, result.A);
			Assert.AreEqual(null, result.B);
		}

		[Test]
		public void GetRow_Typed_DifferingColumnOrder()
		{
			unQueryDB.ResetCache();

			var type = typeof(unQueryDB);
			var field = type.GetField("typeWriterCache", BindingFlags.NonPublic | BindingFlags.Static);
			var value = (ConcurrentDictionary<string, Action<object, object[]>>)field.GetValue(null);

			Assert.AreEqual(0, value.Count);

			var result = DB.GetRow<SimpleType>("SELECT 1 AS A, 'Hello 1' AS B");
			Assert.AreEqual(1, result.A);
			Assert.AreEqual("Hello 1", result.B);

			Assert.AreEqual(1, value.Count);

			result = DB.GetRow<SimpleType>("SELECT 'Hello 2' AS B, 2 AS A");
			Assert.AreEqual(2, result.A);
			Assert.AreEqual("Hello 2", result.B);

			Assert.AreEqual(2, value.Count);

			result = DB.GetRow<SimpleType>("SELECT 3 AS A, 'Hello 3' AS B");
			Assert.AreEqual(3, result.A);
			Assert.AreEqual("Hello 3", result.B);

			Assert.AreEqual(2, value.Count);

			var result2 = DB.GetRow<SimpleTypeReverseOrder>("SELECT 4 AS A, 'Hello 4' AS B");
			Assert.AreEqual(4, result2.A);
			Assert.AreEqual("Hello 4", result2.B);

			Assert.AreEqual(3, value.Count);
		}

		[Test]
		public void GetRow_Typed_TypeMismatch()
		{
			Assert.Throws<TypeMismatchException>(() => DB.GetRow<SimpleType>("SELECT 'Hello' AS A"));
		}

		[Test]
		public void GetRow_Typed_MixedPropertiesAndFields()
		{
			var result = DB.GetRow<MixedPropertiesAndFields>("SELECT 2 AS A, 'Hello' AS C");

			Assert.AreEqual(2, result.A);
			Assert.IsNull(result.B);
		}

		[Test]
		public void GetRow_Typed_NoProperties()
		{
			Assert.Throws<ObjectHasNoPropertiesException>(() => DB.GetRow<NoProperties>("SELECT 2 AS A"));
		}

		[Test]
		public void GetRow_Typed_NoMatchingProperties()
		{
			var result = DB.GetRow<SimpleType>("SELECT 2 AS C");
			
			Assert.AreEqual(0, result.A);
			Assert.IsNull(result.B);
		}

		[Test]
		public void GetRow_NoResults()
		{
			var result = DB.GetRow("SELECT * FROM Persons WHERE 1 = 0");

			Assert.IsNull(result);
		}

		[Test]
		public void GetRow_CaseInsensitive()
		{
			var result = DB.GetRow("SELECT Age FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander", 128) });

			Assert.AreEqual(55, result.Age);
			Assert.AreEqual(55, result.age);
			Assert.AreEqual(55, result.AGE);
		}
	
		[Test]
		public void GetRow_SingleColumn()
		{
			var result = DB.GetRow("SELECT Age FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Stefanie Alexander", 128) });

			Assert.AreEqual(55, result.Age);
			Assert.AreEqual(1, ((Dictionary<string, object>)result).Count);
		}

		[Test]
		public void GetRow_MultipleColumns()
		{
			var result = DB.GetRow("SELECT Age, Sex FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Daniel Gallagher", 128) });

			Assert.AreEqual(25, result.Age);
			Assert.AreEqual("M", result.Sex);
			Assert.AreEqual(2, ((Dictionary<string, object>)result).Count);
		}

		[Test]
		public void GetRow_AllColumns()
		{
			var result = DB.GetRow("SELECT * FROM Persons WHERE Name = @Name", new { Name = Col.NVarChar("Annie Brennan", 128) });

			Assert.AreEqual(5, result.PersonID);
			Assert.AreEqual("Annie Brennan", result.Name);
			Assert.AreEqual(23, result.Age);
			Assert.AreEqual("M", result.Sex);
			Assert.AreEqual(Convert.ToDateTime("1984-01-07 13:24:42.110"), result.SignedUp);
			Assert.AreEqual(5, ((Dictionary<string, object>)result).Count);
		}

		[Test]
		public void GetRow_MultipleResultsPossible()
		{
			var result = DB.GetRow("SELECT * FROM Persons ORDER BY PersonID");

			Assert.AreEqual(1, result.PersonID);
		}
	}
}