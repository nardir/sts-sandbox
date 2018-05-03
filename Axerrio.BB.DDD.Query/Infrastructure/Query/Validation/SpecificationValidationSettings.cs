using FluentValidation;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.Validation
{

    public class SpecificationValidationSettings
    {
        #region ctor

        public SpecificationValidationSettings()
        {
        }

        #endregion

        private SpecificationOptions _allowedSpecificationOptions = SpecificationOptions.All;

        public SpecificationOptions AllowedSpecificationOptions
        {
            get => _allowedSpecificationOptions;

            set
            {
                _allowedSpecificationOptions = value;

                Validate();
            }
        }

        private SpecificationOptions _requiredSpecificationOptions = SpecificationOptions.None;

        public SpecificationOptions RequiredSpecificationOptions
        {
            get => _requiredSpecificationOptions;

            set
            {
                _requiredSpecificationOptions = value;

                Validate();
            }
        }

        private void Validate()
        {
            var validator = new SpecificationValidationSettingsValidator();

            //Refactor naar DomainValidationException, zoals bij validatie van een Command
            validator.ValidateAndThrow(this);
        }
    }
}