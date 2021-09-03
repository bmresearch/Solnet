using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Examples
{
    public class ExampleExplorer
    {
        public static void Main(string[] args)
        {
            var examples = Assembly.GetEntryAssembly().GetExportedTypes().Where(t => t.IsAssignableTo(typeof(IExample))).ToList();

            while (true)
            {
                Console.WriteLine("Choose an example to run: ");
                int i = 0;
                foreach (var ex in examples)
                {
                    Console.WriteLine($"{i++}){ex.Name}");
                }

                var option = Console.ReadLine();

                if (int.TryParse(option, out int res) && res <= examples.Count && res >= 0)
                {
                    var t = examples[res];
                    var example = (IExample)t.GetConstructor(Type.EmptyTypes).Invoke(null);

                    example.Run();
                }
                else
                {
                    Console.WriteLine("invalid option");
                }
            }

        }
    }
}