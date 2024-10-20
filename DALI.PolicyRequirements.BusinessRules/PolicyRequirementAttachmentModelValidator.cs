using DALI.PolicyRequirements.DomainModels;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.BusinessRules.Validators
{
    public class PolicyRequirementAttachmentModelValidator : AbstractValidator<PolicyRequirementAttachmentModel>
    {
        public PolicyRequirementAttachmentModelValidator()
        {
            RuleFor(m => m.Description).NotEmpty().NotNull();
        }

        public static async Task<Dictionary<string, string>> IsValidModel(PolicyRequirementAttachmentModel model)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            await Task.Run(() =>
            {
                PolicyRequirementAttachmentModelValidator validator = new PolicyRequirementAttachmentModelValidator();
                var result = validator.Validate(model);
                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        errors.Add(error.PropertyName, error.ErrorMessage);
                    }
                }
            });

            return errors;
        }
    }
}
