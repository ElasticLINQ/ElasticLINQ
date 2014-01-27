using System;
using System.Linq;

namespace TestConsoleApp.Utility
{
    public static class Write
    {
        public static void WriteToConsole<T>(this IQueryable<T> query)
        {
            foreach (var item in query)
                ToConsole(item);
        }

        public static void ToConsole(object value)
        {
            Console.WriteLine();

            if (value is string || value.GetType().IsValueType)
            {
                Console.WriteLine(value.ToString());
                return;
            }

            foreach (var property in value.GetType().GetProperties())
                Console.WriteLine("{0} : {1}", property.Name, property.GetValue(value) ?? "<null>");

            foreach (var field in value.GetType().GetFields())
                Console.WriteLine("{0} : {1}", field.Name, field.GetValue(value) ?? "<null>");
        }
    }
}
