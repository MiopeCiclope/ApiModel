
using Microsoft.EntityFrameworkCore;
using System;
using Sieve.Services;
using System.IO;
using KivalitaAPI.Common;
using Microsoft.Extensions.Options;
using KivalitaAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace KivalitaAPI.Repositories
{
    public class ImageRepository : Repository<Image, DbContext, SieveProcessor>
    {
        private readonly Settings _myConfiguration;

        public ImageRepository(
            DbContext context,
            SieveProcessor filterProcessor,
            IOptions<Settings> settings
        ) : base(context, filterProcessor)
        {
            _myConfiguration = settings.Value;
        }

        public override Image Add(Image image)
        {
            var folderName = Path.Combine("Resources", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            var fileName = string.Format(@"{0}.jpg", Guid.NewGuid());
            string imgPath = Path.Combine(pathToSave, fileName);

            byte[] imageBytes = Convert.FromBase64String(image.ImageString);

            File.WriteAllBytes(imgPath, imageBytes);

            image.FileName = fileName;
            image.ImageUrl = $"{_myConfiguration.Host}/resources/images/{fileName}";

            return base.Add(image);
        }

        public override Image Update(Image image)
        {
            var folderName = Path.Combine("Resources", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            var fileName = image.FileName;
            string imgPath = Path.Combine(pathToSave, fileName);

            byte[] imageBytes = Convert.FromBase64String(image.ImageString);

            File.Delete(imgPath);
            File.WriteAllBytes(imgPath, imageBytes);

            return base.Update(image);
        }

        public List<Image> GetByType(string imageType)
        {
            return context.Set<Image>()
                .Where(im => im.Type == imageType)
                .ToList();
        }

    }
}

