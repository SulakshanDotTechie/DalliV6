using DALI.PolicyRequirements.DataEntityModels;
using DALI.PublicSpaceManagement.DomainModels;

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PublicSpaceManagementThemeRepository : Repository
    {
        public PublicSpaceManagementThemeRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {

        }

        public async Task<PublicSpaceManagementThemeModel[]> GetAll()
        {
            List<PublicSpaceManagementThemeModel> models = new List<PublicSpaceManagementThemeModel>();

            var entities = await _Context.PublicSpaceManagementThemes.AsNoTracking().ToArrayAsync();

            foreach (var entity in entities)
            {
                var model = new PublicSpaceManagementThemeModel();
                model.Id = entity.Id;
                model.Description = entity.Description;
                model.Abbreviation = entity.Abbreviation;

                models.Add(model);
            }

            return models.ToArray();
        }

        public async Task<PublicSpaceManagementThemeReferenceModel[]> GetAll(int[] themeIds, int versionId)
        {
            List<PublicSpaceManagementThemeReferenceModel> models = new List<PublicSpaceManagementThemeReferenceModel>();

            var groupedEntities = await _Context.PublicSpaceManagementThemeReferences.AsNoTracking().Where(e => themeIds.Contains(e.ThemeId) && e.VersionId == versionId).GroupBy(e => e.ThemeId).ToArrayAsync();

            foreach (var group in groupedEntities)
            {
                foreach (var theme in group)
                {
                    var model = models.SingleOrDefault(e => e.Theme.Id == theme.ThemeId);

                    if (model == null)
                    {
                        model = new PublicSpaceManagementThemeReferenceModel();

                        model.Theme.Id = theme.PublicSpaceManagementThemes.Id;
                        model.Theme.Description = theme.PublicSpaceManagementThemes.Description;
                        model.Theme.Abbreviation = theme.PublicSpaceManagementThemes.Abbreviation;

                        models.Add(model);
                    }

                    model.PolicyRequirementIds.Add(theme.PolicyRequirementId);
                }
            }

            return models.ToArray();
        }


        public async Task<PublicSpaceManagementThemeModel[]> GetByPolicyRequirement(int id, int versionId)
        {
            List<PublicSpaceManagementThemeModel> models = new List<PublicSpaceManagementThemeModel>();

            var entities = await _Context.PublicSpaceManagementThemeReferences.AsNoTracking().Where(e => e.PolicyRequirementId == id && e.VersionId == versionId).ToArrayAsync();

            foreach (var theme in entities)
            {
                var model = new PublicSpaceManagementThemeModel();

                model.Id = theme.PublicSpaceManagementThemes.Id;
                model.Description = theme.PublicSpaceManagementThemes.Description;
                model.Abbreviation = theme.PublicSpaceManagementThemes.Abbreviation;

                models.Add(model);
            }

            return models.ToArray();
        }
    }
}