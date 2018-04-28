using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.Helpers
{
    public class MemberExtractor : ExpressionVisitor
    {
        private MemberInfo _member = null;

        protected MemberInfo Member => _member;

        public static MemberInfo Extract(LambdaExpression expression)
        {
            if (expression == null)
                return null;

            var extractor = new MemberExtractor();

            extractor.Visit(expression);

            return extractor.Member;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _member = node.Member;

            return node;
        }
    }

    public class MembersExtractor : ExpressionVisitor
    {
        private List<MemberInfo> _members = null;

        protected MembersExtractor()
        {
            _members = new List<MemberInfo>();
        }

        protected List<MemberInfo> Members => _members;

        public static List<MemberInfo> Extract(LambdaExpression expression)
        {
            if (expression == null)
                return null;

            var extractor = new MembersExtractor();

            extractor.Visit(expression);

            return extractor.Members;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _members.Add(node.Member);

            return node;
        }
    }
}
