using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.Infrastructure.Models
{
    public class TopicResponseModel
    {
        private string _Title = string.Empty;

        public TopicResponseModel(string title)
        {
            _Title = title;
        }

        public List<ChapterTopic> Results { get; } = new List<ChapterTopic>();

        public int TotalResults { get; set; }

        public string Title
        {
            get
            {
                if (TotalResults > 0)
                {
                    if (string.IsNullOrEmpty(_Title))
                    {
                        _Title = InfrastructureResources.TopicsResources.Results;
                    }
                }
                else
                {
                    _Title = InfrastructureResources.TopicsResources.NoResults;
                }

                return string.Format("{0} ({1} {2})", _Title, TotalResults, InfrastructureResources.TopicsResources.Results.ToLower());
            }
        }
    }

    public class ThemeTopicResponseModel
    {
        private string _Title = string.Empty;

        public ThemeTopicResponseModel(string title)
        {
            _Title = title;
        }

        public List<ThemeTopic> Results { get; } = new List<ThemeTopic>();

        public int TotalResults { get; set; }

        public string Title
        {
            get
            {
                if (TotalResults > 0)
                {
                    if (string.IsNullOrEmpty(_Title))
                    {
                        _Title = InfrastructureResources.TopicsResources.Results;
                    }
                }
                else
                {
                    _Title = InfrastructureResources.TopicsResources.NoResults;
                }

                return string.Format("{0} ({1})", _Title, TotalResults);
            }
        }
    }
}
