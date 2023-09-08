using System;
using System.Collections.Generic;
using System.Text;

namespace Oakbranch.Common.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RelationAttribute : Attribute
    {
        public string ThisKey { get; }
        public string OtherKey { get;}
        public Type OtherType { get; }
        public string LazyLoader { get; set; }

        public RelationAttribute(string ThisKey, string OtherKey, Type OtherType, string LazyLoader = null)
        {
            if (String.IsNullOrWhiteSpace(ThisKey)) throw new ArgumentNullException(nameof(ThisKey));
            if (String.IsNullOrWhiteSpace(OtherKey)) throw new ArgumentNullException(nameof(OtherKey));
            this.ThisKey = ThisKey;
            this.OtherKey = OtherKey;
            this.OtherType = OtherType ?? throw new ArgumentNullException(nameof(OtherType));
            this.LazyLoader = LazyLoader;
        }
    }
}
