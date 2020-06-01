namespace Canute
{
    public interface IPrototypeCopy : IPrototype, INameable, IUUIDLabeled
    {
        /// <summary>
        /// the common prototype of the item
        /// </summary>
        Prototype Proto { get; }
    }
    public interface IPrototypeCopy<T> : IPrototypeCopy, IPrototype, INameable, IUUIDLabeled
    {
        /// <summary>
        /// the prototype of the item
        /// </summary>
        T Prototype { get; }
    }
}
