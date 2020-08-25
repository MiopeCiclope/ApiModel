
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class TagService : Service<Tag, KivalitaApiContext, TagRepository>
    {
        public TagService(KivalitaApiContext context, TagRepository baseRepository) : base(context, baseRepository) { }
    }
}


