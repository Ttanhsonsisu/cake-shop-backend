namespace cake_shop_back_end.Models.Common;

public class Logging
{
    public Guid id { get; set; }

    public string user_type { get; set; } = null!;

    public string? application { get; set; }

    public string? functions { get; set; }

    public string? actions { get; set; }

    public string? IP { get; set; }

    public string? content { get; set; }

    public string? result_logging { get; set; }

    public bool? is_login { get; set; }

    public bool? is_call_api { get; set; }

    public string? user_created { get; set; }

    public DateTime? date_created { get; set; }

    public string? api_name { get; set; }

}
