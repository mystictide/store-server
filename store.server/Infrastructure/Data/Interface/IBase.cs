using store.server.Infrasructure.Models.Helpers;

namespace store.server.Infrastructure.Data.Interface
{
    public interface IBase<T> where T : class
    {
        Task<FilteredList<T>?> FilteredList(Filter filter);
        Task<T?> Get(int? ID, string? Name);
        Task<T?> Manage(T entity);
        Task<bool> Archive(T entity);
    }
}
