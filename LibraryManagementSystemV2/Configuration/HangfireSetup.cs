using Hangfire;
using LibraryManagementSystemV2.Scheduled_Tasks;
using LibraryManagementSystemV2.Services;
using LibraryManagementSystemV2.Services.GenericServiceMappings;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystemV2.Configuration
{
    public static class HangfireSetup
    {
        public static void ConfigureHangfire(WebApplicationBuilder builder, GenerateOverdueRentalsReport reporter)
        {
            builder.Services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                config.UseSimpleAssemblyNameTypeSerializer();
                config.UseDefaultTypeSerializer();
                config.UseInMemoryStorage();
            });

            builder.Services.AddHangfireServer();
        }

        //public static void AddHangfireDashboard(WebApplication app)
        //{
        //    app.UseHangfireDashboard();

        //    RecurringJob.AddOrUpdate(
        //        "check-overdue-rentals",
        //        () => reporter.GenerateReport(),
        //        Cron.Minutely);
        //}
    }
}
