using INT;

namespace LIB1
{
    public class PluginB : IPlugin
    {
        public string Name { get => "PluginB"; }
        public string Description { get => "Descripcion Plugin B"; }

        public void Execute()
        {
            Console.WriteLine("Hello Plugin B!!!");
        }
    }
}
