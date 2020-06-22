
using Microsoft.EntityFrameworkCore;
using System;
using Sieve.Services;
using System.IO;
using System.Drawing;

namespace KivalitaAPI.Repositories
{
    public class ImageRepository : Repository<Models.Image, DbContext, SieveProcessor>
    {
        public ImageRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }

        public override Models.Image Add(Models.Image image)
        {
            image.ImageData = Convert.FromBase64String(image.ImageString);
            image.ThumbnailData = resizeImage(image.ImageString);
            return base.Add(image);
        }

        private byte[] resizeImage(string ImageString)
        {
            byte[] bytes = Convert.FromBase64String(ImageString);

            Image image;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            var newWidth = 450;
            var newHeight = (newWidth * image.Height) / image.Width;

            Bitmap b = new Bitmap(newWidth, newHeight, image.PixelFormat);

            Graphics g = Graphics.FromImage((Image)b);

            g.DrawImage(image, 0, 0, newWidth, newHeight);
            g.Dispose();
            image = (Image)b;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();

                return imageBytes;
            }
        }
    }
}

