using System.Collections.Generic;
using AttributesTableColumn;

namespace Employee_UsingAtrribute
{
    [Table("Employee")]
    public class Employee
    {
        [Column("id", "int", 50, false)]
        public int EID { get; set; }

        [Column("name", "varchar", 50, false)]
        public string EName { get; set; }

        [Column("dept", "varchar", 50, false)]
        public string EDept { get; set; }

        private static List<Employee> empList = new List<Employee>();

        public static List<Employee> GetEmployeeList()
        {
            return empList;
        }
    }
}
