﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace Handlebars.Compiler
{
    internal class HelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new HelperFunctionBinder(context).Visit(expr);
        }

        private readonly CompilationContext _context;

        private HelperFunctionBinder(CompilationContext context)
        {
            _context = context;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Variables,
                node.Expressions.Select(expr => Visit(expr)));
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if(sex.Body is HelperExpression)
            {
                return Visit(sex.Body);
            }
            else
            {
                return sex;
            }
        }

        protected override Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            return HandlebarsExpression.BlockHelper(
                bhex.HelperName,
                bhex.Arguments.Select(arg => Visit(arg)),
                Visit(bhex.Body));
        }

        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            if (_context.Configuration.Helpers.ContainsKey(hex.HelperName))
            {
                var helper = _context.Configuration.Helpers[hex.HelperName];
                var arguments = new Expression[] {
                    Expression.Property(
                        _context.BindingContext,
                        typeof(BindingContext).GetProperty("TextWriter")),
                    Expression.Property(
                        _context.BindingContext,
                        typeof(BindingContext).GetProperty("Value")),
                    Expression.NewArrayInit(typeof(object), hex.Arguments)
                };
                if(helper.Target != null)
                {
                    return Expression.Call(
                        Expression.Constant(helper.Target),
                        helper.Method,
                        arguments);
                }
                else
                {
                    return Expression.Call(
                        helper.Method,
                        arguments);
                }
            }
            else
            {
				return Expression.Call(
					Expression.Constant(this),
					new Action<BindingContext, string, IEnumerable<object>>(LateBindHelperExpression).Method,
					_context.BindingContext,
					Expression.Constant(hex.HelperName),
					Expression.NewArrayInit(typeof(object), hex.Arguments));
            }
        }

		private void LateBindHelperExpression(
			BindingContext context,
			string helperName,
			IEnumerable<object> arguments)
		{
			if(_context.Configuration.Helpers.ContainsKey(helperName))
			{
				var helper = _context.Configuration.Helpers[helperName];
				helper(context.TextWriter, context.Value, arguments.ToArray());
			}
			else
			{
				throw new HandlebarsRuntimeException("Template references a helper that is not registered");
			}
		}
    }
}