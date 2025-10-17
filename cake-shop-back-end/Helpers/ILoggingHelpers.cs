using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.Helpers;

public interface ILoggingHelpers
{
    Task InsertLogging(LoggingRequest loggingRequest);
}
