using NUnit.Framework;
using System;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace unQuery.Tests
{
	public class BatchExecutionerTests : TestFixture
	{
		[Test]
		public void EmptyParameterObject()
		{
			using (var batch = DB.Batch())
			{
				batch.Add("SELECT 2 AS A INTO #TMP", new { });
				Assert.AreEqual(1, batch.Execute());
			}
		}

		[Test]
		public void ExplicitNullParameter()
		{
			using (var batch = DB.Batch())
			{
				batch.Add("SELECT 2 AS A INTO #TMP", null);
				Assert.AreEqual(1, batch.Execute());
			}
		}

		[Test]
		public void ThousandCommand()
		{
			using (var batch = DB.Batch())
			{
				for (int i = 0; i < 1000; i++)
					batch.Add("SELECT @I AS A INTO #TMP", new { I = i });

				Assert.AreEqual(1000, batch.Execute());
			}
		}
		
		[Test]
		public void NonAtomic()
		{
			bool threw = false;

			using (var batch = DB.Batch())
			{
				batch.Add("CREATE TABLE CBF11C591AD94E91A25545FCF32D84ED (ID int)");
				batch.Add("INSERT INTO CBF11C591AD94E91A25545FCF32D84ED VALUES (@A)", new { A = 25 });
				batch.Add("INSERT INTO CBF11C591AD94E91A25545FCF32D84ED VALUES (@Y)", new { B = 25 });
				batch.Add("INSERT INTO CBF11C591AD94E91A25545FCF32D84ED VALUES (@C)", new { C = 25 });
				batch.Add("INSERT INTO CBF11C591AD94E91A25545FCF32D84ED VALUES (@X)", new { D = 25 });
				batch.Add("INSERT INTO CBF11C591AD94E91A25545FCF32D84ED VALUES (@E)", new { E = 25 });
				batch.Add("INSERT INTO CBF11C591AD94E91A25545FCF32D84ED VALUES (@F)", new { F = 25 });

				try
				{
					batch.Execute();
				}
				catch (Exception ex)
				{
					Assert.That(ex.Message.Contains("@Y"));
					Assert.That(ex.Message.Contains("@X"));
					threw = true;
				}
			}

			Assert.IsTrue(threw);
			Assert.AreEqual(4, DB.GetScalar<int>("SELECT COUNT(*) FROM CBF11C591AD94E91A25545FCF32D84ED"));
		}

		[Test]
		public void AtomicWithTransactionScope()
		{
			bool threw = false;

			using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew))
			{
				using (var batch = DB.Batch())
				{
					batch.Add("CREATE TABLE E93BFDF3E03C42C49A80F88AAA4BE23E (ID int)");
					batch.Add("INSERT INTO E93BFDF3E03C42C49A80F88AAA4BE23E VALUES (@A)", new { A = 25 });
					batch.Add("INSERT INTO E93BFDF3E03C42C49A80F88AAA4BE23E VALUES (@Y)", new { B = 25 });
					batch.Add("INSERT INTO E93BFDF3E03C42C49A80F88AAA4BE23E VALUES (@C)", new { C = 25 });
					batch.Add("INSERT INTO E93BFDF3E03C42C49A80F88AAA4BE23E VALUES (@X)", new { D = 25 });
					batch.Add("INSERT INTO E93BFDF3E03C42C49A80F88AAA4BE23E VALUES (@E)", new { E = 25 });
					batch.Add("INSERT INTO E93BFDF3E03C42C49A80F88AAA4BE23E VALUES (@F)", new { F = 25 });

					try
					{
						batch.Execute();
					}
					catch (Exception ex)
					{
						Assert.That(ex.Message.Contains("@Y"));
						Assert.That(ex.Message.Contains("@X"));
						threw = true;
					}
				}
			}

			Assert.IsTrue(threw);
			Assert.Throws<SqlException>(() => DB.GetScalar<int>("SELECT COUNT(*) FROM E93BFDF3E03C42C49A80F88AAA4BE23E"));
		}

		[Test]
		public void MissingParameterProperties()
		{
			using (var batch = DB.Batch())
			{
				batch.Add("SELECT @A AS A INTO #TMP", new { B = 25 });

				Assert.Throws<SqlException>(() => batch.Execute());
			}
		}

		[Test]
		public void SingleCommand()
		{
			using (var batch = DB.Batch())
			{
				batch.Add("SELECT 2 AS A INTO #TMP");

				Assert.AreEqual(1, batch.Execute());
			}
		}

		[Test]
		public void NonExecuted()
		{
			using (var batch = DB.Batch())
			{
				batch.Add("SELECT 2 INTO #TMP");
			}
		}

		[Test]
		public void NoCommands()
		{
			using (var batch = DB.Batch())
				Assert.AreEqual(0, batch.Execute());
		}

		[Test]
		public void ClearsCommandsAfterExecuting()
		{
			using (var batch = DB.Batch())
			{
				Assert.AreEqual(0, batch.CommandCount);

				batch.Add("SELECT 2 AS A INTO #TMP");

				Assert.AreEqual(1, batch.CommandCount);

				batch.Execute();

				Assert.AreEqual(0, batch.CommandCount);
			}
		}
	}
}