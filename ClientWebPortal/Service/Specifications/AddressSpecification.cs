using DataContextLib.Models;
using DataContextLib.Specifications;

namespace ClientWebPortal.Service.Specifications;

public class AddressSpecification : BaseSpecification<Address>
{
    public AddressSpecification(Address address)
    {
        AddCriteria(a=>a.PostalCode==address.PostalCode && a.Street == address.Street);
    }
}