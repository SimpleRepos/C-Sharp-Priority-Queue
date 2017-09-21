using System;
using System.Collections.Generic;

namespace PriorityQueue {
    public interface PQueue<T> {
        int Count { get; }
        void Push(T element);
        T Pop();
        void Remove(T element);
    }

    //Return true  if (a.priority >  b.priority)
    //Return false if (a.priority == b.priority)
    //Return false if (a.priority <  b.priority)
    //If equality does not return false then StablePriorityQueue will not sort correctly.
    public delegate bool PriorityIsGreater<T>(T a, T b);
    
    class Queue<T> : PQueue<T> {
        public Queue(PriorityIsGreater<T> isGreaterPriority) {
            Initialize(isGreaterPriority);
        }

        public Queue(PriorityIsGreater<T> isGreaterPriority, T[] ary) {
            Initialize(isGreaterPriority);
            foreach (T element in ary) { Push(element); }
        }

        public int Count {
            get { return list.Count; }
        }

        public void Push(T element) {
            int index = Count;
            list.Add(element);

            while (!isRoot(index) && IsGreater(list[index], list[parent(index)])) {
                swapElements(index, parent(index));
                index = parent(index);
            }
        }

        public T Pop() {
            if (Count == 0) {
                throw new InvalidOperationException("Tried to pop an empty PriorityQueue.");
            }

            T retval = list[0];

            int victimIndex = Count - 1;
            if (victimIndex > 0) { list[0] = list[victimIndex]; }
            list.RemoveAt(victimIndex);

            int index = 0;
            while (hasChildren(index)) {
                int gci = GreaterChildIndex(index);
                if (IsGreaterByIndex(index, gci)) { break; }
                swapElements(index, gci);
                index = gci;
            }

            return retval;
        }

        public void Remove(T element) {
            int index = list.FindIndex((x) => EqualityComparer<T>.Default.Equals(x, element));
            if (index == -1) { return; }
            while (!isRoot(index)) {
                swapElements(index, parent(index));
                index = parent(index);
            }
            Pop();
        }

        #region Non-Public Members
        PriorityIsGreater<T> IsGreater;

        bool IsGreaterByIndex(int a, int b) {
            return IsGreater(list[a], list[b]);
        }

        int GreaterChildIndex(int index) {
            int left = leftChild(index);
            int right = rightChild(index);

            if (!hasChildren(index)) { throw new IndexOutOfRangeException(); }
            if (right == Count) { return left; }

            if (IsGreaterByIndex(left, right)) { return left; }
            return right;
        }

        List<T> list;

        void Initialize(PriorityIsGreater<T> isGreaterPriority) {
            IsGreater = isGreaterPriority;
            list = new List<T>();
        }

        static bool isRoot(int index) { return index == 0; }
        static int parent(int index) { return (index - 1) / 2; }
        static int leftChild(int index) { return (index * 2) + 1; }
        static int rightChild(int index) { return (index * 2) + 2; }

        bool hasChildren(int index) {
            return leftChild(index) < Count;
        }

        void swapElements(int firstIndex, int secondIndex) {
            T temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }
        #endregion
    }

    class StableQueue<T> : PQueue<T> {
        public StableQueue(PriorityIsGreater<T> isGreaterPriority) {
            Initialize(isGreaterPriority);
        }

        public StableQueue(PriorityIsGreater<T> isGreaterPriority, T[] ary) {
            Initialize(isGreaterPriority);
            foreach (T element in ary) { Push(element); }
        }

        public int Count {
            get { return pq.Count; }
        }

        public void Push(T element) {
            pq.Push(new Node<T>(element, insertionCount++));
        }

        public T Pop() {
            return pq.Pop().Value;
        }

        public void Remove(T element) {
            pq.Remove(new Node<T>(element, -1));
        }

        #region Non-Public Memebers
        int insertionCount = 0;

        struct Node<NT> : IEquatable<Node<NT>> {
            public NT Value;
            public int InsertionOrder;

            public Node(NT value, int order) {
                Value = value;
                InsertionOrder = order;
            }

            public bool Equals(Node<NT> other) {
                return EqualityComparer<NT>.Default.Equals(Value, other.Value);
            }
        }

        Queue<Node<T>> pq;

        PriorityIsGreater<T> isGreater;

        bool NodeIsGreater(Node<T> a, Node<T> b) {
            if (isGreater(b.Value, a.Value)) { return false; }
            if (isGreater(a.Value, b.Value)) { return true; }
            return a.InsertionOrder < b.InsertionOrder;
        }

        void Initialize(PriorityIsGreater<T> isGreaterPriority) {
            isGreater = isGreaterPriority;
            pq = new Queue<Node<T>>(NodeIsGreater);
        }
        #endregion
    }

}

struct TestThing {
    public string Name;
    public int Value;

    public TestThing(string name, int value) {
        Name = name;
        Value = value;
    }

    public override string ToString() {
        return Value.ToString() + Name;
    }
}

class Program {
    static readonly string HR = "------------------------";

    static void TestFunc(PriorityQueue.PQueue<TestThing> pq) {
        while (pq.Count > 5) { Console.Write(pq.Pop() + ", "); }
        Console.WriteLine(pq.Pop());

        pq.Push(new TestThing("", 8));
        pq.Push(new TestThing("", 2));
        pq.Remove(new TestThing("a", 7));
        pq.Push(new TestThing("", 6));
        pq.Push(new TestThing("", 4));

        while (pq.Count > 1) { Console.Write(pq.Pop() + ", "); }
        Console.WriteLine(pq.Pop());
        Console.WriteLine(HR);
    }

    static void Main(string[] args) {
        List<TestThing> list = new List<TestThing> {
            new TestThing("", 1),
            new TestThing("a", 3),
            new TestThing("a", 5),
            new TestThing("a", 7),
            new TestThing("a", 7),
            new TestThing("", 9),
            new TestThing("b", 3),
            new TestThing("b", 5),
            new TestThing("b", 7),
            new TestThing("c", 7)
        };

        PriorityQueue.PriorityIsGreater<TestThing> isGreater = (a, b) => a.Value < b.Value;

        PriorityQueue.PQueue<TestThing> pq = new PriorityQueue.Queue<TestThing>(isGreater, list.ToArray());
        TestFunc(pq);
        pq = new PriorityQueue.StableQueue<TestThing>(isGreater, list.ToArray());
        TestFunc(pq);

        List<int> vals = new List<int> { 1, 5, 2, 3, 5, 4, 3, 2, 4 };
        PriorityQueue.PQueue<int> queue = new PriorityQueue.Queue<int>((a, b) => a > b, vals.ToArray());
        queue.Remove(7);
        queue.Remove(3);
        while (queue.Count > 1) { Console.Write(queue.Pop() + ", "); }
        Console.WriteLine(queue.Pop());
        Console.WriteLine(HR);

        queue = new PriorityQueue.StableQueue<int>((a, b) => a > b, vals.ToArray());
        queue.Remove(7);
        queue.Remove(3);
        while (queue.Count > 1) { Console.Write(queue.Pop() + ", "); }
        Console.WriteLine(queue.Pop());
        Console.WriteLine(HR);

        Console.ReadKey();
    }
}

