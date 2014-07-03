using SimpleBlog.Core;
using SimpleBlog.Core.Objects;
using System.Collections.Generic;

namespace SimpleBlog.Models {
    public class ListViewModel {
        private ListViewModel() { }

        public ListViewModel(IBlogRepository blogRepository, int p) {
            Posts = blogRepository.Posts(p - 1, 10);
            TotalPosts = blogRepository.TotalPosts();
        }

        public IList<Post> Posts { get; private set; }
        public int TotalPosts { get; private set; }

        public Category Category { get; private set; }
        public Tag Tag { get; private set; }
        public string Search { get; private set; }

        public static ListViewModel FromCategory(IBlogRepository blogRepository, string categorySlug, int p) {
            var model = new ListViewModel();
            return new ListViewModel {
                Posts = blogRepository.PostsForCategory(categorySlug, p - 1, 10),
                TotalPosts = blogRepository.TotalPostsForCategory(categorySlug),
                Category = blogRepository.Category(categorySlug)
            };
        }

        public static ListViewModel FromTag(IBlogRepository blogRepository, string tagSlug, int p) {
            return new ListViewModel {
                Posts = blogRepository.PostsForCategory(tagSlug, p - 1, 10),
                TotalPosts = blogRepository.TotalPostsForTag(tagSlug),
                Tag = blogRepository.Tag(tagSlug)
            };
        }

        public static ListViewModel FromSearch(IBlogRepository blogRepository, string searchText, int p) {
            return new ListViewModel {
                Posts = blogRepository.PostsForSearch(searchText, p - 1, 10),
                TotalPosts = blogRepository.TotalPostsForSearch(searchText),
                Search = searchText
            };
        }
    }
}