using DALI.PolicyRequirement.BaseController;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

using System.Runtime.Caching;

namespace DALI.PolicyRequirement.AreasController
{
    public class LiorAreasController : LiorControllerBase
    {
        private readonly ILogger<LiorAreasController> _logger;
        public LiorAreasController(ILogger<LiorAreasController> logger, GraphServiceClient graphServiceClient, [FromServices] PublicSpaceManagementServices services) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        private async Task<List<PolicyRequirementAreaModel>> GetAll()
        {
            if (MemoryCache.Default.Get("PolicyRequirementAreaModels") == null)
            {
                var version = await GetCurrentVersion();

                var models = await Services.GetAreas(version);

                var sorted = models.OrderBy(m => m.Description).ToList();

                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(3600))
                };

                MemoryCache.Default.Set("PolicyRequirementAreaModels", sorted, cip);
            }

            return (List<PolicyRequirementAreaModel>)MemoryCache.Default.Get("PolicyRequirementAreaModels");
        }

        [HttpGet]
        public async Task<List<PolicyRequirementAreaModel>> Get()
        {
            var models = await GetAll();

            return models;
        }

        [HttpGet("{version}")]
        public async Task<List<PolicyRequirementAreaModel>> Get(int version)
        {
            var models = await Services.GetAreas(version);

            var sorted = models.OrderBy(m => m.Description).ToList();

            return sorted;
        }

        [HttpGet("{id}/{version}")]
        public async Task<PolicyRequirementAreaModel> Get(int id, int version)
        {
            var model = await Services.GetArea(id, version);

            return model;
        }
    }
}
