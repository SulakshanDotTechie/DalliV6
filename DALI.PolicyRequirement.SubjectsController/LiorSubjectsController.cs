using DALI.PolicyRequirement.BaseController;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Services;
using DALI.SearchEngine.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;


namespace DALI.PolicyRequirement.SubjectsController
{
    public class LiorSubjectsController : LiorControllerBase
    {
        private readonly ILogger<LiorSubjectsController> _logger;

        public LiorSubjectsController([FromServices] PublicSpaceManagementServices services, ILogger<LiorSubjectsController> logger, GraphServiceClient graphServiceClient) : base(services, graphServiceClient)
        {
            _logger = logger;
        }

        private async Task<List<PolicyRequirementSubjectModel>> GetAll()
        {
            if (MemoryCache.Default.Get("PolicyRequirementSubjectModels") == null)
            {
                var version = await GetCurrentVersion();

                var models = await Services.GetSubjects(version);

                var sorted = models.OrderBy(m => m.Description).ToList();

                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(3600))
                };

                MemoryCache.Default.Set("PolicyRequirementSubjectModels", sorted, cip);
            }

            return (List<PolicyRequirementSubjectModel>)MemoryCache.Default.Get("PolicyRequirementSubjectModels");
        }

        [HttpGet]
        public async Task<List<PolicyRequirementSubjectModel>> Get()
        {
            var models = await GetAll();

            return models;
        }

        [HttpGet("{id}")]
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

        [HttpGet("{id}")]
        public async Task<PolicyRequirementSubjectModel[]> GetByChapter(int id)
        {
            int[] ids = Services.GetSubjects(new int[] { id }, null, null, null, PolicyRequirements);

            var subjects = await GetAll();

            return subjects.Where(m => ids.Contains(m.Id)).OrderBy(m => m.Description).ToArray();
        }
    }
}
