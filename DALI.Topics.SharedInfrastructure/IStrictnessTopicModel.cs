using System;
namespace DALI.Topics.SharedInfrastructure
{
    public interface ISeverityTopicModel
    {
        string ShortName { get; set; }
        int Id { get; set; }
        string Description { get; set; }
    }
}
