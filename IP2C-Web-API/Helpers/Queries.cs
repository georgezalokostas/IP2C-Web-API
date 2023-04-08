namespace IP2C_Web_API.Helpers;

public class Queries
{
    public const string GetReportAll = @" SELECT CO.Name CountryName, COUNT(*) AddressesCount, MAX(IP.UpdatedAt) LastAddressUpdated
                                          FROM IPAddresses IP INNER JOIN COUNTRIES CO ON IP.CountryId = CO.Id
                                          GROUP BY CO.Name";

    public const string GetReportById = @"  SELECT CO.Name CountryName, COUNT(*) AddressesCount, MAX(IP.UpdatedAt) LastAddressUpdated
                                            FROM IPAddresses IP INNER JOIN COUNTRIES CO ON IP.CountryId = CO.Id
                                            WHERE CO.TwoLetterCode IN @codes
                                            GROUP BY CO.Name";
}
