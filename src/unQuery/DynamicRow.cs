using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace unQuery
{
	/// <summary>
	/// Custom dynamic implementation that stores a single row, with a reference to a common field map so multiple rows don't have
	/// to store the same schema.
	/// </summary>
	internal class DynamicRow : IDynamicMetaObjectProvider
	{
		// This stores the raw column values by ordinal index
		private readonly object[] values;

		// The field map stores the <ColumnName, ColumnOrdinal> map, allowing us to retrieve the value from the values array
		private readonly Dictionary<string, int> fieldMap;

		/// <summary>
		/// Instantiates a DynamicRow with the specified values and field map
		/// </summary>
		/// <param name="values">The raw array of values</param>
		/// <param name="fieldMap">A dictionary that maps a column name to an array index where the value is stored</param>
		internal DynamicRow(object[] values, Dictionary<string, int> fieldMap)
		{
			this.values = values;
			this.fieldMap = fieldMap;
		}

		/// <summary>
		/// Returns the value for a specific column
		/// </summary>
		/// <param name="name">The name of the column</param>
		public object GetColumnValue(string name)
		{
			// Let's take advantage of the fact that 99.9% of column access will succeed. Rather than expecting failure,
			// let's optimize for the best case and catch the rare misses.
			try
			{
				return values[fieldMap[name]];
			}
			catch (KeyNotFoundException)
			{
				throw new ColumnDoesNotExistException(name);
			}
		}

		/// <summary>
		/// Allows casting Dictionary<string, object> into a DynamicRow directly
		/// </summary>
		public static explicit operator Dictionary<string, object>(DynamicRow row)
		{
			var dict = new Dictionary<string, object>(row.values.Length);

			foreach (var key in row.fieldMap.Keys)
				dict[key] = row.values[row.fieldMap[key]];

			return dict;
		}

		DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
		{
			return new DynamicFieldMapRowMetaObject(parameter, this);
		}
	}

	/// <summary>
	/// Plumbing class to properly implement IDynamicMetaObjectProvider. All it does is to map the property access
	/// to the DynamicRow.GetColumnValue method.
	/// </summary>
	internal class DynamicFieldMapRowMetaObject : DynamicMetaObject
	{
		/// <summary>
		/// Static reference to the GetColumnValue method which retrieves the value of a specific column. This is the
		/// one that'll be called when we access dynamic properties.
		/// </summary>
		private static readonly MethodInfo getColumnValueMethod = typeof(DynamicRow).GetMethod("GetColumnValue");
		
		internal DynamicFieldMapRowMetaObject(Expression parameter, DynamicRow value)
			: base(parameter, BindingRestrictions.Empty, value)
		{ }

		/// <summary>
		/// This wires up the dynamic property expressions to the GetColumnValue method
		/// </summary>
		public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
		{
			var parameters = new Expression[] { Expression.Constant(binder.Name) };

			return new DynamicMetaObject(
				Expression.Call(Expression.Convert(Expression, LimitType), getColumnValueMethod, parameters),
				BindingRestrictions.GetTypeRestriction(Expression, LimitType)
			);
		}

		/// <summary>
		/// As we currently do not allow setting values on rows, BindSetMember should never be called.
		/// </summary>
		public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// As we currently do not allow calling methods on rows, BindInvokeMember should never be called.
		/// </summary>
		public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
		{
			throw new InvalidCastException();
		}
	}
}