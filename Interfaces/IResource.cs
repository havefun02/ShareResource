using CRUDFramework;
using CRUDFramework.Interfaces;
using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;

namespace ShareResource.Interfaces
{
    public interface IResourceReaderService<T> where T : class
    {
        public Task<IPaginationResult<T>> GetSampleResource(IPaginationParams paginationParams);
        public Task<T> GetResourceById(string resourceId);
        public Task<IPaginationResult<T>> GetPublicUserResources(IPaginationParams paginationParams,string userId);
        public Task<IPaginationResult<T>> GetUserResources(IPaginationParams paginationParams, string userId);


    }
    public interface IResourceWriterService<T> where T : class
    {
        public Task UploadResource(T resource, string userId);
        public Task DeleteResource(string resourceId,string userId);
        public Task EditResource(T resource, string userId);
        public Task UpdateState(string userId, LikeDto likeDto);

    }
}
