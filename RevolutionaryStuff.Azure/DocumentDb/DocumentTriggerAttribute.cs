using RevolutionaryStuff.Core;
using System;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    public abstract class DocumentTriggerAttribute : Attribute
    {
        public string TriggerName { get; }
        public DocumentTriggerOperations Operations { get; }

        public DocumentTriggerAttribute(string triggerName, DocumentTriggerOperations operations)
        {
            Requires.Match(RegexHelpers.Common.CSharpIdentifier, triggerName, nameof(triggerName));

            Operations = operations;
            TriggerName = triggerName;
        }
    }
}
