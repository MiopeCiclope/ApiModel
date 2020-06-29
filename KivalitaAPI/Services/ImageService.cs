
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KivalitaAPI.Services
{

    public class ImageService : Service<Image, KivalitaApiContext, ImageRepository>
    {
        public ImageService(KivalitaApiContext context, ImageRepository baseRepository) : base(context, baseRepository) { }

        public List<Image> GetByType(string imageType)
        {
            return baseRepository.GetListByQuery(
                $@"Select id
                        , null as ImageData
                        , type
                        , thumbnaildata
                        , createdat
                        , CreatedBy
                        , UpdatedAt
                        , updatedby 
                from Image 
                    where type = '{imageType}'").ToList();
        }
    }
}


