using DALI.PolicyRequirement.BaseController;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

using System.Runtime.Caching;

namespace DALI.PolicyRequirement.ChildSubjectsController
{
    public class LiorChildSubjectsController : LiorControllerBase
    {
        private readonly ILogger<LiorChildSubjectsController> _logger;

        public LiorChildSubjectsController(ILogger<LiorChildSubjectsController> logger, GraphServiceClient graphServiceClient, [FromServices] PublicSpaceManagementServices services) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        private async Task<List<PolicyRequirementSubjectModel>> GetAll()
        {
            if (MemoryCache.Default.Get("PolicyRequirementChildSubjectModels") == null)
            {
                var version = await GetCurrentVersion();

                var models = await Services.GetSubjects(version);

                var sorted = models.OrderBy(m => m.Description).ToList();

                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(3600))
                };

                MemoryCache.Default.Set("PolicyRequirementChildSubjectModels", sorted, cip);
            }

            return (List<PolicyRequirementSubjectModel>)MemoryCache.Default.Get("PolicyRequirementChildSubjectModels");
        }

        [HttpGet]
        public async Task<List<PolicyRequirementSubjectModel>> Get()
        {
            var models = await GetAll();

            return models;
        }

        [HttpGet("{version}")]
        public async Task<List<PolicyRequirementSubjectModel>> Get(int version)
        {
            var models = await Services.GetSubjects(version);

            var sorted = models.OrderBy(m => m.Description).ToList();

            return sorted;
        }

        [HttpGet("{id}/{version}")]
        public async Task<PolicyRequirementSubjectModel> Get(int id, int version)
        {
            var model = await Services.GetSubject(id, version);

            return model;
        }
    }
}
