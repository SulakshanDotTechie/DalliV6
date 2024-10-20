using DALI.PolicyRequirement.BaseController;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace DALI.PolicyRequirement.LocalAuthoritiesController
{
    public class LiorLocalAuthoritiesController : LiorControllerBase
    {
        private readonly ILogger<LiorLocalAuthoritiesController> _logger;

        public LiorLocalAuthoritiesController(ILogger<LiorLocalAuthoritiesController> logger, GraphServiceClient graphServiceClient, [FromServices] PublicSpaceManagementServices services) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<PolicyRequirementLocalAuthorityModel>> Get()
        {
            var models = await Services.GetLocalAuthorities();

            return models;
        }

        [HttpGet("{id}")]
        public async Task<PolicyRequirementLocalAuthorityModel> Get(Guid id)
        {
            var model = await Services.GetLocalAuthority(id);

            return model;
        }
    }
}