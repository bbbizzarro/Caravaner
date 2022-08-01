using System;
using System.Collections;
using System.Collections.Generic;

public interface IContainer<T> {
    IEnumerable<T> GetItems();
}
