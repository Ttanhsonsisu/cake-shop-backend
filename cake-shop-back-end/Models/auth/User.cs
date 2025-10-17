using cake_shop_back_end.Models.Common;

namespace cake_shop_back_end.Models.auth;

public class User : MasterCommonModel
{
    public Guid id { get; set; }

    public string? code { get; set; }

    public string username { get; set; } = null!;

    public string? full_name { get; set; }

    public DateTime? birth_date { get; set; }

    public int? gender { get; set; }

    public string? avatar { get; set; }

    public string? description { get; set; }

    public string? phone { get; set; }

    public string? email { get; set; }

    public string? address { get; set; }

    public string password { get; set; } = null!;

    public Guid? user_group_id { get; set; }

    public int? status { get; set; }

    public bool? is_sysadmin { get; set; }

    public bool? is_admin { get; set; }

    public bool? is_admin_store { get; set; }

    public Guid? store_id { get; set; }

    public string? secret_key { get; set; }

    public string? device_id { get; set; }

    public bool? is_delete { get; set; }

}
