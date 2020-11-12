
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Models;
using Sieve.Services;
using System.Linq;
using System.Collections.Generic;
using KivalitaAPI.Common;

namespace KivalitaAPI.Repositories
{
    public class PostRepository : Repository<Post, DbContext, SieveProcessor>
    {
        public PostRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }

        public Post GetAsNoTracking(int id)
        {
            return context.Set<Post>()
                .Where(l => l.Id == id)
                .AsNoTracking()
                .SingleOrDefault();
        }

        public List<Post> GetByLinkId(int linkId)
        {
            return context.Set<Post>()
                .Where(p => p.Id == linkId || p.LinkId == linkId)
                .ToList();
        }

        public Post GetBySlug(string slug)
        {
            return context.Set<Post>()
                .Where(p => p.Slug == slug).FirstOrDefault();
        }

        public override QueryResult<Post> GetAll_v2(SieveModel filterQuery)
        {
            var result = context.Set<Post>().AsNoTracking();

            var total = result.Count();
            result = this.filterProcessor.Apply(filterQuery, result);

            return new QueryResult<Post>
            {
                Items = result.Select(p => new Post
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    ImageId = p.ImageId,
                    PostImage = p.PostImage,
                    Language = p.Language,
                    isNews = p.isNews,
                    Published = p.Published,
                    AuthorId = p.AuthorId,
                    CreatedAt = p.CreatedAt
                }).OrderByDescending(p => p.CreatedAt).ToList(),
                TotalItems = total,
            };
        }
    }
}

