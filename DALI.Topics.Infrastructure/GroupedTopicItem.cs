using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.Infrastructure.Models
{
    public class GroupedTopicItem<TKey, TModel>
    {
        public TKey Key { get; set; }
        public IEnumerable<TModel> Details { get; set; }
        public int Count { get; set; }
    }
}
