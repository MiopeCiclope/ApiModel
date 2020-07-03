using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace KivalitaAPI.Repositories {
	public class CategoryRepository: Repository<Category, DbContext, SieveProcessor> {
		public CategoryRepository(DbContext context, SieveProcessor categoryProcessor) : base (context, categoryProcessor) { }
	}
}
