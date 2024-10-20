﻿using DALI.Topics.SharedInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.Infrastructure.Models
{
    public class ChapterTopic : TopicBase<IChapterTopicModel, LevelTopic>
    {
        public ChapterTopic() : base("chapter")
        {

        }
    }
}
