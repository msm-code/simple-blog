using SimpleBlog.Core;
using SimpleBlog.Core.Objects;
using System.Collections.Generic;
namespace SimpleBlog.Models {
    public class WidgetViewModel {
        public WidgetViewModel(IBlogRepository blogRepository) {
            Categories = blogRepository.Categories();
            Tags = blogRepository.Tags();
            LatestPosts = blogRepository.LatestPosts();
        }

        public IList<Category> Categories { get; private set; }
        public IList<Tag> Tags { get; private set; }
        public IList<Post> LatestPosts { get; private set; }
    }
}