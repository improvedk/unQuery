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

		/// <summary>
		/// Instantiates a DynamicRow object with the provided dictionary as the raw values & schema
		/// </summary>
		internal DynamicRow(Dictionary<string, object> valuesDict)
		{
			this.valuesDict = valuesDict;
		}

		/// <summary>
		/// Supports casting a DynamicRow into a Dictionary<string, object>
		/// </summary>
		public static explicit operator Dictionary<string, object>(DynamicRow row)
		{
			return row.valuesDict;
		}
		
		/// <summary>
		/// Invoked when accessing a property on the dynamic object. Attempts to retrieve the value from
		/// the internal value dictionary.
		/// </summary>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (!valuesDict.TryGetValue(binder.Name, out result))
				throw new ColumnDoesNotExistException(binder.Name);

			return true;
		}
		
		/// <summary>
		/// As we're not supposed to change column values, TrySetMember should never be invoked
		/// </summary>
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			throw new InvalidOperationException("Setting values on DynamicRow is not supported.");
		}
	}
}