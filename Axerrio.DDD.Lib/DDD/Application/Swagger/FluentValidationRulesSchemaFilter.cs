using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BuildingBlocks
{
    public class AddFluentValidationRules : ISchemaFilter
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        ///     Default constructor with DI
        /// </summary>
        /// <param name="provider"></param>
        public AddFluentValidationRules(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        public void Apply(Schema model, SchemaFilterContext context)
        {
            // use IoC or FluentValidatorFactory to get AbstractValidator<T> instance

            var validatorType = typeof(IValidator<>).MakeGenericType(context.SystemType);

            IValidator validator = null;
            try
            {
                validator = _provider.GetRequiredService(validatorType) as IValidator;
            }
            catch (InvalidOperationException)
            {
                return; //Intended. No Validator for type
            }

            if (validator == null) return;

            //https://stackoverflow.com/questions/44638195/fluent-validation-with-swagger-in-asp-net-core
            if (model.Required == null)
                model.Required = new List<string>();

            var validatorDescriptor = validator.CreateDescriptor();
            foreach (var key in model.Properties.Keys)
            {
                foreach (var propertyValidator in validatorDescriptor
                    .GetValidatorsForMember(ToPascalCase(key)))
                {
                    if (propertyValidator is NotNullValidator
                      || propertyValidator is NotEmptyValidator)
                        model.Required.Add(key);

                    if (propertyValidator is LengthValidator lengthValidator)
                    {
                        if (lengthValidator.Max > 0)
                            model.Properties[key].MaxLength = lengthValidator.Max;

                        model.Properties[key].MinLength = lengthValidator.Min;
                    }

                    if (propertyValidator is RegularExpressionValidator expressionValidator)
                        model.Properties[key].Pattern = expressionValidator.Expression;

                    if (propertyValidator is EmailValidator emailValidator)
                        model.Properties[key].Pattern = emailValidator.Expression;
                    // Add more validation properties here;
                }
            }
        }

        /// <summary>
        ///     To convert case as swagger may be using lower camel case
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private static string ToPascalCase(string inputString)
        {
            // If there are 0 or 1 characters, just return the string.
            if (inputString == null) return null;
            if (inputString.Length < 2) return inputString.ToUpper();
            return inputString.Substring(0, 1).ToUpper() + inputString.Substring(1);
        }
    }
}
