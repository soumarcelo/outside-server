using System.Text.Json;

namespace OutsideServer.HTTPClients;

public class GeocodingAPI(IHttpClientFactory httpClientFactory, ILogger<GeocodingAPI> logger)
{
    private const string KEY = "";

    public async Task<GeocodingResponse?> GetAddressInfo(string address)
    {
        using HttpClient client = httpClientFactory.CreateClient();

        try
        {
            address = address.Replace(" ", "+");
            string uri = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={KEY}";
            GeocodingResponse? response = 
                await client.GetFromJsonAsync<GeocodingResponse>(
                    uri,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError("[GeocodingAPI] Error: {Error}", ex);
            throw;
        }
    }
}

public class GeocodingResponse
{
    public class AddressData
    {
        public class AddressComponent
        {
            public string long_name { get; set; } = string.Empty;
            public string short_name { get; set; } = string.Empty;
            public List<string> types { get; set; } = [];
        }

        public class AddressGeometry
        {
            public class GeometryPoint
            {
                public double lat { get; set; } = 0d;
                public double lng { get; set; } = 0d;
            }

            public class GeometryViewport
            {
                public GeometryPoint northeast { get; set; } = new GeometryPoint();
                public GeometryPoint sorthwest { get; set; } = new GeometryPoint();
            }

            public GeometryPoint location { get; set; } = new GeometryPoint();
            public string location_type { get; set; } = string.Empty;
            public GeometryViewport viewport { get; set; } = new GeometryViewport();
        }

        public class AddressPlusCode
        {
            public string compound_code { get; set; } = string.Empty;
            public string global_code { get; set; } = string.Empty;
        }

        public List<AddressComponent> address_components { get; set; } = [];
        public string formatted_address {  get; set; } = string.Empty;
        public AddressGeometry geometry { get; set; } = new AddressGeometry();
        public string place_id {  get; set; } = string.Empty;
        public AddressPlusCode plus_code { get; set; } = new AddressPlusCode();
        
        // https://developers.google.com/maps/documentation/geocoding/requests-geocoding?hl=pt-br#Types
        public List<string> types { get; set; } = []; 
    }

    public List<AddressData> results { get; set; } = [];

    /// <summary>
    /// Valids: OK | ZERO_RESULTS | OVER_QUERY_LIMIT | REQUEST_DENIED | INVALID_REQUEST | UNKNOWN_ERROR
    /// </summary>
    public string status { get; set; } = string.Empty;

    public string? error_message { get; set; } = null;
}
