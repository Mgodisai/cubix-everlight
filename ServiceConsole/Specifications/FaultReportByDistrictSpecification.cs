namespace ServiceConsole.Specifications
{
    internal class FaultReportByDistrictSpecification : FaultReportsNotCompletedWithAddressSpecification
    {
        public FaultReportByDistrictSpecification(string input) 
            : base()
        {
            AddCriteria(fr=> fr.Address.PostalCode.StartsWith(input));
        }
    }
}
