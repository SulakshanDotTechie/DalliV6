using DALI.PublicSpaceManagement.Services;
using Microsoft.AspNetCore.Mvc;
using DALI.PolicyRequirements.DomainModels;
using System.Runtime.Caching;
using Microsoft.Graph;
using DALI.PolicyRequirement.BaseController;
using Microsoft.Extensions.Logging;
namespace DALI.PolicyRequirement.AttachmentsController
{
    public class LiorAttachmentsController : LiorControllerBase
    {
        private readonly ILogger<LiorAttachmentsController> _logger;

        public LiorAttachmentsController(ILogger<LiorAttachmentsController> logger, GraphServiceClient graphServiceClient, [FromServices] PublicSpaceManagementServices services) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        private async Task<List<PolicyRequirementAttachmentModel>> GetAll()
        {
            if (MemoryCache.Default.Get("PolicyRequirementAttachmentModels") == null)
            {
                var models = await Services.GetAttachments();

                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(3600))
                };

                MemoryCache.Default.Set("PolicyRequirementAttachmentModels", models, cip);
            }

            return (List<PolicyRequirementAttachmentModel>)MemoryCache.Default.Get("PolicyRequirementAttachmentModels");
        }

        [HttpGet]
        public async Task<List<PolicyRequirementAttachmentModel>> Get()
        {
            var models = await GetAll();

            return models;
        }

        [HttpGet("{id}/{version}")]
        public async Task<List<PolicyRequirementAttachmentModel>> Get(int id, int version)
        {
            var models = await Services.GetAttachments(id, version);

            return models;
        }
    }
}
