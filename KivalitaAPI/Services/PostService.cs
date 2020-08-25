
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
                CreatedBy = post.AuthorId,
                Type = "Blog"
            };

            var storedImage = this._imageRepository.Add(postImage);
            post.ImageId = storedImage.Id;
            post.Slug = GenerateSlug(post.Title);

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

            post.Slug = GenerateSlug(post.Title);
            return base.Update(post);
        }

        public override Post Get(int id)
        {
            var storedPost = base.Get(id);
            storedPost.PostImage = this._imageRepository.Get(storedPost.ImageId);
            storedPost.Author = this._userRepository.Get(storedPost.AuthorId);

            return storedPost;
        }

        public List<Post> GetBySlug(string slug)
        {
            var postBySlug = baseRepository.GetBySlug(slug);
            var posts = baseRepository.GetByLinkId(postBySlug.Id);

            foreach (var post in posts)
            {
                post.PostImage = this._imageRepository.Get(post.ImageId);
                post.Author = this._userRepository.Get(post.AuthorId);
            }

            return posts;
        }

        private string GenerateSlug(string phrase)
        {
            string str = RemoveAccent(phrase).ToLower();
        
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            str = Regex.Replace(str, @"\s+", " ").Trim();

            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        private string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}


