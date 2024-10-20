using DALI.Models;
using DALI.PolicyRequirements.BusinessRules.Validators;
using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using DALI.PolicyRequirements.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PublicSpaceManagement.Services
{
    public partial class PublicSpaceManagementServices : IDisposable
    {
        #region private members
        private IPolicyRequirementDataEntityModelContext _Context;
        private PolicyRequirementRepository _PolReqsRepos;
        private PolicyRequirementChapterRepository _ChapterRepos;
        private PolicyRequirementLevelRepository _LevelRepos;
        private PolicyRequirementAreaRepository _AreaRepos;
        private PolicyRequirementSubjectRepository _SubjectRepos;
        private PolicyRequirementModificationQueueRepository _ModificationQueueRepos;
        private PolicyRequirementAttachmentRepository _AttachmentRepos;
        private PolicyRequirementSourceDocumentRepository _SourceReferenceRepos;
        private PolicyRequirementSeverityRepository _SeverityRepos;
        private PolicyRequirementChangeRequestRepository _ChangeRequestRepos;
        private PolicyRequirementPublicationRepository _PublicationRepos;
        private PolicyRequirementPublicationQueueRepository _PublicationQueueRepos;
        private PolicyRequirementCommentsRepository _CommentRepos;
        private PolicyRequirementKeywordRepository _KeywordRepos;
        private PolicyRequirementChildSubjectRepository _ChildSubjectRepos;
        private PolicyRequirementLocationRepository _LocationRepos;
        private PolicyRequirementLocalAuthorityRepository _LocalAuthorityRepos;
        private PublicSpaceManagementThemeRepository _PublicSpaceManagementThemeRepos;

        private CommentModelValidator _CommentValidator;
        #endregion

        public PublicSpaceManagementServices(string connectionString)
        {
            _Context = string.IsNullOrEmpty(connectionString) ? new PolicyRequirementDataEntityModels() : new PolicyRequirementDataEntityModels(connectionString);

            _LocalAuthorityRepos = new PolicyRequirementLocalAuthorityRepository(_Context);
            _PolReqsRepos = new PolicyRequirementRepository(_Context);
            _ChapterRepos = new PolicyRequirementChapterRepository(_Context);
            _LevelRepos = new PolicyRequirementLevelRepository(_Context);
            _AreaRepos = new PolicyRequirementAreaRepository(_Context);
            _SubjectRepos = new PolicyRequirementSubjectRepository(_Context);
            _ModificationQueueRepos = new PolicyRequirementModificationQueueRepository(_Context);
            _AttachmentRepos = new PolicyRequirementAttachmentRepository(_Context);
            _SourceReferenceRepos = new PolicyRequirementSourceDocumentRepository(_Context);
            _SeverityRepos = new PolicyRequirementSeverityRepository(_Context);
            _ChangeRequestRepos = new PolicyRequirementChangeRequestRepository(_Context);
            _PublicationRepos = new PolicyRequirementPublicationRepository(_Context);
            _PublicationQueueRepos = new PolicyRequirementPublicationQueueRepository(_Context);
            _CommentRepos = new PolicyRequirementCommentsRepository(_Context);
            _KeywordRepos = new PolicyRequirementKeywordRepository(_Context);
            _ChildSubjectRepos = new PolicyRequirementChildSubjectRepository(_Context);
            _LocationRepos = new PolicyRequirementLocationRepository(_Context);
            _PublicSpaceManagementThemeRepos = new PublicSpaceManagementThemeRepository(_Context);
            _CommentValidator = new CommentModelValidator();
        }

        public PublicSpaceManagementServices() : this(string.Empty)
        {

        }

        #region MainData
        public async Task<int> GetCurrentVersion()
        {
            int result = await _PublicationRepos.GetCurrentVersion();

            return result;
        }

        #region chapters
        public async Task<List<PolicyRequirementModel>> GetAll(int version)
        {
            List<PolicyRequirementModel> result = await _PolReqsRepos.GetAll(version);

            return result;
        }


        public async Task<PolicyRequirementModel[]> GetAll(int chapterId, int version)
        {
            List<PolicyRequirementModel> result = await _PolReqsRepos.GetPolicyRequirements(chapterId, version);

            return result.Where(m => m.ChapterId == chapterId).ToArray();
        }

        public async Task<PolicyRequirementModel> Get(int id, int version)
        {
            PolicyRequirementModel result = await _PolReqsRepos.GetOneModelAsync(id, version);

            return result;
        }

        public async Task<List<PolicyRequirementChapterModel>> GetChapters(int version)
        {
            List<PolicyRequirementChapterModel> result = await _ChapterRepos.GetAll(version);

            return result;
        }

        public async Task<PolicyRequirementChapterModel> GetChapter(int id, int version)
        {
            PolicyRequirementChapterModel result = await _ChapterRepos.GetOneModelAsync(id, version);

            return result;
        }

        public async Task<List<PolicyRequirementChapterModel>> GetChapters(string owner, int version)
        {
            List<PolicyRequirementChapterModel> result = await _ChapterRepos.GetAll(owner, version);

            return result;
        }

        public async Task<List<PolicyRequirementChapterModel>> GetChaptersForEditors(int version)
        {
            List<PolicyRequirementChapterModel> result = await _ChapterRepos.GetForEditors(version);

            return result;
        }

        public async Task<PolicyRequirementChapterOwner[]> GetChapterOwners(int version)
        {
            List<PolicyRequirementChapterModel> chapterModels = await _ChapterRepos.GetAll(version);

            List<PolicyRequirementChapterOwner> ownerModels = new List<PolicyRequirementChapterOwner>();

            foreach (var chapter in chapterModels)
            {
                var ownerModel = new PolicyRequirementChapterOwner();

                ownerModel.ChapterId = chapter.Id;
                ownerModel.FullChapterDescription = chapter.FullChapterDescription;
                ownerModel.Owner = chapter.Owner;

                ownerModels.Add(ownerModel);
            }


            return ownerModels.ToArray();
        }

        #endregion

        #region areas
        public async Task<List<PolicyRequirementAreaModel>> GetAreasForEditors(int version)
        {
            List<PolicyRequirementAreaModel> result = await _AreaRepos.GetForEditors(version);

            return result;
        }

        public async Task<List<PolicyRequirementAreaModel>> GetAreas(int version)
        {
            List<PolicyRequirementAreaModel> models = await _AreaRepos.GetAll(version);

            return models;
        }

        public async Task<PolicyRequirementAreaModel> GetArea(int id, int version)
        {
            PolicyRequirementAreaModel result = await _AreaRepos.GetOneModelAsync(id, version);

            return result;
        }

        public int[] GetAreas(int[] chapters, int[] levels, int[] subjects, List<PolicyRequirementModel> models)
        {
            List<PolicyRequirementModel> items = new List<PolicyRequirementModel>();

            if (chapters != null && chapters.Length > 0)
            {
                IEnumerable<PolicyRequirementModel> chapterItems = models.Where(m => chapters.Contains(m.Chapter.Id));
                items.AddRange(chapterItems);
            }
            else
            {
                items.AddRange(models);
            }

            if (levels != null && levels.Length > 0)
            {
                IEnumerable<PolicyRequirementModel> levelItems = items.Where(m => levels.Contains(m.Level.Id));
                items = new List<PolicyRequirementModel>(levelItems);
            }

            if (subjects != null && subjects.Length > 0)
            {
                IEnumerable<PolicyRequirementModel> subjectModels = items.Where(a => subjects.Contains(a.Subject.Id));
                items = new List<PolicyRequirementModel>(subjectModels);
            }

            if (items.Count > 0)
            {
                int[] ids = items.Select(m => m.Area.Id).Distinct().ToArray();

                return ids;
            }

            return new int[1];
        }
        #endregion

        #region subjects
        public async Task<List<PolicyRequirementSubjectModel>> GetSubjectsForEditors(int version)
        {
            List<PolicyRequirementSubjectModel> result = await _SubjectRepos.GetForEditors(version);

            return result;
        }

        public async Task<List<PolicyRequirementSubjectModel>> GetSubjects(int version)
        {
            List<PolicyRequirementSubjectModel> result = await _SubjectRepos.GetAll(version);

            return result;
        }

        public async Task<PolicyRequirementSubjectModel> GetSubject(int id, int version)
        {
            PolicyRequirementSubjectModel result = await _SubjectRepos.GetOneModelAsync(id, version);

            return result;
        }

        public int[] GetSubjects(int[] chapters, int[] levels, Guid[] locations, int[] areas, List<PolicyRequirementModel> models)
        {
            List<PolicyRequirementModel> items = new List<PolicyRequirementModel>();

            if (chapters != null && chapters.Length > 0)
            {
                IEnumerable<PolicyRequirementModel> chapterItems = models.Where(m => chapters.Contains(m.Chapter.Id));
                items.AddRange(chapterItems);
            }
            else
            {
                items.AddRange(models);
            }

            if (levels != null && levels.Length > 0)
            {
                IEnumerable<PolicyRequirementModel> levelItems = items.Where(m => levels.Contains(m.Level.Id));
                items = new List<PolicyRequirementModel>(levelItems);
            }

            if (locations != null && locations.Length > 0)
            {
                IEnumerable<PolicyRequirementModel> locationModels = items.Where(m => locations.Contains(m.Location.Id));
                items = new List<PolicyRequirementModel>(locationModels);
            }

            if (areas != null && areas.Length > 0)
            {
                IEnumerable<PolicyRequirementModel> areaModels = items.Where(m => areas.Contains(m.Area.Id));
                items = new List<PolicyRequirementModel>(areaModels);
            }

            if (items.Count > 0)
            {
                int[] ids = items.Select(m => m.Subject.Id).Distinct().ToArray();

                return ids;
            }

            return new int[1];
        }

        #endregion

        public async Task<List<PolicyRequirementLevelModel>> GetLevels(int version)
        {
            var models = await _LevelRepos.GetAll();

            return models;
        }

        public async Task<PolicyRequirementLevelModel> GetLevel(int id, int version)
        {
            PolicyRequirementLevelModel model = await _LevelRepos.GetOneModelAsync(id);

            return model;
        }

        public async Task<PolicyRequirementPublicationModel> GetPublicationInfo()
        {
            PolicyRequirementPublicationModel model = await _PublicationRepos.GetLatest();

            return model;
        }

        public async Task<List<PolicyRequirementLevelPropertyModel>> GetLevelProperties(int levelId)
        {
            List<PolicyRequirementLevelPropertyModel> models = await _LevelRepos.GetProperties(levelId);

            return models;
        }

        public async Task<List<string>> GetKeywords(string searchText, int version)
        {
            List<PolicyRequirementKeywordModel> models = await _KeywordRepos.GetAll(version);

            List<string> keywords = models.Select(m => m.Description).ToList();

            if (!string.IsNullOrEmpty(searchText))
            {
                keywords = keywords.Where(p => p.StartsWith(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return keywords;
        }

        public async Task<bool> ActiveOwner(string userName)
        {
            int version = await GetCurrentVersion();

            List<PolicyRequirementChapterModel> chapters = await GetChapters(userName, version);

            if (chapters.Count > 0)
                return true;

            List<PolicyRequirementModel> models = await GetAll(version);

            if (models.Where(m => m.Owner == userName).Count() > 0)
                return true;

            return false;
        }
        #endregion

        #region MainData CRUD

        #region add
        public async Task<ResponseModel> AddChapter(PolicyRequirementChapterModel model, int version)
        {
            var response = new ResponseModel();

            try
            {
                var result = await _ChapterRepos.Add(model, version);

                if (result == 1)
                {
                    response.Status = 1;
                    response.Id = model.Id;
                    response.Errors = null;
                }
                else
                    throw new InvalidOperationException();
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", DALI.Common.Helpers.ExceptionHelper.ErrorMessageHandler(error));
            }

            return response;
        }

        public async Task<ResponseModel> AddArea(PolicyRequirementAreaModel model, int version)
        {
            var response = new ResponseModel();

            try
            {
                var result = await _AreaRepos.Add(model, version);

                if (result == 1)
                {
                    response.Status = 1;
                    response.Id = model.Id;
                    response.Errors = null;
                }
                else
                    throw new InvalidOperationException();
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", DALI.Common.Helpers.ExceptionHelper.ErrorMessageHandler(error));
            }

            return response;
        }

        public async Task<ResponseModel> AddSubject(PolicyRequirementSubjectModel model, int version)
        {
            var response = new ResponseModel();

            try
            {
                var result = await _SubjectRepos.Add(model, version);

                if (result == 1)
                {
                    response.Status = 1;
                    response.Id = model.Id;
                    response.Errors = null;
                }
                else
                    throw new InvalidOperationException();
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", DALI.Common.Helpers.ExceptionHelper.ErrorMessageHandler(error));
            }

            return response;
        }

        public async Task<ResponseModel> AddKeyword(PolicyRequirementKeywordModel model)
        {
            var response = new ResponseModel();

            try
            {
                model.Active = true;

                var result = await _KeywordRepos.Add(model);

                if (result == 1)
                {
                    response.Status = 1;
                    response.Id = model.Id;
                    response.Errors = null;
                }
                else
                    throw new InvalidOperationException();
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", DALI.Common.Helpers.ExceptionHelper.ErrorMessageHandler(error));
            }

            return response;
        }
        #endregion

        #region update
        public async Task<ResponseModel> UpdateChapter(PolicyRequirementChapterModel model, int version)
        {
            var response = new ResponseModel();

            try
            {
                var result = await _ChapterRepos.Update(model, version);

                if (result == 1)
                {
                    response.Status = 1;
                    response.Id = model.Id;
                    response.Errors = null;
                }
                else
                    throw new InvalidOperationException();
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", Common.Helpers.ExceptionHelper.ErrorMessageHandler(error));
            }

            return response;
        }

        public async Task<ResponseModel> UpdateOwner(PolicyRequirementChapterModel model, int version)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var result = await _ChapterRepos.UpdateOwner(model);

                if (result == 1)
                {
                    result = await _ChapterRepos.Save();
                }

                response.Status = 1;
                response.Id = model.Id;
                response.Errors = null;
            }
            catch (Exception error)
            {
                await _ChapterRepos.Refresh(model.Id, model.VersionId);
                response.Status = 0;
                response.Errors.Add("ErrorMessage", DALI.Common.Helpers.ExceptionHelper.ErrorMessageHandler(error));
            }

            return response;
        }

        public async Task<ResponseModel> UpdateArea(PolicyRequirementAreaModel model, int version)
        {
            var response = new ResponseModel();

            try
            {
                var result = await _AreaRepos.Update(model, version);

                if (result == 1)
                {
                    response.Status = 1;
                    response.Id = model.Id;
                    response.Errors = null;
                }
                else
                    throw new InvalidOperationException();
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", Common.Helpers.ExceptionHelper.ErrorMessageHandler(error));
            }

            return response;
        }

        public async Task<ResponseModel> UpdateSubject(PolicyRequirementSubjectModel model, int version)
        {
            var response = new ResponseModel();

            try
            {
                var result = await _SubjectRepos.Update(model, version);

                if (result == 1)
                {
                    response.Status = 1;
                    response.Id = model.Id;
                    response.Errors = null;
                }
                else
                    throw new InvalidOperationException();
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", Common.Helpers.ExceptionHelper.ErrorMessageHandler(error));
            }

            return response;
        }

        public async Task<ResponseModel> UpdateKeyword(PolicyRequirementKeywordModel model)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var result = await _KeywordRepos.Update(model);

                if (result == 1)
                {
                    result = await _KeywordRepos.Save();
                }

                response.Status = 1;
                response.Id = model.Id;
                response.Errors = null;
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", DALI.Common.Helpers.ExceptionHelper.ErrorMessageHandler(error));
            }

            return response;
        }

        public async Task<ResponseModel> AssignNewOwner(int chapterId, string oldOwner, string newOwner, int version)
        {
            ResponseModel response = new ResponseModel();

            System.Data.Common.DbConnection connection = _PolReqsRepos.GetConnection();

            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // assign new owner on policyRequirements
                    var pReqIds = await _PolReqsRepos.AssignNewOwner(chapterId, oldOwner, newOwner, version);

                    if (pReqIds.Count > 0)
                    {
                        // change owner modifications queue
                        int[] prmQIds = await _ModificationQueueRepos.AssignNewOwner(chapterId, oldOwner, newOwner, version);

                        await _PolReqsRepos.Save();

                        if (prmQIds.Length > 0)
                        {
                            // change owner on change requests
                            int resCrq = await _ChangeRequestRepos.AssignNewOwner(prmQIds, oldOwner, newOwner, version);

                            await _ModificationQueueRepos.Save();

                            if (resCrq == 1)
                            {
                                await _ChangeRequestRepos.Save();
                            }
                        }
                    }

                    transaction.Commit();

                    response.Status = 1;
                    response.Errors = null;
                }
                catch (Exception error)
                {
                    transaction.Rollback();

                    response.Status = 0;
                    response.Errors.Add("ErrorMessage", error.Message);
                }
            }

            return await Task.FromResult(response);
        }
        #endregion

        #region delete
        public async Task<ResponseModel> DeleteChapter(int id, int version)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                int result = await _ChapterRepos.Delete(id, version);

                if (result == 1)
                {
                    result = await _ChapterRepos.Save();
                }

                response.Status = 1;
                response.Errors = null;
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", error.Message);
            }

            return response;
        }

        public async Task<ResponseModel> DeleteArea(int id, int version)
        {
            var response = new ResponseModel();

            try
            {
                int result = await _AreaRepos.Delete(id, version);

                if (result == 1)
                {
                    result = await _AreaRepos.Save();
                }

                response.Status = 1;
                response.Errors = null;
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", error.Message);
            }

            return response;
        }

        public async Task<ResponseModel> DeleteSubject(int id, int version)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var result = await _SubjectRepos.Delete(id, version);

                if (result == 1)
                {
                    result = await _SubjectRepos.Save();
                }

                response.Status = 1;
                response.Errors = null;
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", error.Message);
            }

            return response;
        }

        public async Task<ResponseModel> DeleteKeyword(int id, int version)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                int result = await _KeywordRepos.Delete(id, version);

                if (result == 1)
                {
                    result = await _KeywordRepos.Save();
                }

                response.Status = 1;
                response.Errors = null;
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", error.Message);
            }

            return response;
        }
        #endregion

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Context = null;
                _LocalAuthorityRepos.Dispose();
                _PolReqsRepos.Dispose();
                _ChapterRepos.Dispose();
                _LevelRepos.Dispose();
                _AreaRepos.Dispose();
                _SubjectRepos.Dispose();
                _ModificationQueueRepos.Dispose();
                _AttachmentRepos.Dispose();
                _SourceReferenceRepos.Dispose();
                _SeverityRepos.Dispose();
                _ChangeRequestRepos.Dispose();
                _PublicationRepos.Dispose();
                _PublicationQueueRepos.Dispose();
                _CommentRepos.Dispose();
                _KeywordRepos.Dispose();
                _ChildSubjectRepos.Dispose();
                _LocationRepos.Dispose();
                _PublicSpaceManagementThemeRepos.Dispose();
                _CommentValidator = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}