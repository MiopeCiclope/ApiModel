
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;

namespace KivalitaAPI.Repositories
{
    public class ImageRepository : Repository<Image, DbContext>
    {
        public ImageRepository(DbContext context) : base(context) { }
    }
}

