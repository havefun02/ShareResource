using CRUDFramework.Cores;
using CRUDFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using ShareResource.Database;
using ShareResource.Interfaces;
using ShareResource.Models.Entities;

namespace ShareResource.Services
{
    public class ResourceReaderService : IResourceReaderService<Img>
    {
        private readonly IRepository<Img, AppDbContext> _repository;
        private readonly IPaginationService<Img> _paginationService;
        public ResourceReaderService(IRepository<Img, AppDbContext> repository, IPaginationService<Img> paginationService)
        {
            _paginationService = paginationService;
            _repository = repository;
        }

        public async Task<IPaginationResult<Img>> GetPublicUserResources(IPaginationParams paginationParams, string userId)
        {
            var imgContext = _repository.GetDbSet();
            var query = imgContext.Include(i => i.User).Where(u => u.UserId == userId && u.IsPrivate==false);
            var paginationResult =await this._paginationService.Paginate(query, paginationParams);
            return paginationResult;
        }
        public async Task<IPaginationResult<Img>> GetUserResources(IPaginationParams paginationParams, string userId)
        {

            var imgContext = _repository.GetDbSet();
            var query = imgContext.Include(i => i.User).Where(u => u.UserId == userId);
            var paginationResult = await this._paginationService.Paginate(query, paginationParams);
            return paginationResult;
        }
        public async Task<IPaginationResult<Img>> GetSampleResource(IPaginationParams paginationParams)
        {
            var imgContext = _repository.GetDbSet();
            var query = imgContext.Include(i => i.User).Include(t => t.ImgLovers);
            var paginationResult = await this._paginationService.Paginate(query, paginationParams);
            return paginationResult;
        }
        public async Task<Img> GetResourceById(string resourceId)
        {
            var imgContext = _repository.GetDbSet();

            var resource = await imgContext.Include(i => i.User).SingleOrDefaultAsync(r => r.ImgId == resourceId);

            if (resource == null)
            {
                throw new ArgumentException("Resource not found");
            }

            if (resource.IsPrivate)
            {
                throw new UnauthorizedAccessException("User not authorized to access this resource");
            }
            return resource;
        }
    }
}
