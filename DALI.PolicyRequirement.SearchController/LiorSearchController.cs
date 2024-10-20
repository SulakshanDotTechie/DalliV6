using DALI.PolicyRequirements.DomainModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DALI.SearchEngine.Models;
using Newtonsoft.Json;
using DALI.Topics.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using DALI.PublicSpaceManagement.Services;
using DALI.Topics.Infrastructure.Helpers;
using Microsoft.Graph;
using DALI.PolicyRequirement.BaseController;
using Microsoft.Extensions.Logging;

namespace DALI.PolicyRequirement.SearchController
{
    public class LiorSearchController : LiorControllerBase
    {
        private readonly ILogger<LiorSearchController> _logger;

        public LiorSearchController(ILogger<LiorSearchController> logger, GraphServiceClient graphServiceClient, [FromServices] PublicSpaceManagementServices services) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<PolicyRequirementModel>> Get(string model)
        {
            LiorParametersModel parameters = JsonConvert.DeserializeObject<LiorParametersModel>(model);

            if (parameters != null)
            {
                parameters.VersionId = await GetCurrentVersion();

                var models = await Services.Search(parameters, PolicyRequirements);

                return models;
            }

            return new List<PolicyRequirementModel>();
        }

        [HttpPost]
        public async Task<dynamic> Post([FromBody] LiorParametersModel model)
        {
            if (model != null)
            {
                model.VersionId = await GetCurrentVersion();

                var filteredModels = await Services.Search(model, PolicyRequirements);

                var response = new TopicTreeList(model.Keyword).Transform(filteredModels.ToList<ITopicDetailModel>());

                return response;
            }

            return null;
        }
    }
}