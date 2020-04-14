
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class ImageService : Service<Image, KivalitaApiContext, ImageRepository>
    {
        public ImageService(KivalitaApiContext context, ImageRepository baseRepository) : base(context, baseRepository) { }
    }
}


