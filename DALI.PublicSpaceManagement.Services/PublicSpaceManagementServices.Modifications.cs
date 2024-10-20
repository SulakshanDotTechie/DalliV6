using DALI.Enums;
using DALI.Models;
using DALI.PolicyRequirements.DomainModels;
using DALI.PolicyRequirements.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace DALI.PublicSpaceManagement.Services
{
	public partial class PublicSpaceManagementServices
	{
		#region Modifications
		public Task<bool> ModificationsToBePublished()
		{
			return _PublicationQueueRepos.ModificationsToBePublished();
		}

		public async Task<List<PolicyRequirementModificationModel>> GetQueuedModifications(int version, string owner)
		{
			List<PolicyRequirementModificationModel> models = await _ModificationQueueRepos.GetAll(version, owner);

			return models;
		}

		public async Task<PolicyRequirementModificationModel> GetQueuedModification(int id, int version)
		{
			PolicyRequirementModificationModel model = await _ModificationQueueRepos.GetOneModelAsync(id, version);

			if (model == null)
			{
				bool changeRequestIsRegistered = await _ChangeRequestRepos.IsRegistered(id, version);

				if (changeRequestIsRegistered)
				{
					model = new PolicyRequirementModificationModel();
					model.Id = id;
					model.LocalAuthority.Id = Guid.NewGuid();
					model.LocalAuthority.Description = Localization.Unknown;
					model.Chapter.Id = 0;
					model.Chapter.Description = Localization.Unknown;
					model.Level.Id = 0;
					model.Level.Description = Localization.Unknown;
					model.Location.Id = Guid.NewGuid();
					model.Location.Description = Localization.Unknown;
					model.Area.Id = 0;
					model.Area.Description = Localization.Unknown;
					model.Subject.Id = 0;
					model.Subject.Description = Localization.Unknown;
					model.Description = Localization.ModificationModelNotExists;
					model.Active = true;
					model.CreatedBy = "UNKNOWN";
					model.CreatedDate = DateTime.Now;
					model.ModifiedBy = "UNKNOWN";
					model.ModifiedDate = DateTime.Now;
					model.Strictnesses.Add(new PolicyRequirementSeverityModel() { Id = 0, Description = Localization.Unknown });
				}
			}

			return model;
		}





		public void StartQueueTransaction()
		{
			_ModificationQueueRepos.StartTransaction();
		}

		public void CommitQueueTransaction()
		{
			_ModificationQueueRepos.StartTransaction();
		}

		public void RollbackQueueTransaction()
		{
			_ModificationQueueRepos.RollbackTransaction();
		}


		public async Task<ResponseModel> QueueModifications(PolicyRequirementModificationModel model, string userName, int currentVersion)
		{
			var response = new ResponseModel();
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.Parse("60"), TransactionScopeAsyncFlowOption.Enabled))
			{
				try
				{
					model.ModifiedBy = userName;
					model.ModifiedDate = DateTime.Now;
					model.VersionId = currentVersion;
					model.Description = WebUtility.HtmlEncode(model.Description);

					var qModel = await GetQueuedModification(model.Id, currentVersion);

					if (qModel == null || string.Compare(qModel.CreatedBy, Localization.Unknown, true) == 0)
					{
						response = await QueueModifications(model);

						if (response.Status == 1)
						{
							qModel = await GetQueuedModification(model.Id, currentVersion);

							if (qModel != null)
							{
								var action = (model.Id <= 0) ? ReasonEnum.Add : ReasonEnum.Edit;

								response = await AddChangeRequest(qModel, action, currentVersion);
							}
							else
							{
								throw new Exception("Error occurred, cannot queue the modifications!");
							}
						}
						else
						{
							throw new Exception("Error occurred, cannot queue the modifications!");
						}
					}
					else
					{
						response = await UpdateModifications(model);

						if (response.Status == 1)
						{
							response.Status = 1;
							response.Errors = null;
						}
						else
						{
							throw new Exception("Error occurred, cannot queue the modifications!");
						}
					}

					scope.Complete();
					scope.Dispose();
				}
				catch (Exception error)
				{
					scope.Dispose();
					response.Status = 0;
					response.Errors.Add("ErrorMessage", error.Message);
				}
			}

			return response;
		}

		public async Task<ResponseModel> QueueModifications(PolicyRequirementModificationModel model)
		{
			ResponseModel response = new ResponseModel();
			int result = -1;
			try
			{
				PolicyRequirementModificationModel prmModel = await _ModificationQueueRepos.GetOneModelAsync(model.Id, model.VersionId);
				PolicyRequirementLocalAuthorityModel localAuth = await _LocalAuthorityRepos.GetFirst();

				// The model is already queued!
				if (prmModel != null)
				{
					response.Status = 1;
					response.Id = prmModel.Id;
					response.Errors = null;

					return response;
				}

				PolicyRequirementModel versionedModel = await _PolReqsRepos.GetOneModelAsync(model.Id, model.VersionId);

				// Add complete new model to the queue
				if (versionedModel == null && prmModel == null)
				{
					prmModel = await _ModificationQueueRepos.New(true);
					prmModel.LocalAuthority.Id = localAuth.Id;
					prmModel.VersionId = model.VersionId;
					prmModel.Active = true;
					prmModel.CreatedBy = model.CreatedBy;
					prmModel.CreatedDate = DateTime.Now;
					prmModel.ModifiedBy = model.CreatedBy;
					prmModel.ModifiedDate = prmModel.CreatedDate;
					prmModel.Description = model.Description;
					prmModel.Chapter.Id = model.Chapter.Id;
					prmModel.Location.Id = model.Location.Id;
					prmModel.Level.Id = model.Level.Id;
					prmModel.Area.Id = model.Area.Id;
					prmModel.Subject.Id = model.Subject.Id;
					prmModel.ChildSubject.Id = model.ChildSubject.Id;
					prmModel.GroupIndex = model.GroupIndex;
					prmModel.Owner = model.Owner;

					model.Id = prmModel.Id;

					result = await _ModificationQueueRepos.Add(prmModel);

					foreach (var m in model.Strictnesses)
					{
						await _SeverityRepos.AddToQueue(m.Id, prmModel.Id, prmModel.VersionId);
					}

					foreach (var m in model.SourceDocuments)
					{
						await _SourceReferenceRepos.AddToQueue(m.Id, prmModel.Id, prmModel.VersionId);
					}

					foreach (var m in model.Attachments)
					{
						await _AttachmentRepos.AddToQueue(m.Id, prmModel.Id, prmModel.VersionId);
					}
				}

				// Add existing model of the currentversion to the queue for modifications
				if (versionedModel != null && prmModel == null)
				{
					prmModel = await _ModificationQueueRepos.New(false);

					prmModel.Id = versionedModel.Id;
					prmModel.LocalAuthority.Id = localAuth.Id;
					prmModel.VersionId = versionedModel.VersionId;
					prmModel.Active = versionedModel.Active;
					prmModel.CreatedBy = versionedModel.CreatedBy;
					prmModel.CreatedDate = versionedModel.CreatedDate;
					prmModel.Chapter.Id = versionedModel.Chapter.Id;
					prmModel.Location.Id = versionedModel.Location.Id;
					prmModel.Level.Id = versionedModel.Level.Id;
					prmModel.Area.Id = versionedModel.Area.Id;
					prmModel.Subject.Id = versionedModel.Subject.Id;
					prmModel.ChildSubject.Id = versionedModel.ChildSubject.Id;
					prmModel.GroupIndex = versionedModel.GroupIndex;
					prmModel.Owner = versionedModel.Owner;

					prmModel.Description = model.Description ?? versionedModel.Description;
					prmModel.ModifiedBy = model.ModifiedBy;
					prmModel.ModifiedDate = model.ModifiedDate;

					result = await _ModificationQueueRepos.Add(prmModel);

					List<PolicyRequirementAttachmentModel> storedAttachments = await GetAttachments(prmModel.Id, prmModel.VersionId);
					List<PolicyRequirementSourceDocumentModel> storedSourceRefs = await GetSourceDocuments(prmModel.Id, prmModel.VersionId);
					List<PolicyRequirementSeverityModel> storedSeverities = await GetSeverities(prmModel.Id, prmModel.VersionId);

					foreach (PolicyRequirementAttachmentModel s in storedAttachments)
					{
						PolicyRequirementAttachmentModel att = model.Attachments.SingleOrDefault(matt => s.Id == matt.Id);
						if (att == null)
						{
							await _AttachmentRepos.AddToQueue(s.Id, prmModel.Id, prmModel.VersionId);
						}
					}

					foreach (PolicyRequirementAttachmentModel matt in model.Attachments)
					{
						var att = storedAttachments.SingleOrDefault(s => s.Id == matt.Id);
						if (att == null)
						{
							await _AttachmentRepos.AddToQueue(matt.Id, prmModel.Id, prmModel.VersionId);
						}
					}

					foreach (PolicyRequirementSourceDocumentModel s in storedSourceRefs)
					{
						PolicyRequirementSourceDocumentModel srcf = model.SourceDocuments.SingleOrDefault(sf => s.Id == sf.Id);
						if (srcf == null)
						{
							await _SourceReferenceRepos.AddToQueue(s.Id, prmModel.Id, prmModel.VersionId);
						}
					}

					foreach (PolicyRequirementSourceDocumentModel sf in model.SourceDocuments)
					{
						PolicyRequirementSourceDocumentModel srcf = storedSourceRefs.SingleOrDefault(s => s.Id == sf.Id);
						if (srcf == null)
						{
							await _SourceReferenceRepos.AddToQueue(sf.Id, prmModel.Id, prmModel.VersionId);
						}
					}

					foreach (PolicyRequirementSeverityModel s in model.Strictnesses)
					{
						await _SeverityRepos.AddToQueue(s.Id, prmModel.Id, prmModel.VersionId);
					}
				}

				if (prmModel != null)
				{
					if (result == 1)
					{
						result = await _ModificationQueueRepos.Save();
					}

					if (result == 1)
					{

						response.Status = 1;
						response.Id = prmModel.Id;
						response.Errors = null;
					}
					else
					{
						response.Status = 0;
						response.Id = prmModel.Id;
						response.Errors.Add("ErrorMessage", "An error occurred on queuing the modification");
					}
				}
			}
			catch (Exception error)
			{
				throw error;
			}

			return response;
		}

		public async Task<ResponseModel> UpdateModifications(PolicyRequirementModificationModel model)
		{
			var response = new ResponseModel();

			var status = await _ModificationQueueRepos.Update(model);


			var addStses = false;
			var delStses = false;

			if (status == 1)
			{
				var attachments = model.Attachments;
				var srcdocs = model.SourceDocuments;
				var strictnesses = model.Strictnesses;

				if (strictnesses.Count > 0)
				{
					var stses = await _SeverityRepos.GetQueued(model.Id, model.VersionId);

					if (stses.Count > 0)
					{
						var ids = strictnesses.Select(s => s.Id);
						var dqsts = stses.Where(s => !ids.Contains(s.Id));
						foreach (var sts in dqsts)
						{
							var result = await _SeverityRepos.DeleteFromQueue(sts.Id, sts.PolicyRequirementId, sts.VersionId);

							if (!delStses && result == 1)
								delStses = true;
						}
					}

					foreach (var m in strictnesses)
					{
						var result = await _SeverityRepos.AddToQueue(m.Id, model.Id, model.VersionId);

						if (!addStses && result == 1)
							addStses = true;
					}
				}

				foreach (var m in srcdocs)
				{
					if (m.IsAssigned)
					{
						await _SourceReferenceRepos.AddToQueue(m.Id, model.Id, model.VersionId);
					}

					if (!m.IsAssigned)
					{
						await _SourceReferenceRepos.DeleteFromQueue(m.Id, model.Id, model.VersionId);
					}
				}

				foreach (var m in attachments)
				{
					if (m.IsAssigned)
					{
						await _AttachmentRepos.AddToQueue(m.Id, model.Id, model.VersionId);
					}

					if (!m.IsAssigned)
					{
						await _AttachmentRepos.DeleteFromQueue(m.Id, model.Id, model.VersionId);
					}
				}

				status = 1;

				if (status == 1)
				{
					status = await _ModificationQueueRepos.Save();
				}
			}

			if (status == 1)
			{
				response.Status = 1;
				response.Id = model.Id;
				response.Errors = null;
			}
			else
			{
				response.Status = 0;
				response.Errors.Add("ErrorMessage", "An error occured on updating the queued modification");
			}


			return response;
		}

		public async Task<ResponseModel> DequeueModifications(int policyRequirementId, int version)
		{
			var response = new ResponseModel();

			try
			{
				var qSeverities = await GetQueuedSeverities(policyRequirementId, version);
				var qSourceDocuments = await GetQueuedSourceDocuments(policyRequirementId, version);
				var qAttachments = await GetQueuedAttachments(policyRequirementId, version);

				if (qSeverities.Count > 0)
				{
					foreach (var m in qSeverities)
					{
						await _SeverityRepos.DeleteFromQueue(m.Id, policyRequirementId, version);
					}
				}

				if (qSourceDocuments.Count > 0)
				{
					foreach (var m in qSourceDocuments)
					{
						await _SourceReferenceRepos.DeleteFromQueue(m.Id, policyRequirementId, version);
					}
				}

				if (qAttachments.Count > 0)
				{
					foreach (var m in qAttachments)
					{
						await _AttachmentRepos.DeleteFromQueue(m.Id, policyRequirementId, version);
					}
				}

				var result = await _ModificationQueueRepos.Delete(policyRequirementId, version);

				if (result == 1)
				{
					//await _snsRepos.Save();
					//await _srcRepos.Save();
					//await _atmRepos.Save();

					result = await _ModificationQueueRepos.Save();
				}

				response.Status = 1;
				response.Errors = null;
			}
			catch (Exception error)
			{
				throw error;
			}

			return response;
		}

		public async Task<ResponseModel> UpdateOrder(List<int> orderList, int version, string userName)
		{
			var response = new ResponseModel();
			int processedItems = 0;

			try
			{
				foreach (var id in orderList)
				{
					int index = orderList.IndexOf(id);

					var status = await _PolReqsRepos.UpdateOrder(id, index, version, userName);

					if (status == -1)
					{
						throw new Exception(Localization.DenyUpdateOrder);
					}

					if (status == 0)
					{
						throw new Exception(Localization.ChangeOrderFailed);
					}

					if (status == 1)
						processedItems++;
				}

				if (processedItems == orderList.Count())
				{
					await _PolReqsRepos.Save();
				}
				else
					throw new Exception(Localization.ChangeOrderFailed);

				response.Status = 1;
				response.Errors = null;
			}
			catch (Exception error)
			{
				response.Status = 0;
				response.Errors.Add("UpdateOrder", error.Message);
			}

			return response;
		}

		public async Task<bool> ChapterIsQueued(int id, int versionId)
		{
			return await _ModificationQueueRepos.ChapterIsQueued(id, versionId);
		}

		public async Task<bool> AreaIsQueued(int id, int versionId)
		{
			return await _ModificationQueueRepos.AreaIsQueued(id, versionId);
		}

		public async Task<bool> SubjectIsQueued(int id, int versionId)
		{
			return await _ModificationQueueRepos.SubjectIsQueued(id, versionId);
		}
		#endregion
	}
}
