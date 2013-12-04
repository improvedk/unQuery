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
	public class DynamicFieldMapRow : IDynamicMetaObjectProvider
	{
		// This stores the raw column values by ordinal index
		private readonly object[] values;

		// The field map stores the <ColumnName, ColumnOrdinal> map, allowing us to retrieve the value from the values array
		private readonly Dictionary<string, int> fieldMap;

		public DynamicFieldMapRow(object[] values, Dictionary<string, int> fieldMap)
		{
			this.values = values;
			this.fieldMap = fieldMap;
		}

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

		public static explicit operator Dictionary<string, object>(DynamicFieldMapRow row)
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
	/// to the DynamicFieldMapRow.GetColumnValue method.
	/// </summary>
	internal class DynamicFieldMapRowMetaObject : DynamicMetaObject
	{
		private static readonly MethodInfo getColumnValueMethod = typeof(DynamicFieldMapRow).GetMethod("GetColumnValue");

		internal DynamicFieldMapRowMetaObject(Expression parameter, DynamicFieldMapRow value)
			: base(parameter, BindingRestrictions.Empty, value)
		{ }

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