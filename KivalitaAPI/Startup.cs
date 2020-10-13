using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Data;
using KivalitaAPI.Services;
using KivalitaAPI.Repositories;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using AutoMapper;
using KivalitaAPI.Models;
using KivalitaAPI.AuditModels;
using Microsoft.Extensions.Logging;
using Sieve.Services;
using Sieve.Models;
using KivalitaAPI.DTOs;
using KivalitaAPI.Common;
using Quartz.Spi;
using Quartz;
using Quartz.Impl;
using KivalitaAPI.Interfaces;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace KivalitaAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static readonly ILoggerFactory sqlLogger = LoggerFactory.Create(builder => { builder.AddConsole(); });

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Settings>(Configuration.GetSection("Settings"));
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin();
                });
            });

            services.AddControllers().AddNewtonsoftJson();

            var key = Encoding.ASCII.GetBytes(Setting.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Kivalita API",
                        Version = "v1",
                        Description = "Master API",
                        Contact = new OpenApiContact
                        {
                            Name = "Kivalita Consulting",
                            Url = new Uri("https://kivalitaconsulting.com.br")
                        }
                    });
                c.EnableAnnotations();
                c.IgnoreObsoleteActions();
            });

            services.AddDbContext<KivalitaApiContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                    //options.UseLoggerFactory(sqlLogger);
                });

            services.AddScoped<DbContext, KivalitaApiContext>();

            services.AddScoped<UserRepository>();
            services.AddScoped<UserService>();

            services.AddScoped<FilterRepository>();
            services.AddScoped<FilterService>();

            services.AddScoped<TokenRepository>();
            services.AddScoped<TokenService>();

            services.AddScoped<PostRepository>();
            services.AddScoped<PostService>();

            services.AddScoped<JobRepository>();
            services.AddScoped<JobService>();

            services.AddScoped<ImageRepository>();
            services.AddScoped<ImageService>();

            services.AddScoped<LeadsRepository>();
            services.AddScoped<LeadsService>();

            services.AddScoped<CompanyRepository>();
            services.AddScoped<CompanyService>();

            services.AddScoped<GetEmailService>();

            services.AddScoped<IEmailExtractorService, EmailExtractorService>();
            services.AddScoped<RequestService>();

            services.AddScoped<WpRdStationRepository>();
            services.AddScoped<WpRdStationService>();

            services.AddScoped<MicrosoftTokenRepository>();
            services.AddScoped<MicrosoftTokenService>();

            services.AddScoped<CategoryRepository>();
            services.AddScoped<CategoryService>();

            services.AddScoped<TemplateRepository>();
            services.AddScoped<TemplateService>();
            services.AddScoped<TemplateTransformService>();

            services.AddScoped<FlowTaskRepository>();
            services.AddScoped<FlowTaskService>();
            services.AddScoped<ScheduleTasksService>();

            services.AddScoped<TaskNoteRepository>();
            services.AddScoped<TaskNoteService>();

            services.AddScoped<FlowActionRepository>();
            services.AddScoped<FlowRepository>();
            services.AddScoped<FlowService>();

            services.AddScoped<MailTrackRepository>();
            services.AddScoped<MailTrackService>();

            services.AddScoped<LogTaskRepository>();
            services.AddScoped<LogTaskService>();

            services.AddScoped<TagRepository>();
            services.AddScoped<TagService>();

            services.AddScoped<LeadTagRepository>();

            services.AddScoped<MailSignatureRepository>();
            services.AddScoped<MailSignatureService>();

            services.AddScoped<PreLeadRepository>();
            services.AddScoped<PreLeadService>();

            services.AddScoped<MailAnsweredRepository>();
            services.AddScoped<MailAnsweredService>();

            services.AddScoped<FlowTaskDTORepository>();

            services.AddScoped<ISieveCustomFilterMethods, SieveCustomFilterMethods>();

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<UserHistory, User>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<Token, TokenHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<TokenHistory, Token>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<Post, PostHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<PostHistory, Post>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<Image, ImageHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<ImageHistory, Image>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<Job, JobHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<JobHistory, Job>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<Leads, LeadsHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<LeadsHistory, Leads>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<Company, CompanyHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<CompanyHistory, Company>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<Leads, LeadDTO>();
                cfg.CreateMap<LeadDTO, Leads>();

                cfg.CreateMap<GraphAuthDTO, MicrosoftToken>()
                            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => DateTime.UtcNow.AddSeconds(src.expires_in - 10)))
                            .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.access_token))
                            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.refresh_token));
                cfg.CreateMap<MicrosoftToken, GraphAuthDTO>();

                cfg.CreateMap<Category, CategoryHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<CategoryHistory, Category>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<Template, TemplateHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<TemplateHistory, Template>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<Flow, FlowHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<FlowHistory, Flow>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<FlowAction, FlowActionHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<FlowActionHistory, FlowAction>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<FlowTask, FlowTaskHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<FlowTaskHistory, FlowTask>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<TaskNote, TaskNoteHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<TaskNoteHistory, TaskNote>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<MailTrack, MailTrackHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<MailTrackHistory, MailTrack>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<Tag, TagHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<TagHistory, Tag>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<LeadTag, LeadTagHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<LeadTagHistory, LeadTag>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<MailSignature, MailSignatureHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<MailSignatureHistory, MailSignature>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<FlowTaskDTO, FlowTask>();
                cfg.CreateMap<FlowTask, FlowTaskDTO>();

                cfg.CreateMap<PreLead, PreLeadHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<PreLeadHistory, PreLead>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));

                cfg.CreateMap<MailAnswered, MailAnsweredHistory>()
                            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.Id, opt => opt.Ignore());
                cfg.CreateMap<MailAnsweredHistory, MailAnswered>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));
            });

            //Mapper Configured service
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            //Sieve filter/pagination/sorting for entity framework
            services.Configure<SieveOptions>(Configuration.GetSection("Sieve"));
            services.AddScoped<SieveProcessor>();

            //Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            //Add job as Singleton
            services.AddSingleton<BaseJob>();
            services.AddSingleton<SendMailJob>();
            services.AddSingleton<TaskJob>();
            services.AddSingleton<ReplyCheckJob>();
            services.AddSingleton<GetMailJob>();
            services.AddSingleton<MailSchedulerJob>();

            services.AddSingleton(new JobScheduleDTO("MailSchedulerJob", "0 0 9 1/1 * ? *", null, 0));
            services.AddSingleton(new JobScheduleDTO("GetMailJob", null, DateTimeOffset.UtcNow, 0));

            services.AddHostedService<SchedulerService>();
            services.AddSingleton<IJobScheduler, SchedulerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
                          ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("../Logs/Api-{Date}.log");
            // Ativando middlewares para uso do Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Kivalita API");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x =>
            {
                x.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/Resources")
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
