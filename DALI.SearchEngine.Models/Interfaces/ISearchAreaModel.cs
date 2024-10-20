using System;
namespace DALI.SearchEngine.Models
{
    public interface ISearchAreaModel : ISearchVersionedModel<int>
    {
        bool IsTownSpecific { get; set; }
        bool FetchByDefault { get; set; }
    }
}
