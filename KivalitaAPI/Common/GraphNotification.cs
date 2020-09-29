using System;
using System.Collections.Generic;

namespace KivalitaAPI.Common
{
    // https://docs.microsoft.com/pt-br/graph/api/resources/changenotificationcollection?view=graph-rest-1.0

    public class GraphNotificationCollection
    {
        public ICollection<GraphNotification> Value;
    }

    public class GraphNotification
    {
        public string ChangeType;
        public string ClientState;
        public string Resource;
        public ResourceData ResourceData;
        public string SubscriptionId;
        public DateTime SubscriptionExpirationDateTime;
        public string TenantId;
    }

    public class ResourceData
    {
        public string Id;
    }
}
