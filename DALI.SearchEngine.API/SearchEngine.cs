using DALI.SearchEngine.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.SearchEngine.API
{
    public class SearchEngine
    {
        static SearchEngine()
        {

        }

        public static List<T> Get<T>(LiorParametersModel paremeters, List<T> models) where T : class
        {
            if (!string.IsNullOrEmpty(paremeters.Keyword))
            {
                models = GetByKeyword(paremeters.Keyword, models);
            }
            else
            {
                models = GetByPath(paremeters, models);
            }

            return models;
        }

        public static List<T> GetByKeyword<T>(string keyWord, List<T> models) where T : class
        {
            string criteria = keyWord.ToLower();

            var resultsInDes = models.Where(m => ((ISearchModel)m).Description.ToLower().Contains(criteria)).ToList();
            var resultsInPath = models.Where(m => ((ISearchModel)m).Path.ToLower().Contains(criteria)).ToList();

            List<string> pbyDes = resultsInDes.Select(m => ((ISearchModel)m).Path.ToLower()).Distinct().ToList();
            List<string> pByPaths = resultsInPath.Select(m => ((ISearchModel)m).Path.ToLower()).Distinct().ToList();

            pByPaths.AddRange(pbyDes);
            pByPaths = pByPaths.Distinct().ToList();
            pByPaths.Sort();

            var results = models.Where(m => pByPaths.Contains(((ISearchModel)m).Path.ToLower())).ToList();

            return results;
        }

        /// <summary>
        /// T must me of interface type ISearchModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public static List<T> GetByPath<T>(LiorParametersModel parameters, List<T> models) where T : class
        {
            List<T> items = new List<T>();

            if ((!parameters.TownSpecificAreas &&
                parameters.Themes == null &&
                parameters.Chapters == null &&
                parameters.Levels == null &&
                parameters.Locations == null &&
                parameters.Areas == null &&
                parameters.Subjects == null) ||
                (!parameters.TownSpecificAreas &&
                (parameters.Themes != null && parameters.Themes.Length == 0) &&
                (parameters.Chapters != null && parameters.Chapters.Length == 0) &&
                (parameters.Levels != null && parameters.Levels.Length == 0) &&
                (parameters.Locations != null && parameters.Locations.Length == 0) &&
                (parameters.Areas != null && parameters.Areas.Length == 0) &&
                (parameters.Subjects != null && parameters.Subjects.Length == 0)))
            {
                return items;
            }


            items.AddRange(models);

            if (parameters.Chapters != null && parameters.Chapters.Length > 0)
            {
                var chapterModels = items.Where(c => parameters.Chapters.Contains(((ISearchModel)c).ChapterId));

                items = new List<T>(chapterModels);
            }

            if (parameters.Levels != null && parameters.Levels.Length > 0)
            {
                var levelModels = items.Where(l => parameters.Levels.Contains(((ISearchModel)l).LevelId));

                items = new List<T>(levelModels);
            }

            if (parameters.Locations != null && parameters.Locations.Length > 0)
            {
                var locationModels = items.Where(l => parameters.Locations.Contains(((ISearchModel)l).LocationId) || ((ISearchModel)l).LocationId == null || ((ISearchModel)l).LocationId == Guid.Empty);

                items = new List<T>(locationModels);
            }

            if ((parameters.Areas != null && parameters.Areas.Length > 0) || parameters.TownSpecificAreas)
            {
                var areaModels = items.Where(a => parameters.Areas.Contains(((ISearchModel)a).AreaId));

                items = new List<T>(areaModels);
            }

            if ((parameters.Subjects != null && parameters.Subjects.Length > 0))
            {
                var subjectModels = items.Where(a => parameters.Subjects.Contains(((ISearchModel)a).SubjectId));

                items = new List<T>(subjectModels);
            }

            return items;
        }
    }
}