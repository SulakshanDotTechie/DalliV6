using System;
namespace DALI.Topics.SharedInfrastructure
{
    public interface ILevelPropertyTopicModel
    {
        int Sequence { get; set; }
        int Id { get; set; }
        string Description { get; set; }
    }
}
