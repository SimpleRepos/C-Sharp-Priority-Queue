using System;
using System.Collections.Generic;

//Return true  if (a.priority >  b.priority)
//Return false if (a.priority == b.priority)
//Return false if (a.priority <  b.priority)
//If equality does not return false then StablePriorityQueue will not sort correctly.
public delegate bool PriorityQueueDelegate<T>(T a, T b);

class PriorityQueue<T> {
    public PriorityQueue(PriorityQueueDelegate<T> isGreaterPriority) {
        Initialize(isGreaterPriority);
    }

    public PriorityQueue(PriorityQueueDelegate<T> isGreaterPriority, T[] ary) {
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

    #region Non-Public Members
    PriorityQueueDelegate<T> IsGreater;

    bool IsGreaterByIndex(int a, int b) {
        return IsGreater(list[a], list[b]);
    }

    int GreaterChildIndex(int index) {
        int left = leftChild(index);
        int right = rightChild(index);

        if (!hasChildren(index)) { throw new IndexOutOfRangeException(); }
        if (right == Count)  { return left; }

        if (IsGreaterByIndex(left, right)) { return left; }
        return right;
    }
        
    List<T> list;

    void Initialize(PriorityQueueDelegate<T> isGreaterPriority) {
        IsGreater = isGreaterPriority;
        list = new List<T>();
    }

    static bool isRoot(int index) { return index == 0; }
    static int parent(int index) { return (index - 1) / 2; }
    static int leftChild(int index)   { return (index * 2) + 1; }
    static int rightChild(int index)  { return (index * 2) + 2; }

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

class StablePriorityQueue<T> {
    public StablePriorityQueue(PriorityQueueDelegate<T> isGreaterPriority) {
        Initialize(isGreaterPriority);
    }

    public StablePriorityQueue(PriorityQueueDelegate<T> isGreaterPriority, T[] ary) {
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

    #region Non-Public Memebers
    int insertionCount = 0;

    struct Node<NT> {
        public NT Value;
        public int InsertionOrder;

        public Node(NT value, int order) {
            Value = value;
            InsertionOrder = order;
        }
    }

    PriorityQueue<Node<T>> pq;

    PriorityQueueDelegate<T> isGreater;

    bool NodeIsGreater(Node<T> a, Node<T> b) {
        if (isGreater(b.Value, a.Value)) { return false; }
        if (isGreater(a.Value, b.Value)) { return true; }
        return a.InsertionOrder < b.InsertionOrder;
    }

    void Initialize(PriorityQueueDelegate<T> isGreaterPriority) {
        isGreater = isGreaterPriority;
        pq = new PriorityQueue<Node<T>>(NodeIsGreater);
    }
    #endregion
}

class TestThing {
    public string Name;
    public int Value;

    public TestThing(string name, int value) {
        Name = name;
        Value = value;
    }
}

class Program {
    static void Main(string[] args) {
        List<TestThing> list = new List<TestThing> {
            new TestThing("", 1),
            new TestThing("a", 3),
            new TestThing("a", 5),
            new TestThing("a", 7),
            new TestThing("", 9),
            new TestThing("b", 3),
            new TestThing("b", 5),
            new TestThing("b", 7)
        };

        var pq = new PriorityQueue<TestThing>((a, b) => a.Value < b.Value, list.ToArray());
        while (pq.Count > 0) {
            var val = pq.Pop();
            Console.WriteLine(val.Value + val.Name);
        }

        Console.WriteLine("");

        var spq = new StablePriorityQueue<TestThing>((a, b) => a.Value < b.Value, list.ToArray());
        while (spq.Count > 0) {
            var val = spq.Pop();
            Console.WriteLine(val.Value + val.Name);
        }

        Console.ReadKey();
    }
}

