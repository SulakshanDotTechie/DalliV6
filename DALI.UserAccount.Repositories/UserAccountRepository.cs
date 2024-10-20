using DALI.UserAccount.DataEntityModel;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DALI.UserAccount.Models;

namespace DALI.UserAccount.Repositories
{
    public class UserAccountRepository : IDisposable
    {
        private DALI_UserAccount_Entities _Context;

        public UserAccountRepository()
        {
            _Context = new DALI_UserAccount_Entities();
        }

        public async Task<List<UserModel>> GetAll()
        {
            var entities = await _Context.AspNetUsers.AsNoTracking().ToArrayAsync();

            var models = entities.Select(e => new UserModel()
            {
                UserName = e.UserName,
                EmailAddress = e.Email
            }).ToList();

            return models;
        }


        public async Task<UserSelectionViewModel[]> GetUserSelectionListAsync()
        {
            var models = await _Context.AspNetUsers.AsNoTracking().OrderBy(e => e.UserName).Select(u => new UserSelectionViewModel()
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                AccountIsBlocked = u.LockoutEndDateUtc.HasValue ? (u.LockoutEndDateUtc != null) : false,
                //After DALI V4 phoneNumber is mis-used to store organisation info
                Organization = u.PhoneNumber,
                Roles = u.AspNetRoles.Select(e => e.Description).ToList()
            }).ToArrayAsync();

            return models.ToArray();
        }


        public async Task<UserSelectionViewModel[]> GetUserSelectionListAsync(string role)
        {
            UserSelectionViewModel[] models = await GetUserSelectionListAsync();

            return models.Where(e => e.Roles.Contains(role)).ToArray();
        }

        public async Task<string[]> GetRoles(string userName)
        {
            var entity = await _Context.AspNetUsers.AsNoTracking().SingleOrDefaultAsync(e => string.Compare(e.UserName, userName, true) == 0);

            if (entity != null)
            {
                return entity.AspNetRoles.Select(e => e.Name).ToArray();
            }

            return new string[] { };
        }


        public async Task<List<UsersByRoleModel>> GetUsersByRole()
        {
            var roles = await _Context.AspNetRoles.AsNoTracking().Where(e => !e.Description.ToLower().EndsWith("pve")).Select(e => e.Description).ToArrayAsync();
            List<UsersByRoleModel> models = new List<UsersByRoleModel>();

            foreach (var role in roles)
            {
                var users = await GetUserSelectionListAsync(role);

                UsersByRoleModel model = new UsersByRoleModel(role, users);

                models.Add(model);
            }

            return models;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_Context != null)
                {
                    _Context.Dispose();
                    _Context = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
