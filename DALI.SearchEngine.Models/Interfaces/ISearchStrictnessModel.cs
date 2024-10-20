using System;
namespace DALI.SearchEngine.Models
{
    public interface ISearchStrictnessModel : ISearchVersionedModel<int>
    {
        string ShortName { get; set; }
        string Clarification { get; set; }
        bool IsAssigned { get; set; }
        int PolicyRequirementId { get; set; }
    }
}
