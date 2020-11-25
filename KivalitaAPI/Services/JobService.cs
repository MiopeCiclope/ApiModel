
using KivalitaAPI.Common;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KivalitaAPI.Services
{

    public class JobService : Service<Job, KivalitaApiContext, JobRepository>
    {
        private ImageRepository _imageRepository;
        private UserRepository _userRepository;

        public JobService(KivalitaApiContext context, JobRepository baseRepository, ImageRepository imageRepository) : base(context, baseRepository)
        {
            _imageRepository = imageRepository;
            _userRepository = new UserRepository(this.context, this.baseRepository.filterProcessor);
        }

        public override Job Add(Job Job)
        {
            var JobImage = new Image
            {
                ImageString = Job.ImageData,
                CreatedBy = Job.AuthorId,
                Type = "Job"
            };

            var storedImage = this._imageRepository.Add(JobImage);
            Job.ImageId = storedImage.Id;

            return base.Add(Job);
        }

        public override Job Update(Job Job)
        {
            var oldJob = baseRepository.Get(Job.Id);

            if (!String.IsNullOrEmpty(Job.ImageData) && !Convert.ToBoolean(Job.ImageId))
            {
                var JobImage = new Image
                {
                    ImageString = Job.ImageData,
                    Type = "Job"
                };

                var storedImage = this._imageRepository.Add(JobImage);
                Job.ImageId = storedImage.Id;
            }
            else
            {
                Job.ImageId = oldJob.ImageId;
            }

            return base.Update(Job);
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
                Job.JobImage = JobImages.Where(image => image.Id == Job.ImageId)
                    .First();

                Job.Author = JobAuthors.Where(author => author.Id == Job.AuthorId).First();
                return Job;
            }).ToList();

            return storedJobs;
        }

        public override QueryResult<Job> GetAll_v2(SieveModel filterQuery)
        {
            var storedJobs = baseRepository.GetAll_v2(filterQuery);
            var authors = storedJobs.Items.Select(Job => Job.AuthorId);

            var JobImages = this._imageRepository.GetBy(image => image.Type == "Job");
            var JobAuthors = this._userRepository.GetBy(user => authors.Contains(user.Id));

            storedJobs.Items = storedJobs.Items.Select(Job =>
            {
                Job.JobImage = JobImages.Where(image => image.Id == Job.ImageId)
                    .First();

                Job.Author = JobAuthors.Where(author => author.Id == Job.AuthorId).First();
                return Job;
            }).ToList();

            return storedJobs;
        }
    }
}


