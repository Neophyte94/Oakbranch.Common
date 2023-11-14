using System;

namespace Oakbranch.Common.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxLengthAttribute : Attribute
    {
        public int Value { get; }

        public MaxLengthAttribute(int length)
        {
            Value = length;
        }
    }
}
