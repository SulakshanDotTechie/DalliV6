using DALI.PolicyRequirement.BaseController;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

using System.Runtime.Caching;

namespace DALI.PolicyRequirement.LevelsController
{
    public class LiorLevelsController : LiorControllerBase
    {
        private readonly ILogger<LiorLevelsController> _logger;

        public LiorLevelsController(ILogger<LiorLevelsController> logger, GraphServiceClient graphServiceClient, [FromServices] PublicSpaceManagementServices services) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        private async Task<List<PolicyRequirementLevelModel>> GetAll()
        {
            if (MemoryCache.Default.Get("PolicyRequirementLevelModels") == null)
            {
                var version = await GetCurrentVersion();

                var models = await Services.GetLevels(version);

                var sorted = models.OrderBy(m => m.Position).ToList();

                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(3600))
                };

                MemoryCache.Default.Set("PolicyRequirementLevelModels", sorted, cip);
            }

            return (List<PolicyRequirementLevelModel>)MemoryCache.Default.Get("PolicyRequirementLevelModels");
        }

        [HttpGet]
        public async Task<List<PolicyRequirementLevelModel>> Get()
        {
            var models = await GetAll();

            return models;
        }

        [HttpGet("{version}")]
        public async Task<List<PolicyRequirementLevelModel>> Get(int version)
        {
            var models = await Services.GetLevels(version);

            var sorted = models.OrderBy(m => m.Position).ToList();

            return sorted;
        }

        [HttpGet("{id}/{version}")]
        public async Task<PolicyRequirementLevelModel> Get(int id, int version)
        {
            var model = await Services.GetLevel(id, version);

            return model;
        }
    }
}
