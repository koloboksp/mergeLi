using System;
using System.Collections.Generic;

public interface IFieldMergeable
{
    void StartMerge(IEnumerable<IFieldMergeable> others, Action<IFieldMergeable> onMergeComplete);
    int Points { get; }
}