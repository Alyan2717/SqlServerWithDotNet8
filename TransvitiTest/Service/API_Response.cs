using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Json;
using TransvitiTest.DBFolder;
using TransvitiTest.Models;

namespace TransvitiTest.Service
{
    public class API_Response : IAPI_Response
    {
        private readonly HttpClient httpClient;
        private readonly EcommerceDbContext dbContext;

        public API_Response(HttpClient httpClient, EcommerceDbContext dbContext) 
        {  
            this.httpClient = httpClient;
            this.dbContext = dbContext;
        }

        public async Task<List<Products>> GetProducts(string url, int page = 1, int pageSize = 10)
        {
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to retrieve products. Status code: {response.StatusCode}");
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            var products = await JsonSerializer.DeserializeAsync<List<Products>>(contentStream);

            var productsFromDb = dbContext.Products.OrderBy(p => p.Id)
                                                   .Skip((page - 1) * pageSize)
                                                   .Take(pageSize)
                                                   .ToListAsync();

            // --From stored procedure
            var productsFromSP = await dbContext.Products.FromSqlRaw("EXEC GetProducts {0}, {1}", page, pageSize).ToListAsync();

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

        public async Task<Products> PostProduct(string url, Products product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var productJson = JsonSerializer.Serialize(product);
            var httpContent = new StringContent(productJson, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to retrieve products. Status code: {response.StatusCode}");
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            var products = await JsonSerializer.DeserializeAsync<Products>(contentStream);

            //-- From Db Add product
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            //-- From stored procedure
            var productsFromSP = await dbContext.Products.FromSqlRaw("EXEC PostProduct {0}, {1}, {2}, {3}", product.Title, product?.Price, product.Description, product.Category).ToListAsync();

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
