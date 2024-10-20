using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Services;
using Microsoft.Identity.Web.Resource;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Caching.Memory;
using MemoryCache = System.Runtime.Caching.MemoryCache;

namespace DALI.PolicyRequirement.BaseController
{
    [EnableCors("AllowSpecificOrigin")]
    [ResponseCache(CacheProfileName = "Default")]

    //[Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class LiorControllerBase : ControllerBase
    {
        protected GraphServiceClient? _graphServiceClient;
        protected ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new ConcurrentDictionary<string, SemaphoreSlim>();
        private static readonly ReaderWriterLockSlim cacheLockPolReqs = new ReaderWriterLockSlim();
        private PublicSpaceManagementServices _Services;

        public LiorControllerBase(PublicSpaceManagementServices services, GraphServiceClient graphServiceClient)
        {
            _Services = services;
            _graphServiceClient = graphServiceClient;
        }

        protected PublicSpaceManagementServices Services
        {
            get
            {
                return _Services;
            }
        }

        protected async Task<int> GetCurrentVersion()
        {
            var key = "CurrentVersionWebApi";

            MemoryCache cache = MemoryCache.Default;
            CacheItemPolicy cachePolicy = new CacheItemPolicy();
            cachePolicy.AbsoluteExpiration = DateTime.Now.AddSeconds(3600);

            int? version = (int?)cache.Get(key);

            if (version == null)
            {
                // get the semaphore specific to this username
                var keyLock = _keyLocks.GetOrAdd(key, x => new SemaphoreSlim(1));

                await keyLock.WaitAsync().ConfigureAwait(false);

                try
                {
                    // if value isn't cached, get it from the DB asynchronously
                    version = await Services.GetCurrentVersion().ConfigureAwait(false);

                    // cache value
                    cache.Add(key, version, cachePolicy);
                }
                finally
                {
                    keyLock.Release();
                }
            }

            return version ?? 0;
        }

        protected List<PolicyRequirementModel> PolicyRequirements
        {
            get
            {
                var key = "PolicyRequirementsWebApi";
                //First we do a read lock to see if it already exists, this allows multiple readers at the same time.
                cacheLockPolReqs.EnterReadLock();
                try
                {
                    var model = (List<PolicyRequirementModel>)MemoryCache.Default.Get(key);

                    if (model != null)
                    {
                        return model;
                    }
                }
                finally
                {
                    cacheLockPolReqs.ExitReadLock();
                }

                //Only one UpgradeableReadLock can exist at one time, but it can co-exist with many ReadLocks
                cacheLockPolReqs.EnterUpgradeableReadLock();
                try
                {
                    //We need to check again to see if the string was created while we where waiting to enter the EnterUpgradeableReadLock
                    var model = (List<PolicyRequirementModel>)MemoryCache.Default.Get(key);

                    if (model != null)
                    {
                        return model;
                    }

                    //The entry still does not exist so we need to enter the write lock and create it, this will block till all the Readers flush.
                    cacheLockPolReqs.EnterWriteLock();
                    try
                    {
                        InitInMemoryPolicyRequirements();

                        return (List<PolicyRequirementModel>)MemoryCache.Default.Get(key);
                    }
                    finally
                    {
                        cacheLockPolReqs.ExitWriteLock();
                    }
                }
                finally
                {
                    cacheLockPolReqs.ExitUpgradeableReadLock();
                }
            }
        }

        protected void InitInMemoryPolicyRequirements()
        {
            var version = Task.Run(async () => await GetCurrentVersion().ConfigureAwait(false)).Result;

            var models = Task.Run(async () => await Services.GetAll(version).ConfigureAwait(false)).Result;

            CacheItemPolicy cip = new CacheItemPolicy()
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(3600))
            };

            MemoryCache.Default.Set("PolicyRequirementsWebApi", models, cip);
        }
    }
}
