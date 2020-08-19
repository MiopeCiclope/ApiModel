﻿
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;
using System.Linq;
using System.Collections.Generic;

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
    }
}

