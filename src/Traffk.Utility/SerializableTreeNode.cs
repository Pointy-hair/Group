using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Traffk.Utility
{
    public class SerializableTreeNode<TData>
    {
        public TData Data { get; private set; }

        public SerializableTreeNode<TData> Parent { get; set; }

        public SerializableTreeNode<TData>[] ChildrenArray
        {
            get => Children.ToArray();
            set => Children = value.ToList();
        }

        [JsonIgnore]
        public IList<SerializableTreeNode<TData>> Children { get; set; } = new List<SerializableTreeNode<TData>>();

        public bool HasChildren => Children.Count > 0;

        public SerializableTreeNode<TData> this[int index] => Children[index];

        public void Add(SerializableTreeNode<TData> child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public SerializableTreeNode<TData> Add(TData child) => AddChildren(child)[0];

        public IList<SerializableTreeNode<TData>> AddChildren(params TData[] children) => AddChildren((IEnumerable<TData>)children);

        public IList<SerializableTreeNode<TData>> AddChildren(IEnumerable<TData> children)
        {
            var added = new List<SerializableTreeNode<TData>>();
            if (children != null)
            {
                foreach (var kid in children)
                {
                    var tn = new SerializableTreeNode<TData>(kid, this);
                    added.Add(tn);
                }
            }
            return added;
        }

        public override string ToString() => $"TreeNode: Children={this.Children.Count} Data={Data?.ToString()}";

        public SerializableTreeNode(TData data, SerializableTreeNode<TData> parent = null, IEnumerable<TData> children = null)
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

        public void Walk(Action<SerializableTreeNode<TData>, int> visit, WalkOrder order = WalkOrder.Depth) => Walk((tn, d) => { visit(tn, d); return true; }, order);

        public void Walk(Func<SerializableTreeNode<TData>, int, bool> visit, WalkOrder order = WalkOrder.Depth) => Walk(visit, order, 0);

        private void Walk(Func<SerializableTreeNode<TData>, int, bool> visit, WalkOrder order, int depth)
        {
            if (visit(this, depth))
            {
                switch (order)
                {
                    case WalkOrder.Breadth:
                        var entrances = new List<SerializableTreeNode<TData>>(Children.Count);
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

        public static IDictionary<K, SerializableTreeNode<TData>> Flatten<K>(IEnumerable<SerializableTreeNode<TData>> level, Func<SerializableTreeNode<TData>, K> getKey)
        {
            var d = new Dictionary<K, SerializableTreeNode<TData>>();
            foreach (var z in level)
            {
                d[getKey(z)] = z;
                z.Walk(delegate (SerializableTreeNode<TData> parent, SerializableTreeNode<TData> item, int depth)
                {
                    d[getKey(item)] = item;
                    return true;
                });
            }
            return d;
        }

        public void Walk(Func<SerializableTreeNode<TData>, SerializableTreeNode<TData>, int, bool> pre, Action<SerializableTreeNode<TData>, SerializableTreeNode<TData>, int> post = null, Comparison<TData> walkOrder = null)
        {
            Walk(pre, null, 0, post, walkOrder);
        }

        private void Walk(Func<SerializableTreeNode<TData>, SerializableTreeNode<TData>, int, bool> pre, SerializableTreeNode<TData> parent, int depth, Action<SerializableTreeNode<TData>, SerializableTreeNode<TData>, int> post = null, Comparison<TData> walkOrder = null)
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
