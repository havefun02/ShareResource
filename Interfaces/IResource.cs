﻿using CRUDFramework.Interfaces;
using ShareResource.Models.Entities;

namespace ShareResource.Interfaces
{
    public interface IResourceAccess<T> where T : class
    {
        public Task<IPaginationResult<T>> GetSampleResource(IPaginationParams paginationParams);
        public Task<T> GetResourceById(string resourceId);
        public Task<IPaginationResult<T>> GetPublicUserResources(IPaginationParams paginationParams,string userId);
        public Task<IPaginationResult<T>> GetUserResources(IPaginationParams paginationParams, string userId);


    }
    public interface IResourceMod<T> where T : class
    {
        public Task<T> UploadResource(T resource, string userId);
        public Task<int> DeleteResource(string resourceId,string userId);
        public Task<T> EditResource(T resource, string userId);
    }
}
