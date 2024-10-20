using System;
using System.Net;
using System.Collections.Generic;

namespace DALI.SearchEngine.Models
{
    public interface ISearchModel : ISearchVersionedModel<int>
    {
        int? GroupIndex { get; set; }
        int? OrderIndex { get; set; }
        string Owner { get; set; }
        string DescriptionAsHtml { get; set; }
        string SemiPath { get; }
        string SemiPathKey { get; }
        string Path { get; }
        Guid? UniqueID { get; set; }
        Guid LocalAuthorityId { get; }
        int ChapterId { get; }
        int LevelId { get; }
        Guid LocationId { get; }
        int AreaId { get; }
        int SubjectId { get; }
        int ChildSubjectId { get; }
        int ThemeId { get; }
        bool TownSpecificAreas { get; }
    }
}
