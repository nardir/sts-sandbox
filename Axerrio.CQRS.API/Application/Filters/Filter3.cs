using Axerrio.CQRS.API.Application.Specification;
using Microsoft.AspNet.OData.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Filters
{
    public class FilterContext
    {
        protected FilterContext() { }

        public FilterContext(Type context)
        {
            Type = context;
        }

        private string AssemblyQualifiedName
        {
            get => Type.AssemblyQualifiedName;

            set
            {
                if (Type == null)
                    Type = Type.GetType(value);
            }
        }

        public string Name { get; private set; }

        private Type _type;

        public Type Type
        {
            get => _type;

            private set
            {
                _type = value;

                Name = _type.Name;
            }

        } //Ignore
    }

    public class Filter3EntityTypeConfiguration : IEntityTypeConfiguration<Filter3>
    {
        public void Configure(EntityTypeBuilder<Filter3> builder)
        {
            builder.ToTable("Filter", "dbo");

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id)
                .HasColumnName("FilterId")
                .IsRequired();

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasAlternateKey(f => f.Name);

            builder.Ignore(f => f.Context);
            builder.Property(f => f.ContextName)
                .IsRequired();

            builder.Property("ContextAssemblyQualifiedName")
                .IsRequired()
                .HasColumnType("nvarchar(max)");
        }
    }

    public class Filter3
    {
        protected Filter3() //Used by EF Core
        {
        }

        protected Filter3(Type context, string name)
        {
            Context = context;
            Name = name;
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        #region context

        private string ContextAssemblyQualifiedName
        {
            get => Context.AssemblyQualifiedName;

            set
            {
                Context = Type.GetType(value);
            }
        }

        public string ContextName 
        {
            get => Context.Name;

            private set => ContextName = value;
        }

        [JsonIgnore]
        public Type Context { get; private set; } 

        #endregion
    }

    public class FilterCondition3
    {
        protected FilterCondition3()
        {
        }

        public FilterCondition3(Type context, int filterRow, string condition)
        {
            FilterRow = filterRow;
            Condition = condition;
        }

        public int Id { get; private set; }
        public int FilterRow { get; set; }
        public string Condition { get; set; }
    }

    public class Filter3<T>: Filter3
    {
        public Filter3(string name)
            : base(typeof(T), name)
        {
        }

        public ISpecification<T> Specification()
        {
            return null;
        }

        public Filter3<T> AddFilterCondition(int filterRow, string condition)
        {
            var filterCondition = new FilterCondition3(Context, filterRow, condition);

            //add to collection

            return this;
        }
    }
}