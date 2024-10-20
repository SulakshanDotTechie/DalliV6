using System;
namespace DALI.SearchEngine.Models
{
    public interface ISearchLocationModel : ISearchVersionedModel<Guid>
    {
        bool FetchByDefault { get; set; }
        bool IsTownSpecific { get; set; }
    }
}
