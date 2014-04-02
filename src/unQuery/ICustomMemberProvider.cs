using System;
using System.Collections.Generic;

namespace LINQPad
{
	/// <summary>
	/// Special interface implementation that allows LINQPad to visualize DynamicRow values
	/// </summary>
	public interface ICustomMemberProvider
	{
		IEnumerable<string> GetNames();
		IEnumerable<Type> GetTypes();
		IEnumerable<object> GetValues();
	}
}