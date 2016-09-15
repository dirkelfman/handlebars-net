﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new PartialConverter().ConvertTokens(sequence).ToList();
        }

        private PartialConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                var partialToken = item as PartialToken;
                if (partialToken != null)
                {
                    var arguments = AccumulateArguments(enumerator);
                    if (arguments.Count == 0)
                    {
                        throw new HandlebarsCompilerException("A partial must have a name");
                    }

                    var partialName = arguments[0];

                    if (partialName is PathExpression)
                    {
                        partialName = Expression.Constant(((PathExpression)partialName).Path);
                    }

                    if (arguments.Count == 1)
                    {
                        yield return HandlebarsExpression.Partial(partialName);
                    }
                    else if (arguments.Count == 2)
                    {
                        yield return HandlebarsExpression.Partial(partialName, arguments[1]);
                    }
                    else
                    {
                        var pathExp = arguments[1] as PathExpression;
                        if ( pathExp != null && pathExp.Path == "this")
                        {
                            yield return HandlebarsExpression.Partial(partialName, arguments.Last());
                        }
                        else
                        {
                            //tbd: merge the multi vals at runtime.
                            yield return HandlebarsExpression.Partial(partialName, arguments[1]);
                            //tbd
                            //throw new HandlebarsCompilerException("A partial can only accept 0 or 1 arguments");
                        }
                        
                    }

                    yield return enumerator.Current;
                }
                else
                {
                    yield return item;
                }
            }
        }

        private static List<Expression> AccumulateArguments(IEnumerator<object> enumerator)
        {
            var item = GetNext(enumerator);
            List<Expression> helperArguments = new List<Expression>();
            while ((item is EndExpressionToken) == false)
            {
                if ((item is Expression) == false)
                {
                    throw new HandlebarsCompilerException(string.Format("Token '{0}' could not be converted to an expression", item));
                }
                helperArguments.Add((Expression)item);
                item = GetNext(enumerator);
            }
            return helperArguments;
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

