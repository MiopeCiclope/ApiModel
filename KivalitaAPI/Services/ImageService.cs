
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using System.Collections.Generic;

namespace KivalitaAPI.Services
{

    public class ImageService : Service<Image, KivalitaApiContext, ImageRepository>
    {
        public ImageService(KivalitaApiContext context, ImageRepository baseRepository) : base(context, baseRepository) { }

        public List<Image> GetByType(string imageType)
        {
            return baseRepository.GetByType(imageType);
        }
    }
}


