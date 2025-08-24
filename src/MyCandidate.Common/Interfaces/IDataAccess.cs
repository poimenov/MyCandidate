namespace MyCandidate.Common.Interfaces;

public interface IDataAccess<T>
{
    Task CreateAsync(IEnumerable<T> items);
    Task UpdateAsync(IEnumerable<T> items);
    Task DeleteAsync(IEnumerable<int> itemIds);
    Task<T?> GetAsync(int itemId);
    Task<IEnumerable<T>> GetItemsListAsync();
    Task<bool> AnyAsync();
}
