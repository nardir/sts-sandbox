using System;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BuildingBlocks
{
    public interface IUnitOfWork: IDisposable
    {
        Task<bool> DispatchDomainEventsAndSaveChangesAsync(bool saveChanges = true, CancellationToken cancellationToken = default(CancellationToken));
    }
}