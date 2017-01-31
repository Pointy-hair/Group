using RevolutionaryStuff.Core;
using System.Collections.Generic;
using Traffk.Bal.Data;

namespace TraffkPortal.Models
{

    public class ListingFilters
    {
        public class Filter
        {
            public string Title { get; set; }
            public string FieldName { get; set; }
            public string Value { get; set; }
            public bool IsChecked { get; set; }
            public string OnSubmittedUrl { get; set; }
            public int Count { get; set; }
        }

        public class Section
        {
            public string SectionName { get; set; }

            public string SectionTitle => SectionName.ToTitleFriendlyString();

            public Filter[] Filters { get; set; }
            public int TotalCount { get; set; }
        }

        public IList<Section> Sections { get; set; }

        public ListingFilters()
        { }

        public ListingFilters(GetCountsResult counts, string path, string queryString)
        {
            var q = WebHelpers.ParseQueryParams(queryString);
            var sections = new List<Section>();
            var totalCount = 0;
            foreach (var fieldName in counts.CntByValByField.Keys)
            {
                var filters = new List<Filter>();
                var cntByVal = counts.CntByValByField[fieldName];
                foreach (var val in cntByVal.Keys)
                {
                    totalCount += cntByVal[val];
                    var f = new Filter
                    {
                        Title = $"{val} ({cntByVal[val]})",
                        Value = val,
                        FieldName = fieldName,
                        Count = cntByVal[val]
                    };
                    if (q.Contains(fieldName, val))
                    {
                        f.IsChecked = true;
                        q.Remove(fieldName, val);
                        if (q.Count == 0)
                        {
                            f.OnSubmittedUrl = path;
                        }
                        else
                        {
                            f.OnSubmittedUrl = WebHelpers.AppendParameters("", q.AtomEnumerable);
                        }
                        q.Add(fieldName, val);
                    }
                    else
                    {
                        f.IsChecked = false;
                        q.Add(fieldName, val);
                        f.OnSubmittedUrl = WebHelpers.AppendParameters("", q.AtomEnumerable);
                        q.Remove(fieldName, val);
                    }
                    filters.Add(f);
                }
                var section = new Section { SectionName = fieldName, Filters = filters.ToArray(), TotalCount = totalCount};
                sections.Add(section);
            }
            Sections = sections;
        }
    }
}
