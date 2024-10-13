
using ShareResource.Interfaces;
namespace ShareResource.Services
{
    public class ImageStorageConfig : IImageStorageConfig
    {
        public string ImageFolderPath { get; }

        public ImageStorageConfig(string imageFolderPath)
        {
            ImageFolderPath = imageFolderPath;
        }
    }
}
