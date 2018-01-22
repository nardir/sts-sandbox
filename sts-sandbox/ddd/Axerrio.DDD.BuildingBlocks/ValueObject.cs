using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public abstract class ValueObject<T> : IEquatable<T>
        where T : ValueObject<T>
    {
        protected static bool EqualOperator(ValueObject<T> left, ValueObject<T> right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null)) //https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/xor-operator
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        protected static bool NotEqualOperator(ValueObject<T> left, ValueObject<T> right)
        {
            return !(EqualOperator(left, right));
        }

        public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        {
            return EqualOperator(left, right);
        }

        public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        {
            return NotEqualOperator(left, right);
        }

        protected abstract IEnumerable<object> GetMemberValues();

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            T other = obj as T;

            return Equals(other);
        }

        public virtual bool Equals(T other)
        {
            if (other == null || other.GetType() != GetType())
            {
                return false;
            }

            IEnumerator<object> thisValues = GetMemberValues().GetEnumerator();
            IEnumerator<object> otherValues = other.GetMemberValues().GetEnumerator();
            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (ReferenceEquals(thisValues.Current, null) ^ ReferenceEquals(otherValues.Current, null))
                {
                    return false;
                }
                if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
                {
                    return false;
                }
            }
            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        public override int GetHashCode()
        {
            return GetMemberValues()
             .Select(x => x != null ? x.GetHashCode() : 0)
             .Aggregate((x, y) => x ^ y);
        }
    }
}