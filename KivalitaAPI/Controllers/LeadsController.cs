using System;
using KivalitaAPI.Common;
using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KivalitaAPI.Controllers {
	[Route ("api/[controller]")]
	[ApiController]
	public class LeadsController : CustomController<Leads, LeadsService> {
		public LeadsController (LeadsService service, ILogger<LeadsController> logger) : base (service, logger) { }
	}
}
