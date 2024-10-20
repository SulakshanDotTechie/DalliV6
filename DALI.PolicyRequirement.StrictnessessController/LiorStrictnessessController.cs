using DALI.PolicyRequirement.BaseController;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirement.StrictnessessController
{
    public class LiorStrictnessessController : LiorControllerBase
    {
        private readonly ILogger<LiorStrictnessessController> _logger;

        public LiorStrictnessessController([FromServices] PublicSpaceManagementServices services, ILogger<LiorStrictnessessController> logger, GraphServiceClient graphServiceClient) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<PolicyRequirementSeverityModel>> Get()
        {
            var models = await Services.GetSeverities();

            return models;
        }

        [HttpGet("{id}/{version}")]
        public async Task<List<PolicyRequirementSeverityModel>> Get(int id, int version)
        {
            var models = await Services.GetSeverities(id, version);

            return models;
        }
    }
}
