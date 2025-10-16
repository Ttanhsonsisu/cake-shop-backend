using System.Data;

namespace cake_shop_back_end.Helpers;

public class CommonFunction(IConfiguration configuration) : ICommonFunction
{
    public string ComputeSha256Hash(string rawData)
    {
        throw new NotImplementedException();
    }

    public string ConvertDateToStringFull(DateTime? dateObject)
    {
        throw new NotImplementedException();
    }

    public string ConvertDateToStringSort(DateTime? dateObject)
    {
        throw new NotImplementedException();
    }

    public DateTime ConvertStringFullToDate(string stringDate)
    {
        throw new NotImplementedException();
    }

    public DateTime ConvertStringSortToDate(string stringDate)
    {
        throw new NotImplementedException();
    }

    public DataTable ExcuteQuery(string query)
    {
        throw new NotImplementedException();
    }

    public DataTable ExcuteQueryGetTeam(decimal customer_id)
    {
        throw new NotImplementedException();
    }

    public string ReplaceRandomStringTo(string replaceString)
    {
        throw new NotImplementedException();
    }
}
