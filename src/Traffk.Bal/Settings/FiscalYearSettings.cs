using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;

namespace Traffk.Bal.Settings
{
    public class FiscalYearSettings : IValidate
    {
        private static readonly DataContractJsonSerializer JsonSerializer = new DataContractJsonSerializer(typeof(FiscalYearSettings));

        [Required]
        [DisplayName("Calendar Year")]
        [DataMember(Name = "CalendarYear")]
        public Int32 CalendarYear { get; set; }

        [Required]
        [DisplayName("Calendar Month")]
        [DataMember(Name = "CalendarMonth")]
        public Int32 CalendarMonth { get; set; }

        [Required]
        [DisplayName("Fiscal Year")]
        [DataMember(Name = "FiscalYear")]
        public Int16 FiscalYear { get; set; }

        public FiscalYearSettings()
        { }

        public FiscalYearSettings(FiscalYearSettings other)
        {
            if (other == null) return;
            CalendarYear = other.CalendarYear;
            CalendarMonth = other.CalendarMonth;
            FiscalYear = other.FiscalYear;
        }

        public string ToJson()
        {
            return JsonSerializer.WriteObjectToString(this);
        }

        void IValidate.Validate()
        {
            Requires.True(CalendarMonth > 0 && CalendarMonth < 13, nameof(CalendarMonth));
        }
    }
}
