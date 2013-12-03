using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace unQuery
{
	public class DynamicFieldMapRow : IDynamicMetaObjectProvider
	{
		private readonly object[] values;
		private readonly Dictionary<string, int> fieldMap;

		public DynamicFieldMapRow(object[] values, Dictionary<string, int> fieldMap)
		{
			this.values = values;
			this.fieldMap = fieldMap;
		}

		public object GetColumnValue(string name)
		{
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