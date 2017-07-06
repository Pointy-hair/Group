namespace Traffk.Bal.BackgroundJobs
{
    public static class BackgroundJobHelpers
    {
        /// <remarks>
        /// The Queue name argument must consist of lowercase letters, digits and underscore characters only.
        /// http://docs.hangfire.io/en/latest/background-processing/configuring-queues.html?highlight=queue
        /// </remarks>
        public static class QueueNames
        {
            public const string SsisQueue = "ssis";
            public const string TableauQueue = "tableau";
        }
    }
}
