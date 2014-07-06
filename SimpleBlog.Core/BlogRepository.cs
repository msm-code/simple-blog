using SimpleBlog.Core.Objects;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System;

namespace SimpleBlog.Core {
    public class BlogRepository : IBlogRepository {
        private readonly ISession session;

        public BlogRepository(ISession session) {
            this.session = session;
        }

        public IList<Post> Posts(int pageNo, int pageSize) {
            var query = session.Query<Post>()
                .Where(p => p.Published)
                .OrderByDescending(p => p.PostedOn)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Fetch(p => p.Category);

            query.FetchMany(p => p.Tags).ToFuture();

            return query.ToFuture().ToList();
        }

        public int TotalPosts() {
            return TotalPosts(true);
        }

        public IList<Post> PostsForCategory(string categorySlug, int pageNo, int pageSize) {
            var query = session.Query<Post>()
                .Where(p => p.Published && p.Category.UrlSlug.Equals(categorySlug))
                .OrderByDescending(p => p.PostedOn)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Fetch(p => p.Category);

            query.FetchMany(p => p.Tags).ToFuture();
            return query.ToFuture().ToList();
        }

        public int TotalPostsForCategory(string categorySlug) {
            return session.Query<Post>()
                .Where(p => p.Published && p.Category.UrlSlug.Equals(categorySlug))
                .Count();
        }

        public Category Category(string categorySlug) {
            return session.Query<Category>()
                .FirstOrDefault(t => t.UrlSlug.Equals(categorySlug));
        }

        public IList<Post> PostsForTag(string tagSlug, int pageNo, int pageSize) {
            var query = session.Query<Post>()
                .Where(p => p.Published && p.Tags.Any(t => t.UrlSlug.Equals(tagSlug)))
                .OrderByDescending(p => p.PostedOn)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Fetch(p => p.Category);

            query.FetchMany(p => p.Tags).ToFuture();
            return query.ToFuture().ToList();
        }

        public int TotalPostsForTag(string tagSlug) {
            return session.Query<Post>()
                .Where(p => p.Published && p.Tags.Any(t => t.UrlSlug.Equals(tagSlug)))
                .Count();
        }

        public Tag Tag(string tagSlug) {
            return session.Query<Tag>()
                .FirstOrDefault(t => t.UrlSlug.Equals(tagSlug));
        }


        public IList<Post> PostsForSearch(string search, int pageNo, int pageSize) {
            var query = session.Query<Post>()
                .Where(p => p.Published && (p.Title.Contains(search)
                    || p.Category.Name.Contains(search)
                    || p.Tags.Any(t => t.Name.Contains(search))))
                .OrderByDescending(p => p.PostedOn)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Fetch(p => p.Category);

            query.FetchMany(p => p.Tags).ToFuture();
            return query.ToFuture().ToList();
        }

        public int TotalPostsForSearch(string search) {
            return session.Query<Post>()
                .Where(p => p.Published && (p.Title.Contains(search)
                    || p.Category.Name.Equals(search)
                    || p.Tags.Any(t => t.Name.Equals(search))))
                .Count();
        }


        public Post Post(int year, int month, string titleSlug) {
            var query = session.Query<Post>()
                .Where(p => p.PostedOn.Year == year &&
                        p.PostedOn.Month == month &&
                        p.UrlSlug == titleSlug)
                .Fetch(p => p.Category);

            query.FetchMany(p => p.Tags).ToFuture();
            return query.ToFuture().Single();
        }


        public IList<Category> Categories() {
            return session.Query<Category>().OrderBy(p => p.Name).ToList();
        }


        public IList<Tag> Tags() {
            return session.Query<Tag>().OrderBy(p => p.Name).ToList();
        }


        public IList<Post> LatestPosts() {
            return session.Query<Post>()
                .OrderBy(p => p.PostedOn)
                .Take(5)
                .ToList();
        }


        public IList<Post> Posts(int pageNo, int pageSize, string sortColumn, bool sortAsc) {
            Func<IQueryable<Post>, IOrderedQueryable<Post>> orderAsc = null;
            Func<IQueryable<Post>, IOrderedQueryable<Post>> orderDesc = null;

            switch (sortColumn) {
                case "Title":
                    orderAsc = (q) => Queryable.OrderBy(q, p => p.Title);
                    orderDesc = (q) => Queryable.OrderByDescending(q, p => p.Title);
                    break;
                case "Published":
                    orderAsc = (q) => Queryable.OrderBy(q, p => p.Published);
                    orderDesc = (q) => Queryable.OrderByDescending(q, p => p.Published);
                    break;
                case "PostedOn":
                    orderAsc = (q) => Queryable.OrderBy(q, p => p.PostedOn);
                    orderDesc = (q) => Queryable.OrderByDescending(q, p => p.PostedOn);
                    break; 
                case "Modified": 
                    orderAsc = (q) => Queryable.OrderBy(q, p => p.Modified);
                    orderDesc = (q) => Queryable.OrderByDescending(q, p => p.Modified);
                    break; 
                case "Category":
                    orderAsc = (q) => Queryable.OrderBy(q, p => p.Category.Name);
                    orderDesc = (q) => Queryable.OrderByDescending(q, p => p.Category.Name);
                    break; 
            }

            var orderFunc = sortAsc ? orderAsc : orderDesc;

            IQueryable<Post> query = orderFunc(session.Query<Post>())
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Fetch(p => p.Category);
            
            query.FetchMany(p => p.Tags).ToFuture();

            return query.ToFuture().ToList();
        }

        public int TotalPosts(bool checkIsPublished = true) {
            return session.Query<Post>()
                .Where(p => checkIsPublished || p.Published == true)
                .Count();
        }


        public int AddPost(Post post) {
            using (var tran = session.BeginTransaction()) {
                session.Save(post);
                tran.Commit();
                return post.Id;
            }
        }

        public Category Category(int id) {
            return session.Query<Category>().FirstOrDefault(t => t.Id == id);
        }

        public Tag Tag(int id) {
            return session.Query<Tag>().FirstOrDefault(t => t.Id == id);
        }

        public void EditPost(Post post) {
            using (var tran = session.BeginTransaction()) {
                session.SaveOrUpdate(post);
                tran.Commit();
            }
        }

        public void DeletePost(int id) {
            using (var tran = session.BeginTransaction()) {
                var post = session.Get<Post>(id);
                session.Delete(post);
                tran.Commit();
            }
        }

        public IQueryable<Objects.Post> Func { get; set; }
    }
}
