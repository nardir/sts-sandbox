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

            #region Allowed

            RuleFor(s => s).Must(s => !s.HasPredicate || _validationSettings.AllowedSpecificationOptions.HasFlag(SpecificationOptions.Filter))
                .WithMessage("Filtering is not allowed");

            RuleFor(s => s).Must(s => !s.HasOrdering || _validationSettings.AllowedSpecificationOptions.HasFlag(SpecificationOptions.Ordering))
                .WithMessage("Ordering is not allowed");

            RuleFor(s => s).Must(s => !s.HasPaging || _validationSettings.AllowedSpecificationOptions.HasFlag(SpecificationOptions.Paging))
                .WithMessage("Paging is not allowed");

            RuleFor(s => s).Must(s => !s.HasSelector || _validationSettings.AllowedSpecificationOptions.HasFlag(SpecificationOptions.Projection))
                .WithMessage("Projection is not allowed");

            #endregion

            #region Required

            RuleFor(s => s).Must(s => s.HasPredicate || !_validationSettings.RequiredSpecificationOptions.HasFlag(SpecificationOptions.Filter))
                .WithMessage("Filtering is required");

            RuleFor(s => s).Must(s => s.HasOrdering || !_validationSettings.RequiredSpecificationOptions.HasFlag(SpecificationOptions.Ordering))
                .WithMessage("Ordering is required");

            RuleFor(s => s).Must(s => s.HasPaging || !_validationSettings.RequiredSpecificationOptions.HasFlag(SpecificationOptions.Paging))
                .WithMessage("Paging is required");

            RuleFor(s => s).Must(s => s.HasSelector || !_validationSettings.RequiredSpecificationOptions.HasFlag(SpecificationOptions.Projection))
                .WithMessage("Projection is required");

            #endregion
        }
    }
}
