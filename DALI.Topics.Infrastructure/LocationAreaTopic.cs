using DALI.Topics.SharedInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.Infrastructure.Models
{
    public class LocationAreaTopic : TopicBase<IAreaTopicModel, SubjectSubTopic>
    {
        public LocationAreaTopic() : base("area")
        {

        }

        public ILocationTopicModel Location
        {
            get;
            set;
        }
    }
}
