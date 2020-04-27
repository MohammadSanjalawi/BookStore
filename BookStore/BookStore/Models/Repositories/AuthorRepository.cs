using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.Repositories
{
    public class AuthorRepository : IBookstoreRepository<Author>
    {
        IList<Author> authors;

        public AuthorRepository()
        {
            authors = new List<Author>() {
                new Author{ Id = 1 , FullName = "Mohammad Sanjalawi" },
                new Author{ Id = 2 , FullName = "Mohammad AlMushet" },
                new Author{ Id = 3 , FullName = "Mohammad Hamdan" }
            };
        }

        public void Add(Author entity)
        {
            authors.Add(entity);
        }

        public void Delete(int id)
        {
            var author = Find(id);
            authors.Remove(author);
        }

        public Author Find(int id)
        {
            var author = authors.SingleOrDefault(b => b.Id == id);

            return author;
        }

        public IList<Author> List()
        {
            return authors;
        }

        public void Update(int id, Author NewAuthor)
        {
            var author = Find(id);

            author.FullName = NewAuthor.FullName;
        }
    }
}
