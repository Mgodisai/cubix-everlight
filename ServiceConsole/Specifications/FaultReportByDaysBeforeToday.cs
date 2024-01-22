
namespace ServiceConsole.Specifications
{
    internal class FaultReportByDaysBeforeTodaySpecification : FaultReportsNotCompletedWithAddressSpecification
    {
        public FaultReportByDaysBeforeTodaySpecification(int input) 
            : base()
        {
            AddCriteria(fr=> fr.ReportedAt.AddDays(input).Date<DateTime.Now.Date);
        }
    }
}
