using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.SharedInfrastructure
{
    public interface ILocationTopicModel
    {
        Guid Id { get; set; }
        string Description { get; set; }
        string Location { get; }
        int OrderIndex { get; set; }
    }
}
