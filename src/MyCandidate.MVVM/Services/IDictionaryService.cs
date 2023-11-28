using System.Collections.Generic;

namespace MyCandidate.MVVM.Services;

public interface IDictionaryService<T>
{
    IEnumerable<T> ItemsList { get; }
    void Create(IEnumerable<T> items);
    void Update(IEnumerable<T> items);
    void Delete(IEnumerable<int> itemIds);
}
