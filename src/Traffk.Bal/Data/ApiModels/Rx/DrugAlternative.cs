using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Traffk.Orchestra.Models;

namespace Traffk.Bal.Data.ApiModels.Rx
{

    public class DrugAlternativeResponse
    {
        private string ObjectName = ApiObjectTypes.ObjectNames.DrugAlternative.ToString().ToLower();
        private string IdPrefix = "OD";

        public DrugAlternativeResponse()
        {
            Object = ObjectName;
        }

        public DrugAlternativeResponse(DrugToReplace o)
        {
            Object = ObjectName;
            Name = o.DrugName;
            ConditionDescription = o.ConditionDescription;
            Warning = o.Warning;
            Drug = new Drug(o);

            var alternativeList = new List<Alternative>();
            if (o.Options != null)
            {
                foreach (var option in o.Options)
                {
                    var alternative = new Alternative(option);
                    alternativeList.Add(alternative);
                }

                Alternatives = new Alternatives { Data = alternativeList.ToArray() };
            }

            Id = IdPrefix + Drug.Id;
        }

        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string ConditionDescription { get; set; }
        [JsonProperty("warning")]
        public object Warning { get; set; }
        public Drug Drug { get; set; }
        public Alternatives Alternatives { get; set; }
    }
    public class Alternatives
    {
        public Alternatives()
        {
        }

        [JsonProperty("object")]
        public string Object { get; set; } = ApiObjectTypes.ObjectNames.List.ToString().ToLower();
        [JsonProperty("data")]
        public Alternative[] Data { get; set; }
    }

    public class Alternative
    {
        public Alternative()
        {
            
        }

        public Alternative(Option o)
        {
            TotalCost = o.AWPTotalCost;
            IsDirectGeneric = o.Alternatives.ToList().Any(x => x.IsDirectGeneric);
            SwitchScore = o.SwitchScore;
            SwitchDescription = o.SwitchDescription;
            var alternativeDrugList = new List<Drug>();
            foreach (var alternative in o.Alternatives)
            {
                var drug = new Drug(alternative);
                alternativeDrugList.Add(drug);
            }

            var alternativeDrugs = new AlternativeDrugs
            {
                Data = alternativeDrugList.ToArray()
            };

            Drugs = alternativeDrugs;
        }

        [JsonProperty("object")]
        public string Object { get; set; } = typeof(Alternative).Name.ToLower();

        [JsonProperty("avgPrice")]
        public float TotalCost { get; set; }
        [JsonProperty("isDirectGeneric")]
        public bool IsDirectGeneric { get; set; }
        [JsonProperty("switchScore")]
        public int SwitchScore { get; set; }
        [JsonProperty("switchDescription")]
        public string SwitchDescription { get; set; }
        [JsonProperty("drugs")]
        public AlternativeDrugs Drugs { get; set; }
    }

    public class AlternativeDrugs
    {
        [JsonProperty("object")]
        public string Object { get; set; } = ApiObjectTypes.ObjectNames.List.ToString().ToLower();
        [JsonProperty("data")]
        public Drug[] Data { get; set; }
    }

}
