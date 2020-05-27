﻿
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KivalitaAPI.Services
{

    public class JobService : Service<Job, KivalitaApiContext, JobRepository>
    {
        private ImageRepository _imageRepository;
        private UserRepository _userRepository;

        public JobService(KivalitaApiContext context, JobRepository baseRepository) : base(context, baseRepository)
        {
            _imageRepository = new ImageRepository(this.context);
            _userRepository = new UserRepository(this.context);
        }

        public override Job Add(Job Job)
        {
            var JobImage = new Image
            {
                ImageData = Convert.FromBase64String(Job.ImageData),
                Type = "Job"
            };

            var storedImage = this._imageRepository.Add(JobImage);
            Job.ImageId = storedImage.Id;

            return base.Add(Job);
        }

        public override Job Get(int id)
        {
            var storedJob = base.Get(id);
            storedJob.JobImage = this._imageRepository.Get(storedJob.ImageId);
            storedJob.Author = this._userRepository.Get(storedJob.AuthorId);

            return storedJob;
        }

        public override List<Job> GetAll()
        {
            var storedJobs = base.GetAll();
            var authors = storedJobs.Select(Job => Job.AuthorId);

            var JobImages = this._imageRepository.GetBy(image => image.Type == "Job");
            var JobAuthors = this._userRepository.GetBy(user => authors.Contains(user.Id));

            storedJobs = storedJobs.Select(Job =>
            {
                Job.JobImage = JobImages.Where(image => image.Id == Job.ImageId).First();
                Job.Author = JobAuthors.Where(author => author.Id == Job.AuthorId).First();
                return Job;
            }).ToList();

            return storedJobs;
        }
    }
}

