using DALI.PolicyRequirement.BaseController;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace DALI.PolicyRequirement.LocationsController
{
    public class LiorLocationsController : LiorControllerBase
    {
        private readonly ILogger<LiorLocationsController> _logger;

        public LiorLocationsController(ILogger<LiorLocationsController> logger, GraphServiceClient graphServiceClient, [FromServices] PublicSpaceManagementServices services) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<PolicyRequirementLocationModel>> Get()
        {
            var models = await Services.GetLocations();

            var sorted = models.OrderBy(m => m.Description).ToList();

            return sorted;
        }

        [HttpGet("{id}")]
        public async Task<PolicyRequirementLocationModel> Get(Guid id)
        {
            var model = await Services.GetLocation(id);

            return model;
        }
    }
}
