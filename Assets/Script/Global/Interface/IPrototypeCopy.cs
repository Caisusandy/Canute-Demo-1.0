namespace Canute
{
    public interface IPrototypeCopy : IPrototype, INameable, IUUIDLabeled
    {
        Prototype Proto { get; }
    }
    public interface IPrototypeCopy<T> : IPrototypeCopy, IPrototype, INameable, IUUIDLabeled
    {
        T Prototype { get; }
    }
}
