
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class MailSignatureService : Service<MailSignature, KivalitaApiContext, MailSignatureRepository>
    {
        public MailSignatureService(KivalitaApiContext context, MailSignatureRepository baseRepository) : base(context, baseRepository) { }
    }
}


