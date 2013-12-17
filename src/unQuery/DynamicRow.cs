using System;
using System.Collections.Generic;
using System.Dynamic;

namespace unQuery
{
	/// <summary>
	/// Custom dynamic implementation for storing a single row in an internal dictionary.
	/// </summary>
	internal class DynamicRow : DynamicObject
	{
		private readonly Dictionary<string, object> valuesDict;

		internal DynamicRow(Dictionary<string, object> valuesDict)
		{
			this.valuesDict = valuesDict;
		}

		public static explicit operator Dictionary<string, object>(DynamicRow row)
		{
			return row.valuesDict;
		}
		
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (!valuesDict.TryGetValue(binder.Name, out result))
				throw new ColumnDoesNotExistException(binder.Name);

			return true;
		}
		
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			throw new InvalidOperationException("Setting values on DynamicRow is not supported.");
		}
	}
}