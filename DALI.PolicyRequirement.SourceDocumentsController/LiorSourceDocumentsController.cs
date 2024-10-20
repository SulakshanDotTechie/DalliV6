using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DALI.PublicSpaceManagement.Services;
using Microsoft.AspNetCore.Mvc;
using DALI.PolicyRequirements.DomainModels;
using System.Runtime.Caching;
using Microsoft.Graph;
using DALI.PolicyRequirement.BaseController;
using Microsoft.Extensions.Logging;

namespace DALI.PolicyRequirement.SourceDocumentsController
{
    public class LiorSourceDocumentsController : LiorControllerBase
    {
        private readonly ILogger<LiorSourceDocumentsController> _logger;

        public LiorSourceDocumentsController([FromServices] PublicSpaceManagementServices services, ILogger<LiorSourceDocumentsController> logger, GraphServiceClient graphServiceClient) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        private async Task<List<PolicyRequirementSourceDocumentModel>> GetAll()
        {
            if (MemoryCache.Default.Get("PolicyRequirementSourceDocumentModels") == null)
            {
                var models = await Services.GetSourceDocuments();

                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(3600))
                };

                MemoryCache.Default.Set("PolicyRequirementSourceDocumentModels", models, cip);
            }

            return (List<PolicyRequirementSourceDocumentModel>)MemoryCache.Default.Get("PolicyRequirementSourceDocumentModels");
        }

        [HttpGet]
        public async Task<List<PolicyRequirementSourceDocumentModel>> Get()
        {
            var models = await GetAll();

            return models;
        }

        [HttpGet("{id}/{version}")]
        public async Task<List<PolicyRequirementSourceDocumentModel>> Get(int id, int version)
        {
            var models = await Services.GetSourceDocuments(id, version);

            return models;
        }
    }
}
