using System;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DocumentPostTriggerAttribute : DocumentTriggerAttribute
    {
        public DocumentPostTriggerAttribute(string triggerName, DocumentTriggerOperations operations = DocumentTriggerOperations.All)
            : base(triggerName, operations)
        { }
    }
}
