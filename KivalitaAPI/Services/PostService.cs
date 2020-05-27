﻿
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

        public PostService(KivalitaApiContext context, PostRepository baseRepository) : base(context, baseRepository) 
        {
            _imageRepository = new ImageRepository(this.context);
            _userRepository = new UserRepository(this.context);
        }

        public override Post Add(Post post)
        {
            var postImage = new Image
            {
                ImageData = Convert.FromBase64String(post.ImageData),
                Type = "Blog"
            };

            var storedImage = this._imageRepository.Add(postImage);
            post.ImageId = storedImage.Id;

            return base.Add(post);
        }

        public override Post Get(int id)
        {
            var storedPost = base.Get(id);
            storedPost.PostImage = this._imageRepository.Get(storedPost.ImageId);
            storedPost.Author = this._userRepository.Get(storedPost.AuthorId);

            return storedPost;
        }

        public override List<Post> GetAll()
        {
            var storedPosts = base.GetAll();
            var authors = storedPosts.Select(post => post.AuthorId);

            var postImages = this._imageRepository.GetBy(image => image.Type == "Blog");
            var postAuthors = this._userRepository.GetBy(user => authors.Contains(user.Id));

            storedPosts = storedPosts.Select(post => 
            {
                post.PostImage = postImages.Where(image => image.Id == post.ImageId).First();
                post.Author = postAuthors.Where(author => author.Id == post.AuthorId).First();
                return post; 
            }).ToList();

            return storedPosts;
        }
    }
}

