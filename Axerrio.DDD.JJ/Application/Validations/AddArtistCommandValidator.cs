using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Application.Commands;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.Validations
{
    public class AddArtistCommandValidator : AbstractValidator<AddArtistCommand>, IValidator<AddArtistCommand>
    {
        public AddArtistCommandValidator()
        {
            RuleFor(addArtist => addArtist.EmailAddress).EmailAddress().WithMessage("EmailAddress is not valid");
            RuleFor(addArtist => addArtist.LastName).NotEmpty();
            RuleFor(addArtist => addArtist.FirstName).NotEmpty();
        }
    }

}
