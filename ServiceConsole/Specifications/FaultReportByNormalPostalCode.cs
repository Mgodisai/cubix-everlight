
namespace ServiceConsole.Specifications
{
    internal class FaultReportByNormalPostalCodeSpecification : FaultReportsNotCompletedWithAddressSpecification
    {
        public FaultReportByNormalPostalCodeSpecification(string input) 
            : base()
        {
            AddCriteria(fr => fr.Address != null && fr.Address.PostalCode != null && fr.Address.PostalCode.Equals(input));
        }
    }
}
