﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JNKJ.Core
{
    /// <summary>
    /// 查询构建器扩展
    /// </summary>
    public static class QueryBuilderExtension
    {
        public static Expression<Func<TEntity, bool>> And<TEntity>(this Expression<Func<TEntity, bool>> first, Expression<Func<TEntity, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }
        public static Expression<Func<TEntity, bool>> Or<TEntity>(this Expression<Func<TEntity, bool>> first, Expression<Func<TEntity, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
        private static Expression<TEntity> Compose<TEntity>(this Expression<TEntity> first, Expression<TEntity> second, Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<TEntity>(merge(first.Body, secondBody), first.Parameters);
        }
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> one)
        {
            var candidateExpr = one.Parameters[0];
            var body = Expression.Not(one.Body);

            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

    }
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }
        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
}
