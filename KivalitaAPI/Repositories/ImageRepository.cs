
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using System;

namespace KivalitaAPI.Repositories
{
    public class ImageRepository : Repository<Image, DbContext>
    {
        public ImageRepository(DbContext context) : base(context) { }

        public override Image Add(Image image)
        {
            image.ImageData = Convert.FromBase64String(image.ImageString);
            return base.Add(image);
        }
    }
}

