using Hangfire;
using LibraryManagementSystemV2.DTOs.RentalDTOs;
using LibraryManagementSystemV2.Services.GenericServiceMappings;
using Serilog;

namespace LibraryManagementSystemV2.Scheduled_Tasks
{
    public class GenerateOverdueRentalsReport(ITestRentalMapping _testRentalMapping)
    {
        private readonly ITestRentalMapping _testRentalMapping;

        public async Task GenerateReport()
        {
            List<RentalShowDTO> overdueRentals = (await _testRentalMapping.GetOverdueRentals()).ToList();

            HashSet<Guid> previousOverdueRentalIds = GetPreviousOverdueRentalIds();
            List<Guid> newOverdueRentalIds = overdueRentals.Select(rental => rental.Id).Where(rentalId => !previousOverdueRentalIds.Contains(rentalId)).ToList();

            Log.Error(newOverdueRentalIds.ToString()); 

        }

        private HashSet<Guid> GetPreviousOverdueRentalIds()
        {
            var storage = JobStorage.Current.GetConnection();
            var jobData = storage.GetJobData("previousOverdueRentalIds");
            return new HashSet<Guid>{ };
            //return jobData?.StateData?.Data != null
            //    ? JsonConvert.DeserializeObject<HashSet<int>>(jobData.StateData.Data)
            //    : new HashSet<int>();
        }

        //private void StoreOverdueRentalIds(List<Guid> overdueRentalIds)
        //{
        //    var storage = JobStorage.Current.GetConnection();
        //    var serializedIds = JsonConvert.SerializeObject(overdueRentalIds);
        //    storage.SetJobParameter("previousOverdueRentalIds", serializedIds);
        //}
    }
}
