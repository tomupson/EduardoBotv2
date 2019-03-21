using System.Collections.Generic;

namespace EduardoBotv2.Core.Modules.News.Models
{
    public class NewsData
    {
        public int MaxHeadlines { get; set; }

        public List<string> NewsSources { get; set; }
    }
}