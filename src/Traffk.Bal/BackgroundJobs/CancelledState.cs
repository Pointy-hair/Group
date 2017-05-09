using System;
using System.Collections.Generic;
using System.Text;
using Hangfire.States;
using Newtonsoft.Json;

namespace Traffk.Bal.BackgroundJobs
{
    public class CancelledState : IState
    {
        public static readonly string StateName = "Cancelled";

        string IState.Name => StateName;

        string IState.Reason => "User Cancelled";

        bool IState.IsFinal => true;

        bool IState.IgnoreJobLoadException => true;

        Dictionary<string, string> IState.SerializeData()
        {
            var result = new Dictionary<string, string>();
            result.Add(StateName, JsonConvert.SerializeObject(this));
            return result;
        }
    }
}
