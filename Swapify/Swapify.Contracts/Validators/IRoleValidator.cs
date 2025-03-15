using Swapify.Contracts.Models;
using Swapify.Contracts.Transactions;

namespace Swapify.Contracts.Validators;

public interface IRoleValidator
{
    Task ValidateRoleUpdatesAsync(IRole role, IAtomicScope atomicScope);
    Task ValidateRoleCreatesAsync(string roleName, IAtomicScope atomicScope);
}