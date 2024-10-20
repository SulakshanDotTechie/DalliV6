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
    public class CommentModelValidator : AbstractValidator<PolicyRequirementAdditionalInfoModel>
    {
        public CommentModelValidator()
        {
            RuleFor(m => m.Description).NotEmpty().NotNull();
        }

        public static async Task<Dictionary<string, string>> IsValidModel(PolicyRequirementAdditionalInfoModel model)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            await Task.Run(() =>
            {
                CommentModelValidator validator = new CommentModelValidator();
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
