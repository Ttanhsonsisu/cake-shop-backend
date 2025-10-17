namespace cake_shop_back_end.DataObjects.Responses;

public class APIResponse
{
    public string Code { get; set; } 
    public string? Error { get; set; } = string.Empty;
    public Object? Data { get; set; }

    /// <summary>
    /// Return Error Status Code & Error Code.
    /// </summary>
    public APIResponse(int status)
    {
        switch (status)
        {
            case 200:
                this.Code = "200";
                break;
            case 400:
                this.Error = "BAD_REQUEST";
                this.Code = "400";
                break;
            case 404:
                this.Error = "NO_Data_FOUND";
                this.Code = "400";
                break;
            default:
                this.Code = "400";
                break;
        }
    }

    public APIResponse(object Data)
    {
        this.Code = "200";
        this.Data = Data;
        this.Error = null;
    }

    public APIResponse(int status, string Data)
    {
        this.Code = status.ToString();
        this.Data = Data;
        this.Error = null;
    }

    public APIResponse(int status, object Data)
    {
        this.Code = status.ToString();
        this.Data = Data;
        this.Error = null;
    }

    public APIResponse(string Error)
    {
        this.Code = "400";
        this.Error = Error;
        this.Data = null;
    }

}
