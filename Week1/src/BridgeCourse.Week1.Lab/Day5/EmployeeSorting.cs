using System;
using System.Collections.Generic;

namespace BridgeCourse.Week1.Lab.Day5
{
    /// <summary>
    /// Employee class implementing IComparable to establish default sort order by Salary.
    /// </summary>
    public class Employee : IComparable<Employee>
    {
        public string Name { get; }
        public decimal Salary { get; }

        public Employee(string name, decimal salary)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Salary = salary;
        }

        /// <summary>
        /// IComparable implementation. Orders employees by salary ascending.
        /// </summary>
        public int CompareTo(Employee? other)
        {
            if (other == null) return 1;
            
            // Compare salaries ascending
            return Salary.CompareTo(other.Salary);
        }

        public override string ToString()
        {
            return $"{Name} ({Salary:C})";
        }
    }

    /// <summary>
    /// Custom comparer implementing IComparer to sort employees alphabetically by Name.
    /// </summary>
    public class EmployeeNameComparer : IComparer<Employee>
    {
        /// <summary>
        /// IComparer implementation. Orders employees alphabetically by name.
        /// </summary>
        public int Compare(Employee? x, Employee? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Demo runner for testing Employee IComparable & IComparer sorting.
    /// </summary>
    public static class EmployeeSorterDemo
    {
        /// <summary>
        /// Instantiates 10 employees and demonstrates sorting them by salary and by name.
        /// </summary>
        public static List<Employee> GetSampleEmployees()
        {
            return new List<Employee>
            {
                new Employee("John", 55000m),
                new Employee("Alice", 85000m),
                new Employee("Bob", 42000m),
                new Employee("Charlie", 120000m),
                new Employee("Eva", 62000m),
                new Employee("David", 48000m),
                new Employee("Grace", 95000m),
                new Employee("Frank", 51000m),
                new Employee("Ivan", 110000m),
                new Employee("Heidi", 72000m)
            };
        }
    }
}
