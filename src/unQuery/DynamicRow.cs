using System.Collections.Generic;
using System.Dynamic;

namespace unQuery
{
	public class DynamicRow : DynamicObject
	{
		private Dictionary<string, object> dict;

		public DynamicRow(Dictionary<string, object> dict)
		{
			this.dict = dict;
		}

		public static explicit operator Dictionary<string, object>(DynamicRow row)
		{
			return row.dict;
		}
		
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return dict.TryGetValue(binder.Name, out result);
		}
		
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			dict[binder.Name] = value;
		
			return true;
		}
	}
}