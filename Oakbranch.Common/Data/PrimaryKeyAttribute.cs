using System;

namespace Oakbranch.Common.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute() { }
    }
}
