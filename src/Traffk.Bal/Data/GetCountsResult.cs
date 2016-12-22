using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traffk.Bal.Data
{
    public class GetCountsResult
    {
        [JsonProperty("docCnt")]
        public int DocCount { get; set; }

        [JsonProperty("sql")]
        internal string Sql { get; set; }

        public class Item
        {
            [Column("fieldName")]
            [JsonProperty("field")]
            public string Field { get; set; }

            [Column("fieldVal")]
            [JsonProperty("val")]
            public string Val { get; set; }

            [Column("fieldCnt")]
            [JsonProperty("occurrences")]
            public int Count { get; set; }
        }

        [JsonProperty("items")]
        internal List<Item> Items { get; set; }

        [JsonProperty("cntByValByField")]
        //        public dynamic CntByValByField { get; set; }
        public Dictionary<string, Dictionary<string, int>> CntByValByField { get; set; } = new Dictionary<string, Dictionary<string, int>>();

        public GetCountsResult() { }

        public GetCountsResult(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                if (item.Field == null || item.Val == null) continue;
                var d = CntByValByField.FindOrCreate(item.Field, () => new Dictionary<string, int>(Comparers.CaseInsensitiveStringComparer));
                d[item.Val] = item.Count;
            }
        }
    }
}
