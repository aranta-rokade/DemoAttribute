using System;

namespace AttributesTableColumn
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }
        public string ColumnDataType { get; set; }
        public int ColumnSize { get; set; }
        public bool ColumnAllowNulls { get; set; }

        public ColumnAttribute(string CName, string CDataType, int CSize, bool CAllowNulls)
        {
            this.ColumnName = CName;
            this.ColumnDataType = CDataType;
            this.ColumnSize = CSize;
            this.ColumnAllowNulls = CAllowNulls;  
        }
    }
}
