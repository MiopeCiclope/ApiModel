
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using KivalitaAPI.Common;
using System.Net;
using KivalitaAPI.DTOs;
using System.Collections.Generic;
using System;

namespace KivalitaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : CustomController<Company, CompanyService>
    {
        public CompanyController(CompanyService service, ILogger<CompanyController> logger) : base(service, logger) {}

    }
}
