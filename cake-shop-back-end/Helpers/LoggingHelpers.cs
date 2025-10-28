using cake_shop_back_end.Data;
using cake_shop_back_end.DataObjects.Requests.Common;
using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Helpers;

public class LoggingHelpers(AppDbContext _context) : ILoggingHelpers
{
    public Task InsertLogging(LoggingRequest loggingRequest)
    {
        try
        {
            var logging = new Logging();
            logging.user_type = loggingRequest.UserType;
            logging.api_name = loggingRequest.ApiName;
            logging.application = loggingRequest.Application;
            logging.functions = loggingRequest.Functions;
            logging.actions = loggingRequest.Actions;
            logging.IP = loggingRequest.IP;
            logging.content = loggingRequest.Content;
            logging.result_logging = loggingRequest.ResultLogging;
            logging.is_login = loggingRequest.IsLogin;
            logging.is_call_api = loggingRequest.IsCallApi;
            logging.user_created = loggingRequest.UserCreated;
            logging.date_created = DateTime.Now;

            _context.Loggings.Add(logging);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {

        }

        return Task.CompletedTask;
    }
}
