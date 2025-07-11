using System.Linq.Expressions;

namespace TaskManager.Domain.Common;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    bool IsPagingEnabled { get; }
    int Take { get; }
    int Skip { get; }
}
