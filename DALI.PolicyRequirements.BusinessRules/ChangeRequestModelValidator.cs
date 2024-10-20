using DALI.PolicyRequirements.DomainModels;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.BusinessRules.Validators
{
    public class ChangeRequestModelValidator : AbstractValidator<PolicyRequirementChangeRequestModel>
    {
        public ChangeRequestModelValidator()
        {
            RuleFor(m => m.PolicyRequirementId).GreaterThan(0);
            RuleFor(m => m.VersionId).GreaterThan(0);
            RuleFor(m => m.ActionId).GreaterThan(0);
            RuleFor(m => m.StatusId).GreaterThan(0);
            RuleFor(m => m.Remark).NotEmpty().NotNull();
            RuleFor(m => m.UserName).NotEmpty().NotNull();
        }

        public static async Task<Dictionary<string, string>> IsValidModel(PolicyRequirementChangeRequestModel model)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            await Task.Run(() =>
            {
                ChangeRequestModelValidator validator = new ChangeRequestModelValidator();
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
