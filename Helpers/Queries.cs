namespace IP2C_Web_API.Helpers;

public class Queries
{
public const string GetReport = @"SELECT CO.Name CountryName, COUNT(*) AddressesCount, MAX(IP.UpdatedAt)
                                  FROM IPAddresses IP INNER JOIN COUNTRIES CO ON IP.CountryId = CO.Id
                                  GROUP BY CO.Name
                                  ORDER BY 2 DESC";
}
