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
    //TODO: Template classfile for CommandHandler (Install with Nuget --> in package?)
    //Command/Handler toevoegen, meteen 2 files mogelijk?
    public class SubmitMenuCommandHandler : ICommandHandler<SubmitMenuCommand,bool>
    {
        private readonly IMenuRepository _menuRepository;

        public SubmitMenuCommandHandler(IMenuRepository menuRepository)
        {
            _menuRepository = EnsureArg.IsNotNull(menuRepository);
        }

        public async Task<bool> Handle(SubmitMenuCommand message)
        {           
            var menu = new MenuAggr.Menu(MenuStatus.Created, message.Description, new RequestInfo(message.RequesterName, DateTime.UtcNow));
            _menuRepository.Add(menu);

            if(message.Initiating)
                return await _menuRepository.UnitOfWork.DispatchDomainEventsAndSaveChangesAsync();

            return true;
        }        
    }

    //public class SubmitMenuIdentifiedCommandHandler: IdentifiedCommandHandler<SubmitMenuCommand, bool>
    //{
    //    public SubmitMenuIdentifiedCommandHandler(IMediator mediator, IClientRequestService clientRequestService) : base(mediator, clientRequestService)
    //    {
    //    }

    //    protected override bool CreateResultForDuplicateRequest()
    //    {
    //        return true;
    //    }
    //}
}
