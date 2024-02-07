using DataContextLib.Models;
using DataContextLib.Specifications;

namespace RepairOperationService.Specifications;

internal class RepairTypeByNameSpec : BaseSpecification<RepairOperationType>
{
    public RepairTypeByNameSpec(string rtName)
    {
        AddCriteria(rt=>rt.Name == rtName );
    }
}