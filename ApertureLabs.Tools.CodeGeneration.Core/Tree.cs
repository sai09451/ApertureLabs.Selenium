using System;
using System.Collections;
using System.Collections.Generic;

namespace ApertureLabs.Tools.CodeGeneration.Core
{
    public class NTree<T> : IEnumerable<T>
    {
        private readonly LinkedList<NTree<T>> children;

        public NTree(T data)
        {
            Value = data;
            children = new LinkedList<NTree<T>>();
        }

        public T Value { get; }

        public void AddChild(T data)
        {
            children.AddFirst(new NTree<T>(data));
        }

        public NTree<T> GetChild(int i)
        {
            foreach (NTree<T> n in children)
                if (--i == 0)
                    return n;
            return null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var childNodes = new List<T>();
            Traverse(value => childNodes.Add(value));

            return childNodes.GetEnumerator();
        }

        public void Traverse(Action<T> visitor)
        {
            visitor(Value);
            foreach (var node in children)
                Traverse(node, visitor);
        }

        private void Traverse(NTree<T> node, Action<T> visitor)
        {
            visitor(Value);
            foreach (var kid in node.children)
                Traverse(kid, visitor);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
