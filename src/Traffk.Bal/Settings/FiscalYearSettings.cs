using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Traffk.Bal.Settings
{
    public class FiscalYearSettings : IValidate
    {
        [Required]
        [DisplayName("Calendar Year")]
        [JsonProperty("CalendarYear")]
        public int CalendarYear { get; set; }

        [Required]
        [DisplayName("Calendar Month")]
        [JsonProperty("CalendarMonth")]
        public int CalendarMonth { get; set; }

        [Required]
        [DisplayName("Fiscal Year")]
        [JsonProperty("FiscalYear")]
        public int FiscalYear { get; set; }

        public FiscalYearSettings()
        { }

        public FiscalYearSettings(FiscalYearSettings other)
        {
            if (other == null) return;
            CalendarYear = other.CalendarYear;
            CalendarMonth = other.CalendarMonth;
            FiscalYear = other.FiscalYear;
        }

        public static FiscalYearSettings CreateFromJson(string json)
            => TraffkHelpers.JsonConvertDeserializeObjectOrFallback<FiscalYearSettings>(json);

        public string ToJson()
            => JsonConvert.SerializeObject(this);

        void IValidate.Validate()
        {
            Requires.Between(CalendarMonth, nameof(CalendarMonth), 1, 12);
        }
    }
}
