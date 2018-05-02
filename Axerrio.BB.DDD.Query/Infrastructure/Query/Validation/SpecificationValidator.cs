using EnsureThat;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.Validation
{
    //https://github.com/JeremySkinner/FluentValidation/wiki/b.-Creating-a-Validator
    public class SpecificationValidator<TEntity>: AbstractValidator<Specification<TEntity>>
    {
        private readonly SpecificationValidationSettings _validationSettings;

        public SpecificationValidator(SpecificationValidationSettings validationSettings)
        {
            _validationSettings = EnsureArg.IsNotNull(validationSettings, nameof(validationSettings));

            RuleFor(s => _validationSettings).SetValidator(new SpecificationValidationSettingsValidator());

            //RuleFor(s => s.HasPaging).Must(paging => paging == false || ((_validationSettings.AllowedSpecificationOptions & SpecificationOptions.Paging) == SpecificationOptions.Paging))
            //    .WithMessage("Paging is not allowed");

            RuleFor(s => s).Must(s => !s.HasPaging || _validationSettings.AllowedSpecificationOptions.HasFlag(SpecificationOptions.Paging))
                .WithMessage("Paging is not allowed");

            RuleFor(s => s).Must(s => s.HasPaging || !_validationSettings.RequiredSpecificationOptions.HasFlag(SpecificationOptions.Paging))
                .WithMessage("Paging is required");
        }
    }

    public class SpecificationValidationSettingsValidator: AbstractValidator<SpecificationValidationSettings>
    {
        public SpecificationValidationSettingsValidator()
        {

        }
    }
}
