using CRUDFramework;
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
            _paginationService = paginationService  ;
            _repository = repository;
        }

        public async Task<IPaginationResult<Img>> GetPublicUserResources(IPaginationParams paginationParams, string userId)
        {
            var imgContext = _repository.GetDbSet();
            var query = imgContext.Include(i=>i.ImgLovers).Where(u => u.UserId == userId && u.IsPrivate==false);
            var paginationResult =await this._paginationService.Paginate(query, paginationParams);
            return paginationResult;
        }
        public async Task<List<Img>> GetUserResources(string userId)
        {
            var imgContext = _repository.GetDbSet();
            var imgs =await  imgContext.Where(u => u.UserId == userId && u.IsPrivate == false).ToListAsync();
            return imgs;
        }

        public async Task<IPaginationResult<Img>> GetUserResources(IPaginationParams paginationParams, string userId)
        {
            var imgContext = _repository.GetDbSet();
            var userImg= imgContext.Include(il=>il.ImgLovers).Where(u=>u.UserId == userId);
            var paginationResult = await _paginationService.Paginate(userImg, paginationParams);
            if (paginationResult == null)
                throw new NotFoundException("Can not find result");
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

            var resource = await imgContext.SingleOrDefaultAsync(r => r.ImgId == resourceId);

            if (resource == null)
            {
                throw new NullReferenceException("Resource not found");
            }

            if (resource.IsPrivate)
            {
                throw new UnauthorizedAccessException("User not authorized to access this resource");
            }
            return resource;
        }
    }
}
