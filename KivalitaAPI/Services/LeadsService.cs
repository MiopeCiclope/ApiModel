using System;
using System.Security.Cryptography;
using System.Text;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services {

	public class LeadsService : Service<Leads, KivalitaApiContext, LeadsRepository> {
		public LeadsService (KivalitaApiContext context, LeadsRepository baseRepository) : base (context, baseRepository) { }

	}
}
