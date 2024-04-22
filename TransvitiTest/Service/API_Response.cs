using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using TransvitiTest.Models;

namespace TransvitiTest.Service
{
    public class API_Response : IAPI_Response
    {
        private readonly HttpClient httpClient;

        public API_Response(HttpClient httpClient) {  this.httpClient = httpClient; }

        public async Task<List<Products>> GetProducts(string url, int page = 1, int pageSize = 10)
        {
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to retrieve products. Status code: {response.StatusCode}");
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            var products = await JsonSerializer.DeserializeAsync<List<Products>>(contentStream);
            if ( products != null && products.Count > 0)
            {
                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList(); ;
            }
            else
            {
                return new List<Products>();
            }
        }

        public async Task<Products> GetSingleProduct(string url)
        {
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to retrieve products. Status code: {response.StatusCode}");
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            var products = await JsonSerializer.DeserializeAsync<Products>(contentStream);
            if (products != null)
            {
                return products;
            }
            else
            {
                return new Products();
            }

        }
    }
}
