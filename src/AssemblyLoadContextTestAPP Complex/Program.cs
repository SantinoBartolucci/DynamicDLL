using AssemblyLoadContextTestAPP_Complex;
using INT;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Input;

namespace AssemblyLoadContextTestAPP
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 1 && args[0] == "/d")
                {
                    Console.WriteLine("Waiting for any key...");
                    Console.ReadLine();
                }

                string[] pluginPaths = new string[]
                {
                    // Paths to plugins to load.
                    @"LIB1\bin\Debug\net8.0\LIB1.dll",
                    @"LIB2\bin\Debug\net8.0\LIB2.dll",
                    @"JsonPlugin\bin\Debug\net8.0\JsonPlugin.dll",
                };

                IEnumerable<IPlugin> plugins = pluginPaths.SelectMany(pluginPath =>
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    return CreatePlugins(pluginAssembly);
                }).ToList();

                if (args.Length == 0)
                {
                    Console.WriteLine("Plugins: ");
                    foreach (IPlugin plugin in plugins)
                    {
                        Console.WriteLine($"{plugin.Name}\t - {plugin.Description}");
                    }
                }
                else
                {
                    foreach (string pluginName in args)
                    {
                        Console.WriteLine($"-- {pluginName} --");

                        IPlugin plugin = plugins.FirstOrDefault(c => c.Name == pluginName);
                        if (plugin == null)
                        {
                            Console.WriteLine("No such command is known.");
                            return;
                        }

                        plugin.Execute();

                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        static IEnumerable<IPlugin> CreatePlugins(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    IPlugin result = Activator.CreateInstance(type) as IPlugin;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}