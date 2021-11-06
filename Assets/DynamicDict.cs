using System.Collections.Generic;
using System.Linq;

public class DynamicDict<T, S> : Dictionary<T, S> {
    public DynamicDict() : base() { }
    public DynamicDict(int capacity) : base(capacity) { }

    public new S this[T key] {
        get {
            if (!ContainsKey(key)) {
                Add(key, default(S));
            }
            return base[key];
        }
        set {
            if (!ContainsKey(key)) {
                Add(key, default(S));
            } else {
                base[key] = value;
            }
        }
    }

    public override string ToString()
    {
        if (Count == 0) return "{}";
        var elements = string.Join(", ", this.Select(kvp => $"[{kvp.Key}, {kvp.Value}]"));
        return $"{{ {elements} }}";
    }
}