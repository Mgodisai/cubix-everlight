using DataContextLib.Models;
using DataContextLib.Specifications;

namespace ServiceConsole.Specifications;

internal class FindEmployeeByUsernameSpecification : BaseSpecification<Employee>
{
    public FindEmployeeByUsernameSpecification(string username) {

        AddCriteria(e => e.Username.Equals(username));
    }

}