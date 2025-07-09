using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Specifications;

public class UserByEmailSpecification : BaseSpecification<User>
{
	public UserByEmailSpecification(string email)
		: base(user => user.Email.ToLower() == email.ToLower() && !user.IsDeleted)
	{
	}
}

public class ActiveUsersSpecification : BaseSpecification<User>
{
	public ActiveUsersSpecification()
		: base(user => user.IsActive && !user.IsDeleted)
	{
		ApplyOrderBy(user => user.FirstName);
	}
}

public class UserSearchSpecification : BaseSpecification<User>
{
	public UserSearchSpecification(string searchTerm)
		: base(user => (user.FirstName.Contains(searchTerm) ||
						user.LastName.Contains(searchTerm) ||
						user.Email.Contains(searchTerm)) &&
					   !user.IsDeleted)
	{
		ApplyOrderBy(user => user.FirstName);
	}
}
