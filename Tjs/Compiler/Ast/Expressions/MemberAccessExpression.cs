﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronTjs.Compiler.Ast
{
	public class DirectMemberAccessExpression : Expression
	{
		public DirectMemberAccessExpression(Expression target, string memberName)
		{
			Target = target;
			MemberName = memberName;
			if (Target != null)
				Target.Parent = this;
		}

		public Expression Target { get; private set; }

		public string MemberName { get; private set; }

		System.Linq.Expressions.Expression TargetExpression
		{
			get
			{
				if (Target != null)
					return Target.TransformRead();
				Node node = this;
				while (node.Parent != null)
				{
					var with = node as WithStatement;
					if (with != null)
						return with.AccessibleVariable;
					node = node.Parent;
				}
				return ((SourceUnitTree)node).Context;
			}
		}

		public override System.Linq.Expressions.Expression TransformRead()
		{
			bool direct = false;
			for (var node = Parent; node != null; node = node.Parent)
			{
				var unary = node as UnaryExpression;
				if (unary != null && unary.ExpressionType == Runtime.Binding.TjsOperationKind.AccessPropertyObject)
				{
					direct = true;
					break;
				}
			}
			return System.Linq.Expressions.Expression.Dynamic(LanguageContext.CreateGetMemberBinder(MemberName, false, direct), typeof(object), TargetExpression);
		}

		public override System.Linq.Expressions.Expression TransformWrite(System.Linq.Expressions.Expression value)
		{
			bool direct = false;
			for (var node = Parent; node != null; node = node.Parent)
			{
				var unary = node as UnaryExpression;
				if (unary != null && unary.ExpressionType == Runtime.Binding.TjsOperationKind.AccessPropertyObject)
				{
					direct = true;
					break;
				}
			}
			return System.Linq.Expressions.Expression.Dynamic(LanguageContext.CreateSetMemberBinder(MemberName, false, true, direct), typeof(object), TargetExpression, value);
		}

		public override System.Linq.Expressions.Expression TransformDelete()
		{
			return System.Linq.Expressions.Expression.Dynamic(LanguageContext.CreateDeleteMemberBinder(MemberName, false, true), typeof(object), TargetExpression);
		}

		public override System.Linq.Expressions.Expression TransformVoid() { return System.Linq.Expressions.Expression.Empty(); }
	}

	public class IndirectMemberAccessExpression : Expression
	{
		public IndirectMemberAccessExpression(Expression target, Expression member)
		{
			Target = target;
			Member = member;
			Target.Parent = Member.Parent = this;
		}

		public Expression Target { get; private set; }

		public Expression Member { get; private set; }

		public override System.Linq.Expressions.Expression TransformRead()
		{
			bool direct = false;
			for (var node = Parent; node != null; node = node.Parent)
			{
				var unary = node as UnaryExpression;
				if (unary != null && unary.ExpressionType == Runtime.Binding.TjsOperationKind.AccessPropertyObject)
				{
					direct = true;
					break;
				}
			}
			return System.Linq.Expressions.Expression.Dynamic(LanguageContext.CreateGetIndexBinder(new CallInfo(2), direct), typeof(object), Target.TransformRead(), Member.TransformRead());
		}

		public override System.Linq.Expressions.Expression TransformWrite(System.Linq.Expressions.Expression value)
		{
			bool direct = false;
			for (var node = Parent; node != null; node = node.Parent)
			{
				var unary = node as UnaryExpression;
				if (unary != null && unary.ExpressionType == Runtime.Binding.TjsOperationKind.AccessPropertyObject)
				{
					direct = true;
					break;
				}
			}
			return System.Linq.Expressions.Expression.Dynamic(LanguageContext.CreateSetIndexBinder(new CallInfo(3), direct), typeof(object), Target.TransformRead(), Member.TransformRead(), value);
		}

		public override System.Linq.Expressions.Expression TransformDelete()
		{
			return System.Linq.Expressions.Expression.Dynamic(LanguageContext.CreateDeleteIndexBinder(new CallInfo(2)), typeof(object), Target.TransformRead(), Member.TransformRead());
		}

		public override System.Linq.Expressions.Expression TransformVoid() { return System.Linq.Expressions.Expression.Empty(); }
	}
}
