using FluentValidation;

namespace Axerrio.BB.DDD.Infrastructure.Query.Validation
{
    public class SpecificationValidationSettingsValidator: AbstractValidator<SpecificationValidationSettings>
    {
        public SpecificationValidationSettingsValidator()
        {
            RuleFor(s => s).Must(s => (s.AllowedSpecificationOptions.HasFlag(SpecificationOptions.Filtering) || !s.RequiredSpecificationOptions.HasFlag(SpecificationOptions.Filtering))).WithMessage("Filtering cannot be both required and not allowed");
            RuleFor(s => s).Must(s => (s.AllowedSpecificationOptions.HasFlag(SpecificationOptions.Ordering) || !s.RequiredSpecificationOptions.HasFlag(SpecificationOptions.Ordering))).WithMessage("Ordering cannot be both required and not allowed");
            RuleFor(s => s).Must(s => (s.AllowedSpecificationOptions.HasFlag(SpecificationOptions.Paging) || !s.RequiredSpecificationOptions.HasFlag(SpecificationOptions.Paging))).WithMessage("Paging cannot be both required and not allowed");
            RuleFor(s => s).Must(s => (s.AllowedSpecificationOptions.HasFlag(SpecificationOptions.Projection) || !s.RequiredSpecificationOptions.HasFlag(SpecificationOptions.Projection))).WithMessage("Paging cannot be both required and not allowed");
        }
    }
}
