using System.Data;

namespace cake_shop_back_end.Helpers;

public interface ICommonFunction
{
    public string ComputeSha256Hash(string rawData);
    public DateTime ConvertStringFullToDate(string stringDate);
    public DateTime ConvertStringSortToDate(string stringDate);
    public string ConvertDateToStringSort(DateTime? dateObject);
    public string ConvertDateToStringFull(DateTime? dateObject);
    public string ReplaceRandomStringTo(string replaceString);
    public DataTable ExcuteQuery(string query);
    public DataTable ExcuteQueryGetTeam(decimal customer_id);
}
