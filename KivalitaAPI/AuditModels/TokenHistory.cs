using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KivalitaAPI.AuditModels
{
    public class TokenHistory : IAuditTable
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
        public LoginTypeEnum LoginClient { get; set; }
        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
