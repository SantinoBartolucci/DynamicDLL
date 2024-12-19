using INT;

namespace LIB1
{
    public class PluginA : IPlugin
    {
        public string Name { get => "PluginA"; }
        public string Description { get => "Descripcion Plugin A"; }

        public void Execute()
        {
            Console.WriteLine("Hello Plugin A!!!");
        }
    }
}
