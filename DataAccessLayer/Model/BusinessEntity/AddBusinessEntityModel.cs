using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
namespace DataAccessLayer.Model.BusinessEntity;

public class AddBusinessEntityModel
{
    public required string BusinessEntityCode { get; set; }

    public string? BusinessEntityDesc { get; set; }

    public required IFormFile LogoFile { get; set; }

    [JsonIgnore]
    public string Logo { get; set; }

    public required string Address1 { get; set; }

    public string? Address2 { get; set; }

    public required string PostalCode { get; set; }

    public required Guid CountryGUID { get; set; }

    public required string PrimaryContactName { get; set; }

    public required string PrimaryContactDesignation { get; set; }

    public required string PrimaryContactNumber { get; set; }

    public required string PrimaryContactEmail { get; set; }

    public string? SecondaryContactName { get; set; }

    public string? SecondaryContactDesignation { get; set; }

    public string? SecondaryContactNumber { get; set; }

    public string? SecondaryContactEmail { get; set; }

    public required string BillingAddress1 { get; set; }

    public string? BillingAddress2 { get; set; }

    public required string BillingPostalCode { get; set; }

    public required Guid BillingCountryGUID { get; set; }

    public required Guid TimeZoneGUID { get; set; }

    public required Guid CurrencyGUID { get; set; }

    public decimal? LatitudeCoordinate { get; set; }

    public decimal? LongitudeCoordinate { get; set; }

    public decimal? Radius { get; set; }

    public string? Account_Code { get; set; }

    public string? Integration_Code { get; set; }

    public bool? IsChild { get; set; }

    public Guid? ParentBusinessEntityGUID { get; set; }

    public Guid? EntityGroupGUID { get; set; }

    public bool? Active { get; set; }

    public required string CreatedBy { get; set; }
}


