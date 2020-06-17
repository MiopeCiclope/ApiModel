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

namespace KivalitaAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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
                            .ForMember(dest => dest.Id, opt => opt.Ignore())
                            .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.ImageData == null ? Convert.FromBase64String(src.ImageString) : src.ImageData));
                cfg.CreateMap<ImageHistory, Image>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TableId));
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
