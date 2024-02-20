using System.Collections.Generic;

namespace MyCandidate.MVVM.Services;

public interface IDictionaryService<T>
{
    IEnumerable<T> ItemsList { get; }
    T Get(int id);
    bool Create(IEnumerable<T> items, out string message);
    bool Update(IEnumerable<T> items, out string message);
    bool Delete(IEnumerable<int> itemIds, out string message);
}
