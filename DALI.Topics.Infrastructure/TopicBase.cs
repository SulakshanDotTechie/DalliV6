using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.Infrastructure.Models
{
    public interface ITopicModel<TModel, TTopicModel>
    {
        string Type { get; }
        TModel Model { get; }
        string Name { get; set; }
        List<TTopicModel> Topics { get; }
        int TotalTopics { get; }
        string UniqueId { get; set; }
    }

    public abstract class TopicBase<TModel, TTopicModel> : ITopicModel<TModel, TTopicModel>
    {
        protected List<TTopicModel> _topics = new List<TTopicModel>();

        protected string _Type;

        public TopicBase(string type)
        {
            _Type = type;
        }

        public string Name { get; set; }

        public string Type => _Type;

        public TModel Model { get; set; }

        public List<TTopicModel> Topics => _topics;

        public int TotalTopics => _topics.Count;

        public string UniqueId { get; set; }
    }
}
