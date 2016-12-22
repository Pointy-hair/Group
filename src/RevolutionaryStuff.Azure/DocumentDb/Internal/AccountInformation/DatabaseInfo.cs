using System.Collections.Generic;

namespace RevolutionaryStuff.Azure.DocumentDb.Internal.AccountInformation
{
    public class WritableLocation
    {
        public string name { get; set; }
        public string databaseAccountEndpoint { get; set; }
    }

    public class ReadableLocation
    {
        public string name { get; set; }
        public string databaseAccountEndpoint { get; set; }
    }

    public class UserReplicationPolicy
    {
        public bool asyncReplication { get; set; }
        public int minReplicaSetSize { get; set; }
        public int maxReplicasetSize { get; set; }
    }

    public class UserConsistencyPolicy
    {
        public string defaultConsistencyLevel { get; set; }
    }

    public class SystemReplicationPolicy
    {
        public int minReplicaSetSize { get; set; }
        public int maxReplicasetSize { get; set; }
    }

    public class ReadPolicy
    {
        public int primaryReadCoefficient { get; set; }
        public int secondaryReadCoefficient { get; set; }
    }

    public class Account
    {
        public string _self { get; set; }
        public string id { get; set; }
        public string _rid { get; set; }
        public string media { get; set; }
        public string addresses { get; set; }
        public string _dbs { get; set; }
        public List<WritableLocation> writableLocations { get; set; }
        public List<ReadableLocation> readableLocations { get; set; }
        public UserReplicationPolicy userReplicationPolicy { get; set; }
        public UserConsistencyPolicy userConsistencyPolicy { get; set; }
        public SystemReplicationPolicy systemReplicationPolicy { get; set; }
        public ReadPolicy readPolicy { get; set; }
        public string queryEngineConfiguration { get; set; }
    }
}
