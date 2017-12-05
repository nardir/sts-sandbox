namespace Axerrio.BuildingBlocks
{
    public interface IRepository<T> where T : IAggregateRoot, IEntity
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
