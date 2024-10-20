using DALI.Topics.SharedInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.Infrastructure.Models
{
    public class LocationTopic : TopicBase<ILocationTopicModel, AreaTopic>
    {
        public LocationTopic() : base("location")
        {

        }
    }
}
