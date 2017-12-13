using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MenuAggr = Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using MediatR;
using System.Threading;

namespace Axerrio.DDD.Menu.Application.Commands
{   
    //TODO: Template classfile for CommandHandler (Install with Nuget --> in package?)
    //Command/Handler toevoegen, meteen 2 files mogelijk?
    public class SubmitMenuCommandHandler : CommandHandler<SubmitMenuCommand,bool>
    {
        private readonly IMenuRepository _menuRepository;

        public SubmitMenuCommandHandler(IMenuRepository menuRepository)
        {
            _menuRepository = EnsureArg.IsNotNull(menuRepository);
        }

        public override async Task<bool> Handle(SubmitMenuCommand message, CancellationToken cancellationToken = default(CancellationToken))
        {         
            var menu = new MenuAggr.Menu(MenuStatus.Created, message.Description, new RequestInfo(message.RequesterName, DateTime.UtcNow));
            _menuRepository.Add(menu); // async on repo!?
            
            return await _menuRepository.UnitOfWork.DispatchDomainEventsAndSaveChangesAsync(message.Initiating, cancellationToken);            
        }        
    }

    public class SubmitMenuIdentifiedCommandHandler : IdentifiedCommandHandler<SubmitMenuCommand, bool>
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
