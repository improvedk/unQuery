﻿using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;

namespace unQuery.Tests
{
	public partial class unQueryTests
	{
		[Test]
		public void AddParameterToCommand_Byte()
		{
			var cmd = new SqlCommand();

			db.AddParametersToCommand(cmd, new {
				Age = (byte)55
			});

			var param = cmd.Parameters[0];

			Assert.AreEqual("@Age", param.ParameterName);
			Assert.AreEqual(SqlDbType.TinyInt, param.SqlDbType);
			Assert.AreEqual(55, (byte)param.Value);
		}

		[Test]
		public void AddParameterToCommand_NullableByte()
		{
			var cmd = new SqlCommand();

			db.AddParametersToCommand(cmd, new {
				Age = (byte?)null
			});

			var param = cmd.Parameters[0];

			Assert.AreEqual("@Age", param.ParameterName);
			Assert.AreEqual(SqlDbType.TinyInt, param.SqlDbType);
			Assert.AreEqual(DBNull.Value, param.Value);
		}
	}
}