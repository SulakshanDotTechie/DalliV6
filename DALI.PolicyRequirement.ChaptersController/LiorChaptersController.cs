using DALI.Administration.Services;
using DALI.PolicyRequirement.BaseController;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

using System.Runtime.Caching;

namespace DALI.PolicyRequirement.ChaptersController
{
    public class LiorChaptersController : LiorControllerBase
    {
        private readonly ILogger<LiorChaptersController> _logger;

        public LiorChaptersController(ILogger<LiorChaptersController> logger, GraphServiceClient graphServiceClient, [FromServices] PublicSpaceManagementServices services) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        private async Task<List<PolicyRequirementChapterModel>> GetAll()
        {
            if (MemoryCache.Default.Get("PolicyRequirementChapterModels") == null)
            {
                var version = await GetCurrentVersion();

                var models = await Services.GetChapters(version);

                var users = await new UserAccountAdministrationServices().GetAll();

                foreach (var model in models)
                {
                    var user = users.SingleOrDefault(e => string.Compare(e.UserName, model.Owner, true) == 0);
                    if (user != null)
                    {
                        model.OwnerEmailAddress = user.EmailAddress;
                    }
                }

                var sorted = models.OrderBy(m => m.ChapterNumber).ToList();

                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(3600))
                };

                MemoryCache.Default.Set("PolicyRequirementChapterModels", sorted, cip);
            }

            return (List<PolicyRequirementChapterModel>)MemoryCache.Default.Get("PolicyRequirementChapterModels");
        }


        [HttpGet]
        public async Task<List<PolicyRequirementChapterModel>> Get()
        {
            var models = await GetAll();

            return models;
        }

        [HttpGet("{version}")]
        public async Task<List<PolicyRequirementChapterModel>> Get(int version)
        {
            var models = await Services.GetChapters(version);

            var sorted = models.OrderBy(m => m.ChapterNumber).ToList();

            return sorted;
        }

        [HttpGet("{id}/{version}")]
        public async Task<PolicyRequirementChapterModel> Get(int id, int version)
        {
            var model = await Services.GetChapter(id, version);

            return model;
        }
    }
}
