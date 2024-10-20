using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.SharedInfrastructure
{
    public interface IThemeTopicModel
    {
        int Id { get; }
        string Description { get; set; }
        string Abbreviation { get; set; }
    }
}
