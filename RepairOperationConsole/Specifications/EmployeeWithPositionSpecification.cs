using DataContextLib.Models;
using DataContextLib.Specifications;

namespace ClientWebPortal.Service.Specifications;

public class EmployeeWithPositionSpecification : BaseSpecification<Employee>
{
    public EmployeeWithPositionSpecification()
    {
        AddInclude(e => e.Position);
    }
}