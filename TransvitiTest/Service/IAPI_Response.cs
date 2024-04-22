using Microsoft.AspNetCore.Mvc;
using TransvitiTest.Models;

namespace TransvitiTest.Service
{
    public interface IAPI_Response
    {
        Task<List<Products>> GetProducts(string url, int page = 1, int pageSize = 10);
        Task<Products> GetSingleProduct(string url);
        Task<Products> PostProduct(string url, Products product);
    }
}
