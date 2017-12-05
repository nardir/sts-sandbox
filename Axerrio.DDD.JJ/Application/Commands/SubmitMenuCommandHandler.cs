using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MenuAggr = Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using MediatR;

namespace Axerrio.DDD.Menu.Application.Commands
{   

    public class SubmitMenuCommandHandler : ICommandHandler<SubmitMenuCommand>
    {
        private readonly IMenuRepository _menuRepository;

        public SubmitMenuCommandHandler(IMenuRepository menuRepository)
        {
            _menuRepository = EnsureArg.IsNotNull(menuRepository);
        }

        public async Task Handle(SubmitMenuCommand message)
        {
            //todo: menu props erbij!
            var menu = new MenuAggr.Menu(MenuStatus.Created, message.Description);
            _menuRepository.Add(menu);

            if(message.Initiating)
                await _menuRepository.UnitOfWork.DispatchDomainEventsAndSaveChangesAsync();       
        }
        
    }

    public class SubmitMenuIdentifiedCommandHandler: IdentifiedCommandHandler<SubmitMenuCommand, bool>
    {
        public SubmitMenuIdentifiedCommandHandler(IMediator mediator, IClientRequestService clientRequestService) : base(mediator, clientRequestService)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;
        }
    }
}
