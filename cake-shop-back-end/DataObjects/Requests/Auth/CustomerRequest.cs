using cake_shop_back_end.DataObjects.Requests.Common;

namespace cake_shop_back_end.DataObjects.Requests.Auth;

public class CustomerRequest : PaggingRequest
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Phone { get; set; }
    public int? Gender { get; set; }
    public int? CountryId { get; set; }
    public string? Avatar { get; set; }
    public string? Address { get; set; }
    public int? StatesId { get; set; }
    public string? ZipCode { get; set; }
    public int? Status { get; set; }
    public string? CompanyName { get; set; }
}
