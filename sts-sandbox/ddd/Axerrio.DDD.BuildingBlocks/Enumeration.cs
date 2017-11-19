using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public abstract class Enumeration<T>: ValueObject<T>, IComparable
        where T : Enumeration<T>
    {
        protected readonly static List<T> Items = new List<T>();

        public string Name { get; private set; }
        public int Id { get; private set; }

        protected Enumeration()
        {
        }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var other = obj as Enumeration<T>;
            if (other == null)
                throw new InvalidOperationException(); //ToDo supply message to exception

            return Id.CompareTo(other.Id);
        }

        //public static explicit operator int(Enumeration<T> value)
        //{
        //    return value.Id;
        //}

        //public static explicit operator Enumeration<T>(int value)
        //{
        //    return Parse(value);
        //}

        public static T Parse(int value)
        {
            if (!TryParse(value, out T item))
                throw new OverflowException($"{value} is not an underlying value of the {nameof(T)} enumeration.");

            return item;
        }

        public static bool TryParse(int value, out T item)
        {
            //https://msdn.microsoft.com/en-us/library/essfb559(v=vs.110).aspx

            item = Items.FirstOrDefault(i => i.Id == value);

            if (item == null)
                return false;

            return true;
        }

        //public static implicit operator Enumeration<T>(int value)
        //{
        //    var matchedItem = Items.FirstOrDefault(i => i.Id == value);

        //    if (matchedItem == null)
        //        throw new InvalidCastException();

        //    return matchedItem;
        //}

        //public static implicit operator Enumeration<T>(string value)
        //{
        //    var matchedItem = Items.FirstOrDefault(i => i.Name == value?.ToLowerInvariant());

        //    if (matchedItem == null)
        //        throw new InvalidCastException();

        //    return matchedItem;
        //}
    }
}