using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels.Models;
using DALI.DesignBrief.Repositories;

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class DesignBriefThemeRepository : Repository
    {
        public DesignBriefThemeRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {

        }

        public async Task<DesignBriefThemeModel[]> GetAll()
        {
            List<DesignBriefThemeModel> models = new List<DesignBriefThemeModel>();

            var entities = await _Context.DesignBriefThemes.AsNoTracking().ToArrayAsync();

            foreach (var entity in entities)
            {
                var model = new DesignBriefThemeModel();
                model.Id = entity.Id;
                model.Description = entity.Description;

                models.Add(model);
            }

            return models.ToArray();
        }

        public async Task<DesignBriefThemeReferenceModel[]> GetAll(int[] themeIds, int designBriefId)
        {
            List<DesignBriefThemeReferenceModel> models = new List<DesignBriefThemeReferenceModel>();

            var groupedEntities = await _Context.DesignBriefThemeReferences.AsNoTracking().Where(e => themeIds.Contains(e.ThemeId) && e.DesignBriefId == designBriefId).GroupBy(e => e.ThemeId).ToArrayAsync();

            foreach (var group in groupedEntities)
            {
                foreach (var theme in group)
                {
                    var model = models.SingleOrDefault(e => e.Theme.Id == theme.ThemeId);

                    if (model == null)
                    {
                        model = new DesignBriefThemeReferenceModel();

                        model.Theme.Id = theme.DesignBriefThemes.Id;
                        model.Theme.Description = theme.DesignBriefThemes.Description;

                        models.Add(model);
                    }

                    model.DesignBriefItemIds.Add(theme.DesignBriefItemId);
                }
            }

            return models.ToArray();
        }


        public async Task<DesignBriefThemeModel[]> GetByPolicyRequirement(int id, int designBriefId)
        {
            List<DesignBriefThemeModel> models = new List<DesignBriefThemeModel>();

            var entities = await _Context.DesignBriefThemeReferences.Where(e => e.DesignBriefItemId == id && e.DesignBriefId == designBriefId).ToArrayAsync();

            foreach (var theme in entities)
            {
                var model = new DesignBriefThemeModel();

                model.Id = theme.DesignBriefThemes.Id;
                model.Description = theme.DesignBriefThemes.Description;

                models.Add(model);
            }

            return models.ToArray();
        }
    }
}

