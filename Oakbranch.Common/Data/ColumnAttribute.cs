using System;
using System.Collections.Generic;
using System.Text;

namespace Oakbranch.Common.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        private string ColumnName { get; }
        public bool NotNull { get; set; }
        public bool IsUnique { get; set; }

        public ColumnAttribute(string columnName)
        {
            if (String.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            ColumnName = columnName;
            NotNull = false;
            IsUnique = false;
        }
    }
}
