using System;
using System.Collections;
using System.Collections.Generic;

public interface IContainer<T> {
    T Remove(T item);
    void Add(T item);
    IEnumerable<T> GetItems();
}
