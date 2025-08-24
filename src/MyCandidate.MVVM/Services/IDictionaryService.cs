using System.Collections.Generic;
using System.Threading.Tasks;
using MyCandidate.Common;

namespace MyCandidate.MVVM.Services;

public interface IDictionaryService<T>
{
    Task<IEnumerable<T>> GetItemsListAsync();
    Task<T?> GetAsync(int id);
    Task<OperationResult> CreateAsync(IEnumerable<T> items);
    Task<OperationResult> UpdateAsync(IEnumerable<T> items);
    Task<OperationResult> DeleteAsync(IEnumerable<int> itemIds);
    Task<bool> AnyAsync();
}
