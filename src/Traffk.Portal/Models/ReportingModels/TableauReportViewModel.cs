using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevolutionaryStuff.Core;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.ReportVisuals;

namespace TraffkPortal.Models.ReportingModels
{
    public class TableauReportViewModel
    {
        public TableauReportViewModel(ReportVisual reportVisual)
        {
            WorkbookName = reportVisual.Parameters.SingleOrDefault(p => p.Key == nameof(WorkbookName)).Value;
            ViewName = reportVisual.Parameters.SingleOrDefault(p => p.Key == nameof(ViewName)).Value;
            FolderName = reportVisual.FolderName;
            Description = reportVisual.Description;
            Title = reportVisual.Title;
        }

        public string Title { get; private set; }
        public string FolderName { get; private set; }
        public string WorkbookName { get; private set; }
        public string ViewName { get; private set; }
        public string Description { get; private set; }
    }

    //public class TableauContactReportViewModel : TableauReportViewModel
    //{
    //    public TableauContactReportViewModel(ReportVisual reportVisual, Contact contact) : base(reportVisual)
    //    {
    //        Requires.NonNull(contact, nameof(contact));
    //        ContactId = contact.ContactId;
    //    }

    //    public long ContactId { get; private set; }
    //}
}
