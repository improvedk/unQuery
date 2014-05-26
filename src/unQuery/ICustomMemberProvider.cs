using System;
using System.Collections.Generic;

namespace LINQPad
{
	/// <summary>
	/// Special interface implementation that allows LINQPad to visualize DynamicRow values. LINQPad will pick up on this interface using duck-typing.
	/// http://www.linqpad.net/faq.aspx
	/// </summary>
	public interface ICustomMemberProvider
	{
		IEnumerable<string> GetNames();
		IEnumerable<Type> GetTypes();
		IEnumerable<object> GetValues();
	}
}