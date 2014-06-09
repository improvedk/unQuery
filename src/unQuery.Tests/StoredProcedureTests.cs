using NUnit.Framework;
using System.Data;
using unQuery.SqlTypes;

namespace unQuery.Tests
{
	public class StoredProcedureTests : TestFixture
	{
		[Test]
		public void InputParameter()
		{
			DB.Execute("CREATE TABLE InputParameter (ID int)");

			DB.Execute(@"
				CREATE PROCEDURE uspInputParameter
					@Input int
				AS
				BEGIN
					INSERT INTO InputParameter VALUES (@Input)
				END");

			var affectedRows = DB.Execute("uspInputParameter", new { Input = 5 }, new QueryOptions { CommandType = CommandType.StoredProcedure });

			Assert.AreEqual(1, affectedRows);
			Assert.AreEqual(1, DB.GetScalar<int>("SELECT COUNT(*) FROM InputParameter"));
			Assert.AreEqual(5, DB.GetScalar<int>("SELECT TOP 1 ID FROM InputParameter"));
		}

		[Test]
		public void OutputParameter()
		{
			DB.Execute(@"
				CREATE PROCEDURE uspOutputParameter
					@Input int,
					@PlusOne int OUTPUT,
					@PlusTwo int OUTPUT
				AS
				BEGIN
					SET @PlusOne = @Input + 1
					SET @PlusTwo = @Input + 2
				END");

			var parameters = new {
				Input = 5,
				PlusOne = Col.Int(null, ParameterDirection.Output),
				PlusTwo = Col.Int(null, ParameterDirection.Output)
			};

			var affectedRows = DB.Execute("uspOutputParameter", parameters, new QueryOptions { CommandType = CommandType.StoredProcedure });

			Assert.AreEqual(-1, affectedRows);
			Assert.AreEqual(6, parameters.PlusOne.Value);
			Assert.AreEqual(7, parameters.PlusTwo.Value);
		}

		[Test]
		public void InputOutputParameter()
		{
			DB.Execute(@"
				CREATE PROCEDURE uspInputOutputParameter
					@Input int,
					@InputOutput int OUTPUT,
					@Output int OUTPUT
				AS
				BEGIN
					SET @InputOutput = @Input + 1
					SET @Output = @InputOutput + 2
				END");

			var parameters = new {
				Input = 5,
				InputOutput = Col.Int(3, ParameterDirection.InputOutput),
				Output = Col.Int(null, ParameterDirection.Output)
			};

			var affectedRows = DB.Execute("uspInputOutputParameter", parameters, new QueryOptions { CommandType = CommandType.StoredProcedure });

			Assert.AreEqual(-1, affectedRows);
			Assert.AreEqual(6, parameters.InputOutput.Value);
			Assert.AreEqual(8, parameters.Output.Value);
		}

		[Test]
		public void ReturnValueParameter()
		{
			DB.Execute(@"
				CREATE PROCEDURE uspReturnValueParameter
					@Input int
				AS
				BEGIN
					RETURN @Input + 1
				END");

			var parameters = new {
				Input = 5,
				ReturnValue = Col.Int(null, ParameterDirection.ReturnValue)
			};

			var affectedRows = DB.Execute("uspReturnValueParameter", parameters, new QueryOptions { CommandType = CommandType.StoredProcedure });

			Assert.AreEqual(-1, affectedRows);
			Assert.AreEqual(6, parameters.ReturnValue.Value);
		}

		[Test]
		public void ReturnAndResultSet()
		{
			DB.Execute(@"
				CREATE PROCEDURE uspReturnAndResultSet
				AS
				BEGIN
					SELECT 'Test' AS A
					RETURN 5
				END");

			var parameters = new {
				ReturnValue = Col.Int(null, ParameterDirection.ReturnValue)
			};

			var row = DB.GetRow("uspReturnAndResultSet", parameters, new QueryOptions { CommandType = CommandType.StoredProcedure });

			Assert.AreEqual("Test", row.A);
			Assert.AreEqual(5, parameters.ReturnValue.Value);
		}

		[Test]
		public void AllKindsOfParametersAndResults()
		{
			DB.Execute(@"
				CREATE PROCEDURE uspAllKindsOfParametersAndResults
					@Input int,
					@InputOutput int OUTPUT,					
					@Output int OUTPUT
				AS
				BEGIN
					SET @InputOutput = @InputOutput + 1
					SET @Output = @Input + 1

					SELECT 'Hello world' AS A

					RETURN 5
				END");

			var parameters = new {
				Input = 5,
				InputOutput = Col.Int(6, ParameterDirection.InputOutput),
				Output = Col.Int(null, ParameterDirection.Output),
				ReturnValue = Col.Int(null, ParameterDirection.ReturnValue)
			};

			var row = DB.GetRow("uspAllKindsOfParametersAndResults", parameters, new QueryOptions { CommandType = CommandType.StoredProcedure });

			Assert.AreEqual("Hello world", row.A);
			Assert.AreEqual(7, parameters.InputOutput.Value);
			Assert.AreEqual(6, parameters.Output.Value);
			Assert.AreEqual(5, parameters.ReturnValue.Value);
		}
	}
}