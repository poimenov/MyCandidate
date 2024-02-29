namespace MyCandidate.Common.Interfaces;

public interface IDataAccess<T>
{
    void Create(IEnumerable<T> items);
    void Update(IEnumerable<T> items);
    void Delete(IEnumerable<int> itemIds);
    T? Get(int itemId);
    IEnumerable<T> ItemsList { get; }
    bool Any();
}
