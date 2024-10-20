using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.SharedInfrastructure
{
    public interface ILevelTopicModel
    {
        int Id { get; set; }
        string Position { get; set; }
        string Description { get; set; }
        string Name { get; set; }
    }
}
