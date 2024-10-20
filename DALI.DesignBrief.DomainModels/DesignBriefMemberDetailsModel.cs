namespace DALI.DesignBrief.DomainModels
{
    public class DesignBriefMemberDetailsModel
    {
        public DesignBriefMemberDetailsModel(string userName, DesignBriefMemberDetailModel[] designBriefInfoModels)
        {
            UserName = userName;
            Models = designBriefInfoModels;
        }

        public string UserName { get; }

        public DesignBriefMemberDetailModel[] Models { get; }
    }

    public class DesignBriefMemberDetailModel
    {
        public DesignBriefMemberDetailModel(int designBriefId, string code, string name, string description, string number, int? versionId, string projectManager)
        {
            DesignBriefId = designBriefId;
            Code = code;
            Name = name;
            Description = description;
            ProjectManager = projectManager;
            Number = number;
            VersionId = versionId;
        }

        public int DesignBriefId { get; }
        public int? VersionId { get; }
        public string Number { get; }
        public string Code { get; }
        public string Name { get; }
        public string Description { get; }
        public string ProjectManager { get; }
        public string Role { get; set; }
    }
}
