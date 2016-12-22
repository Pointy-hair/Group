using System;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DocumentPreTriggerAttribute : DocumentTriggerAttribute
    {
        public DocumentPreTriggerAttribute(string triggerName, DocumentTriggerOperations operations=DocumentTriggerOperations.All)
            : base(triggerName, operations)
        { }
    }
}
