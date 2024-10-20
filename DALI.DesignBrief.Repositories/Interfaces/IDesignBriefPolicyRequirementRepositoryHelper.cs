using DALI.DesignBrief.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.Interfaces
{
    public interface IDesignBriefPolicyRequirementRepositoryHelper
    {
        Task<List<DesignBriefChapterModel>> GetChapters();
        Task<List<DesignBriefLocationModel>> GetLocations();
        Task<DesignBriefLocationModel> GetLocation(Guid id);
        Task<List<DesignBriefAreaModel>> GetAreas();
        Task<List<DesignBriefSubjectModel>> GetSubjects();
        Task<List<DesignBriefChildSubjectModel>> GetChildSubjects();
        Task<DesignBriefChildSubjectModel> GetChildSubject(int id);
        Task<List<DesignBriefSeverityModel>> GetStrictnessess();
        Task<DesignBriefChapterModel> GetChapter(int id);
        Task<DesignBriefAreaModel> GetArea(int id);
        Task<DesignBriefSubjectModel> GetSubject(int id);
    }
}
