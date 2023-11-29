using System.Collections.Generic;

namespace MyCandidate.MVVM.Services;

public interface IDictionaryService<T>
{
    IEnumerable<T> ItemsList { get; }
    bool Create(IEnumerable<T> items, out string message);
    bool Update(IEnumerable<T> items, out string message);
    bool Delete(IEnumerable<int> itemIds, out string message);
}
