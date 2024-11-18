using CRUDFramework.Interfaces;
using ShareResource.Database;
using ShareResource.Interfaces;
using ShareResource.Exceptions;
using ShareResource.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ShareResource.Models.Dtos;
using CRUDFramework;

namespace ShareResource.Services
{
    public class ResourceWriterService : IResourceWriterService<Img>
    {
        private readonly IRepository<Img, AppDbContext> _repository;
        private readonly IPaginationService<Img> _paginationService;



        public ResourceWriterService( IRepository<Img, AppDbContext> repository, IPaginationService<Img> paginationService)
        {
            _paginationService = paginationService;
            _repository = repository;
        }

        public async Task DeleteResource(string resourceId, string userId)
        {
            try
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

                await _repository.Delete(resourceId);
                await _repository.SaveAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task EditResource(Img resource, string userId)
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
            //existingResource.FileUrl = resource.FileUrl;
            //existingResource.ContentType = resource.ContentType;
            //existingResource.FileSize = resource.FileSize;
            //existingResource.Description = resource.Description;
            //existingResource.IsPrivate = resource.IsPrivate;

            _repository.Update(existingResource);
        }

       
        public async Task UpdateState(string userId,LikeDto likeDto)
        {
            try
            {
                var context = this._repository.GetDbContext();
                var user=await context.Users.Include(u=>u.UserImgs).SingleOrDefaultAsync(x => x.UserId == likeDto.userId);
                if (user == null || user.UserImgs== null) { throw new NullReferenceException("Cant find resource"); }
                var userImg = user.UserImgs.Where(i => i.ImgId == likeDto.resourceId).SingleOrDefault();
                if (userImg == null) { throw new NullReferenceException("The post is not available anymore"); }
                var likeImg=await context.ImgLovers.SingleOrDefaultAsync(t=>t.UserId==userId&&t.ImgId==likeDto.resourceId);

                if (likeImg==null)
                { 
                    if (likeDto.state)
                    {
                        await context.ImgLovers.AddAsync(new ImgLovers() { ImgId = userImg.ImgId, UserId = userId });
                        await context.SaveChangesAsync();
                    }
                    else
                    throw new InvalidOperationException();
                }
                else
                {
                    if (likeDto.state)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        context.ImgLovers.Remove(likeImg);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch {
                throw;
            }
        }
        public async Task UploadResource(Img resource, string userId)
        {
            try
            {

                resource.UserId = userId;
                await _repository.CreateAsync(resource);
            }
            catch (Exception ex) {
                throw new InternalException("Save img failed due to", ex);
            }
        }


    }
}
