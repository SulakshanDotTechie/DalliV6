using DALI.PublicSpaceManagement.Services;
using Microsoft.AspNetCore.Mvc;
using DALI.PolicyRequirements.DomainModels;
using Microsoft.Graph;
using DALI.PolicyRequirement.BaseController;
using Microsoft.Extensions.Logging;

namespace DALI.PolicyRequirement.PolicyRequirementController
{
    public class LiorPolicyRequirementsController : LiorControllerBase
    {
        private readonly ILogger<LiorPolicyRequirementsController> _logger;

        public LiorPolicyRequirementsController(ILogger<LiorPolicyRequirementsController> logger, GraphServiceClient graphServiceClient, [FromServices] PublicSpaceManagementServices services) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        [HttpGet("{id}/{version}")]
        public async Task<PolicyRequirementModel> Get(int id, int version)
        {
            var model = await Services.Get(id, version);

            return model;
        }

        [HttpGet("{id}/{version}")]
        public async Task<PolicyRequirementModel[]> GetByChapter(int id, int version)
        {
            var models = await Services.GetAll(id, version);

            return models;
        }

        [HttpGet("{chapterId}/{subjectId}")]
        public PolicyRequirementFullModel[] GetDetailsBy(int chapterId, int subjectId)
        {
            var models = PolicyRequirements.Where(m => m.Chapter.Id == chapterId && m.Subject.Id == subjectId).Select(m => new PolicyRequirementFullModel()
            {
                LocalAuthority = m.LocalAuthority,
                Chapter = m.Chapter,
                Level = m.Level,
                Location = m.Location,
                Area = m.Area,
                Subject = m.Subject,
                ChildSubject = m.ChildSubject,
                Strictnesses = m.Strictnesses,
                Attachments = m.Attachments,
                SourceDocuments = m.SourceDocuments,
                Id = m.Id,
                Description = m.Description,
                Active = m.Active,
                GroupIndex = m.GroupIndex,
                OrderIndex = m.OrderIndex,
                Owner = m.Owner,
                UniqueID = m.UniqueID,
                ModifiedBy = m.ModifiedBy,
                ModifiedDate = m.ModifiedDate,
                CreatedBy = m.CreatedBy,
                CreatedDate = m.CreatedDate,
                HasAttachments = m.HasAttachments,
                HasSourceReferences = m.HasSourceReferences,
                VersionId = m.VersionId
            });

            return models.ToArray();
        }
    }
}
