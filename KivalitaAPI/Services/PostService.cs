
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KivalitaAPI.Services
{

    public class PostService : Service<Post, KivalitaApiContext, PostRepository>
    {
        private ImageRepository _imageRepository;
        private UserRepository _userRepository;

        public PostService(KivalitaApiContext context, PostRepository baseRepository, ImageRepository imageRepository) : base(context, baseRepository) 
        {
            _imageRepository = imageRepository;
            _userRepository = new UserRepository(this.context, this.baseRepository.filterProcessor);
        }

        public override Post Add(Post post)
        {
            var postImage = new Image
            {
                ImageString = post.ImageData,
                Type = "Blog"
            };

            var storedImage = this._imageRepository.Add(postImage);
            post.ImageId = storedImage.Id;

            var postAdded = base.Add(post);

            if (postAdded.LinkId.HasValue)
            {
                var postLinked = baseRepository.Get((int)postAdded.LinkId);
                postLinked.LinkId = postAdded.Id;
                base.Update(postLinked);
            }

            return postAdded;
        }

        public override Post Update(Post post)
        {
            var oldPost = baseRepository.GetAsNoTracking(post.Id);
            if (oldPost.LinkId.HasValue && oldPost.LinkId != post.LinkId)
            {
                var oldLinked = baseRepository.Get((int)oldPost.LinkId);
                oldLinked.LinkId = null;
                base.Update(oldLinked);
            }

            if (!String.IsNullOrEmpty(post.ImageData) && !Convert.ToBoolean(post.ImageId))
            {
                var JobImage = new Image
                {
                	ImageString = post.ImageData,
                    Type = "Blog"
                };

                var storedImage = this._imageRepository.Add(JobImage);
                post.ImageId = storedImage.Id;
            }
            else
            {
                post.ImageId = oldPost.ImageId;
            }

            if (post.LinkId.HasValue)
            {
                var postLinked = baseRepository.Get((int)post.LinkId);
                postLinked.LinkId = post.Id;
                base.Update(postLinked);
            }

            return base.Update(post);
        }

        public override Post Get(int id)
        {
            var storedPost = base.Get(id);
            storedPost.PostImage = this._imageRepository.Get(storedPost.ImageId);
            storedPost.Author = this._userRepository.Get(storedPost.AuthorId);

            return storedPost;
        }

        public List<Post> GetByLinkId(int id)
        {
            var posts = baseRepository.GetByLinkId(id);
            foreach (var post in posts)
            {
                post.PostImage = this._imageRepository.Get(post.ImageId);
                post.Author = this._userRepository.Get(post.AuthorId);
            }

            return posts;
        }

        public override List<Post> GetAll()
        {
            var storedPosts = base.GetAll();
            var authors = storedPosts.Select(post => post.AuthorId);

            var postImages = this._imageRepository.GetListByQuery(
                                            $@"Select id
                                                    , type
                                                    , fileName
                                                    , url
                                                    , CreatedAt
                                                    , createdBy
                                                    , updatedAt
                                                    , updatedby
                                            from Image
                                                where type = 'Blog'");
            var postAuthors = this._userRepository.GetBy(user => authors.Contains(user.Id));

            storedPosts = storedPosts.Select(post =>
            {
                post.PostImage = postImages.Where(image => image.Id == post.ImageId)
                    .First();

                post.Author = postAuthors.Where(author => author.Id == post.AuthorId).First();
                return post;
            }).OrderByDescending(p => p.Id).ToList();

            return storedPosts;
        }
    }
}


