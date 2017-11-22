using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.DDD.BuildingBlocks
{
    public interface IUnitOfWork: IDisposable
    {
        Task<bool> DispatchDomainEventsAndSaveChangesAsync(bool saveChanges = true, CancellationToken cancellationToken = default(CancellationToken));
    }
}