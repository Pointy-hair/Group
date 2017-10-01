using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Traffk.Utility
{
    public class ReportTreeNode<TData>
    {
        public TData Data { get; private set; }

        public ReportTreeNode<TData> Parent { get; set; }

        public ReportTreeNode<TData>[] ChildrenArray
        {
            get => Children.ToArray();
            set => Children = value.ToList();
        }

        [JsonIgnore]
        public IList<ReportTreeNode<TData>> Children { get; set; } = new List<ReportTreeNode<TData>>();

        public bool HasChildren => Children.Count > 0;

        public ReportTreeNode<TData> this[int index] => Children[index];

        public void Add(ReportTreeNode<TData> child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public ReportTreeNode<TData> Add(TData child) => AddChildren(child)[0];

        public IList<ReportTreeNode<TData>> AddChildren(params TData[] children) => AddChildren((IEnumerable<TData>)children);

        public IList<ReportTreeNode<TData>> AddChildren(IEnumerable<TData> children)
        {
            var added = new List<ReportTreeNode<TData>>();
            if (children != null)
            {
                foreach (var kid in children)
                {
                    var tn = new ReportTreeNode<TData>(kid, this);
                    added.Add(tn);
                }
            }
            return added;
        }

        public override string ToString() => $"TreeNode: Children={this.Children.Count} Data={Data?.ToString()}";

        public ReportTreeNode(TData data, ReportTreeNode<TData> parent = null, IEnumerable<TData> children = null)
        {
            Data = data;
            if (parent != null)
            {
                Parent = parent;
                parent.Children.Add(this);
            }
            AddChildren(children);
        }

        public enum WalkOrder { Breadth, Depth }

        public void Walk(Action<ReportTreeNode<TData>, int> visit, WalkOrder order = WalkOrder.Depth) => Walk((tn, d) => { visit(tn, d); return true; }, order);

        public void Walk(Func<ReportTreeNode<TData>, int, bool> visit, WalkOrder order = WalkOrder.Depth) => Walk(visit, order, 0);

        private void Walk(Func<ReportTreeNode<TData>, int, bool> visit, WalkOrder order, int depth)
        {
            if (visit(this, depth))
            {
                switch (order)
                {
                    case WalkOrder.Breadth:
                        var entrances = new List<ReportTreeNode<TData>>(Children.Count);
                        foreach (var kid in Children)
                        {
                            if (visit(kid, depth + 1))
                            {
                                entrances.Add(kid);
                            }
                        }
                        entrances.ForEach(e => e.Walk(visit, order, depth + 1));
                        break;
                    case WalkOrder.Depth:
                        foreach (var kid in Children)
                        {
                            kid.Walk(visit, order, depth + 1);
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        public static IDictionary<K, ReportTreeNode<TData>> Flatten<K>(IEnumerable<ReportTreeNode<TData>> level, Func<ReportTreeNode<TData>, K> getKey)
        {
            var d = new Dictionary<K, ReportTreeNode<TData>>();
            foreach (var z in level)
            {
                d[getKey(z)] = z;
                z.Walk(delegate (ReportTreeNode<TData> parent, ReportTreeNode<TData> item, int depth)
                {
                    d[getKey(item)] = item;
                    return true;
                });
            }
            return d;
        }

        public void Walk(Func<ReportTreeNode<TData>, ReportTreeNode<TData>, int, bool> pre, Action<ReportTreeNode<TData>, ReportTreeNode<TData>, int> post = null, Comparison<TData> walkOrder = null)
        {
            Walk(pre, null, 0, post, walkOrder);
        }

        private void Walk(Func<ReportTreeNode<TData>, ReportTreeNode<TData>, int, bool> pre, ReportTreeNode<TData> parent, int depth, Action<ReportTreeNode<TData>, ReportTreeNode<TData>, int> post = null, Comparison<TData> walkOrder = null)
        {
            if (pre == null || pre(parent, this, depth))
            {
                var kids = this.Children.ToList();
                if (walkOrder != null)
                {
                    kids.Sort((a, b) => walkOrder(a.Data, b.Data));
                }
                foreach (var kid in kids)
                {
                    kid.Walk(pre, this, depth + 1, post, walkOrder);
                }
                post?.Invoke(parent, this, depth);
            }
        }
    }
}
