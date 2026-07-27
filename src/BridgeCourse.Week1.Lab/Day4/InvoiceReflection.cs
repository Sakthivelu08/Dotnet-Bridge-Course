using System;
using System.Reflection;
using System.Text;

namespace BridgeCourse.Week1.Lab.Day4
{
    /// <summary>
    /// Model class representing an Invoice.
    /// </summary>
    public class Invoice
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }

        /// <summary>
        /// Explicit constructor with parameter list.
        /// </summary>
        public Invoice(int id, string customerName, decimal amount)
        {
            Id = id;
            CustomerName = customerName;
            Amount = amount;
        }

        public decimal CalculateTax(decimal taxRate)
        {
            return Amount * taxRate;
        }

        public string GetSummary()
        {
            return $"Invoice #{Id} for {CustomerName}: Total {Amount:C}";
        }
    }

    /// <summary>
    /// Helper to reflect and manipulate the Invoice type.
    /// </summary>
    public static class InvoiceReflector
    {
        /// <summary>
        /// Reflects over the Invoice class, printing class name, property details, method names, and constructor parameters.
        /// </summary>
        public static string ReflectAndInspect()
        {
            var sb = new StringBuilder();
            Type type = typeof(Invoice);

            sb.AppendLine($"Class Name: {type.FullName}");

            sb.AppendLine("\nProperties:");
            foreach (PropertyInfo prop in type.GetProperties())
            {
                sb.AppendLine($"  - Name: {prop.Name}, Type: {prop.PropertyType.Name}");
            }

            sb.AppendLine("\nMethods (Declared):");
            // BindingFlags.DeclaredOnly filters out methods inherited from System.Object
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (MethodInfo method in methods)
            {
                sb.AppendLine($"  - Name: {method.Name}, Return Type: {method.ReturnType.Name}");
            }

            sb.AppendLine("\nConstructors:");
            foreach (ConstructorInfo ctor in type.GetConstructors())
            {
                var parameters = ctor.GetParameters();
                var paramList = string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                sb.AppendLine($"  - Constructor: Invoice({paramList})");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates an Invoice instance via reflection (Activator.CreateInstance)
        /// and sets the 'CustomerName' property via PropertyInfo.SetValue.
        /// </summary>
        public static Invoice CreateAndModifyViaReflection(int id, string originalName, decimal amount, string newName)
        {
            Type type = typeof(Invoice);

            // 1. Instantiation via reflection
            // Activator.CreateInstance locates the matching constructor arguments dynamically.
            object? invoiceObj = Activator.CreateInstance(type, new object[] { id, originalName, amount });
            if (invoiceObj == null)
            {
                throw new InvalidOperationException("Failed to instantiate Invoice via reflection.");
            }

            // 2. Set property value via reflection
            PropertyInfo? propInfo = type.GetProperty(nameof(Invoice.CustomerName));
            if (propInfo == null)
            {
                throw new InvalidOperationException("Property 'CustomerName' not found on Invoice.");
            }

            // Update the name property on the dynamic object instance
            propInfo.SetValue(invoiceObj, newName);

            // Cast and return
            return (Invoice)invoiceObj;
        }
    }
}
