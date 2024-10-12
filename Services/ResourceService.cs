using CRUDFramework.Interfaces;
using ShareResource.Database;
using ShareResource.Interfaces;
using ShareResource.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ShareResource.Services
{
    public class ResourceService : IResource<Img>
    {
        private readonly IRepository<Img, AppDbContext> _repository;

        public ResourceService(IRepository<Img, AppDbContext> repository)
        {
            _repository = repository;
        }

        public async Task<int> DeleteResource(string resourceId, string userId)
        {
            var resource = await _repository.FindOneById(resourceId);
            if (resource == null)
            {
                throw new ArgumentException("Resource not found");
            }

            if (resource.UserId != userId)
            {
                throw new UnauthorizedAccessException("User not authorized to delete this resource");
            }

            return await _repository.Delete(resourceId);
        }

        public async Task<Img> EditResource(Img resource, string userId)
        {
            var existingResource = await _repository.FindOneById(resource.ImgId);
            if (existingResource == null)
            {
                throw new ArgumentException("Resource not found");
            }

            if (existingResource.UserId != userId)
            {
                throw new UnauthorizedAccessException("User not authorized to edit this resource");
            }

            existingResource.FileName = resource.FileName;
            existingResource.FileUrl = resource.FileUrl;
            existingResource.ContentType = resource.ContentType;
            existingResource.FileSize = resource.FileSize;
            existingResource.Description = resource.Description;
            existingResource.IsPrivate = resource.IsPrivate;

            var updateResult=await _repository.Update(existingResource);
            return updateResult;
        }

        public async Task<ICollection<Img>> GetDetailAllResource(string userId)
        {
            var imgContext = _repository.GetDbSet();
            var resources = await imgContext.Where(r=>r.UserId==userId).ToListAsync();

            return resources;
        }

        public async Task<Img> GetDetailResourceById(string userId, string resourceId)
        {
            var resource = await _repository.FindOneById(resourceId);
            if (resource == null)
            {
                throw new ArgumentException("Resource not found");
            }

            if (resource.IsPrivate && resource.UserId != userId)
            {
                throw new UnauthorizedAccessException("User not authorized to access this resource");
            }
            return resource;
        }

        public async Task<Img> UploadResource(Img resource, string userId)
        {
            resource.UserId = userId;
            var createdResult=await _repository.CreateAsync(resource);
            return createdResult;
        }
    }
}
