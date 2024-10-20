using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.SharedInfrastructure
{
    public interface IChildSubjectTopicModel : ISubjectTopicModel
    {
        string ChildSubject { get; }
    }
}
