namespace ServiceConsole.Specifications;

internal class FaultReportByDistrictSpecification : FaultReportsNotCompletedWithAddressSpecification
{
    public FaultReportByDistrictSpecification(string input) 
        : base()
    {
        AddCriteria(fr=> fr.Address != null && fr.Address.PostalCode != null && fr.Address.PostalCode.StartsWith(input));

        AddOrderBy(fr => (fr.Address != null && fr.Address.PostalCode != null) ? fr.Address.PostalCode : "");

    }
}
