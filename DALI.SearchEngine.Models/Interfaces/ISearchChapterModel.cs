using System;
namespace DALI.SearchEngine.Models
{
    public interface ISearchChapterModel : ISearchVersionedModel<int>
    {
        string ChapterNumber { get; set; }
        string Owner { get; set; }
        string OwnerEmailAddress { get; set; }
        string FullChapterDescription { get; }
        bool FetchByDefault { get; set; }
        bool IsTownSpecific { get; set; }
    }
}
