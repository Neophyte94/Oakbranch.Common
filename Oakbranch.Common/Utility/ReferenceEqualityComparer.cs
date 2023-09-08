using System;
using System.Collections;
using System.Collections.Generic;

namespace Oakbranch.Common.Utility
{
    public class ReferenceEqualityComparer : IEqualityComparer<object>, IEqualityComparer
    {
        public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

        public new bool Equals(object x, object y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => obj.GetHashCode();

        private ReferenceEqualityComparer() { }
    }
}
