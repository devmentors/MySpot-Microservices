using Micro.Abstractions;

namespace Micro.Pagination;

public interface IPagedQuery<T> : IQuery<T>
{
    int Page { get; set; }
    int Results { get; set; }
}