using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using Axerrio.DDD.Menu.Domain.Events;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.DomainEventHandlers.MenuCreated
{
    public class NotifyArtistsWhenMenuCreated : IDomainEventHandler<MenuCreatedDomainEvent>
    {
        private readonly IMenuRepository _menuRepository;
        public NotifyArtistsWhenMenuCreated(IMenuRepository MenuRepository)
        {
            _menuRepository = EnsureArg.IsNotNull(MenuRepository);
        }

        public Task Handle(MenuCreatedDomainEvent notification)
        {
            return Task.Run(() => {
                //Todo:
                //Task handle? doen ze shop wel await??
            });
        }
    }
}
