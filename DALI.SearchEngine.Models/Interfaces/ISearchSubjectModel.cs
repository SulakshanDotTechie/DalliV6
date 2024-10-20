using System;
namespace DALI.SearchEngine.Models
{
    public interface ISearchSubjectModel : ISearchVersionedModel<int>
    {
        bool FetchByDefault { get; set; }
        bool IsTownSpecific { get; set; }
    }
}
