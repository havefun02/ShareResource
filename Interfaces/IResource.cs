using ShareResource.Models.Entities;

namespace ShareResource.Interfaces

{
    public interface IResource<T> where T : class
    {
        public Task<T> UploadResource(T resource, string userId);
        public Task<int> DeleteResource(string resourceId,string userId);
        public Task<T> EditResource(T resource, string userId);
        public Task<ICollection<T>> GetDetailAllResource(string userId);
        public Task<T> GetDetailResourceById(string userId,string resourceId);
    }
}
