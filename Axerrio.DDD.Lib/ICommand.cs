using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.Lib
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<TResponse>: ICommand, IRequest<TResponse>
    {
    }
}
