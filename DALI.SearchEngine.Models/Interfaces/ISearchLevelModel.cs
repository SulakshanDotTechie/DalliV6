using System;
namespace DALI.SearchEngine.Models
{
    public interface ISearchLevelModel : ISearchVersionedModel<int>
    {
        string Name { get; set; }
        string Position { get; set; }
    }
}
