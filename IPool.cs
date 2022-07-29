using System.Collections;
using System.Collections.Generic;

public interface IPool<T> {
	T Get();
	void Clear();
	void Add(T item);
	IEnumerable<T> GetActive();
}
