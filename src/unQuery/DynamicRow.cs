using System;
using System.Collections.Generic;
using System.Dynamic;

namespace unQuery
{
	public class DynamicRow : DynamicObject
	{
		private readonly Dictionary<string, object> valuesDict;
		private readonly object[] values;
		private readonly Dictionary<string, int> fieldMap;

		public DynamicRow(Dictionary<string, object> dict)
		{
			this.valuesDict = dict;
		}

		public DynamicRow(object[] values, Dictionary<string, int> fieldMap)
		{
			this.values = values;
			this.fieldMap = fieldMap;
		}
		
		public static explicit operator Dictionary<string, object>(DynamicRow row)
		{
			if (row.valuesDict != null)
				return row.valuesDict;

			var dict = new Dictionary<string, object>(row.values.Length);

			foreach (var key in row.fieldMap.Keys)
				dict[key] = row.values[row.fieldMap[key]];

			return dict;
		}
		
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (valuesDict != null)
			{
				if (!valuesDict.TryGetValue(binder.Name, out result))
					throw new ColumnDoesNotExistException(binder.Name);

				return true;
			}

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
			throw new InvalidOperationException("Setting values on DynamicRow is not supported.");
		}
	}
}