﻿using Traffk.Utility;

namespace Traffk.Bal.ReportVisuals
{
    [IsSerializable(false)]
    public interface IReportResource
    {
        string Title { get; set; }
        string Description { get; set; }
    }
}
