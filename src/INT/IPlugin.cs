namespace INT
{
    public interface IPlugin
    {
        string Name { get; }
        public string Description { get; }

        void Execute();
    }
}
