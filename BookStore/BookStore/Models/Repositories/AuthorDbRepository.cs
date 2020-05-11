using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.Repositories
{
    public class AuthorDbRepository : IBookstoreRepository<Author>
    {
        BookStoreDbContext db;

        public AuthorDbRepository(BookStoreDbContext _db)
        {
            db = _db;
        }

        public void Add(Author entity)
        { 
            db.Authors.Add(entity);
            SaveChanges();
        }

        public void Delete(int id)
        {
            var author = Find(id);
            db.Authors.Remove(author);
            SaveChanges();
        }

        public Author Find(int id)
        {
            var author = db.Authors.SingleOrDefault(b => b.Id == id);

            return author;
        }

        public IList<Author> List()
        {
            return db.Authors.ToList();
        }

        public void Update(int id, Author NewAuthor)
        {
            db.Update(NewAuthor);
            SaveChanges();
        }

        void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
