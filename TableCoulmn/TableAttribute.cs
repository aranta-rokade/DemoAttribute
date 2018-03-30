using System;

namespace AttributesTableColumn
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; set; }

        public TableAttribute(string TableName)
        {
            this.TableName = TableName;
        }
    }
}
