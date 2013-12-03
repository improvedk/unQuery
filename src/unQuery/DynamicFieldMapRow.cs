using System;
using System.Collections.Generic;
using System.Dynamic;

namespace unQuery
{
	public class DynamicFieldMapRow : DynamicObject
	{
		private readonly object[] values;
		private readonly Dictionary<string, int> fieldMap;

		public DynamicFieldMapRow(object[] values, Dictionary<string, int> fieldMap)
		{
			this.values = values;
			this.fieldMap = fieldMap;
		}

		public static explicit operator Dictionary<string, object>(DynamicFieldMapRow row)
		{
			var dict = new Dictionary<string, object>(row.values.Length);

			foreach (var key in row.fieldMap.Keys)
				dict[key] = row.values[row.fieldMap[key]];

			return dict;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			try
			{
				result = values[fieldMap[binder.Name]];
				return true;
			}
			catch (KeyNotFoundException)
			{
				throw new ColumnDoesNotExistException(binder.Name);
			}
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			throw new InvalidOperationException("Setting values on DynamicFieldMapRow is not supported.");
		}
	}
}