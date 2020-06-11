
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using System;

namespace KivalitaAPI.Repositories
{
    public class ImageRepository : Repository<Image, DbContext>
    {
        public ImageRepository(DbContext context) : base(context) { }

        public override Image Add(Image Image)
        {
            var NewImage = new Image
            {
                ImageData = Convert.FromBase64String(Image.ImageString),
                Type = Image.Type
            };

            return base.Add(NewImage);
        }
    }
}

