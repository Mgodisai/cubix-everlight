using Data.Specifications;
using DataContextLib.Models;

namespace ClientWebPortal.Service.Specifications
{
    public class EmployeeWithPositionSpecification : BaseSpecification<Employee>
    {
        public EmployeeWithPositionSpecification()
        {
            AddInclude(e => e.Position);
        }
    }

}
