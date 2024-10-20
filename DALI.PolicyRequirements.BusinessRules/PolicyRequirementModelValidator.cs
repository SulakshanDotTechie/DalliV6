using DALI.PolicyRequirements.DomainModels;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.BusinessRules.Validators
{
    public class PolicyRequirementModelValidator : AbstractValidator<PolicyRequirementModel>
    {
        public PolicyRequirementModelValidator()
        {
            RuleFor(m => m.Chapter.Id).GreaterThan(0);
            RuleFor(m => m.Level.Id).GreaterThan(0);
            RuleFor(m => m.Area.Id).GreaterThan(0);
            RuleFor(m => m.Subject.Id).GreaterThan(0);
            RuleFor(m => m.ChildSubject.Id).GreaterThan(0);
            RuleFor(m => m.Description).NotEmpty().NotNull();
        }

        public static async Task<Dictionary<string, string>> IsValidModel(PolicyRequirementModel model)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            await Task.Run(() =>
            {
                PolicyRequirementModelValidator validator = new PolicyRequirementModelValidator();
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
