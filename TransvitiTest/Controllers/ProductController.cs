using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using TransvitiTest.DBFolder;
using TransvitiTest.Service;

namespace TransvitiTest.Controllers
{
    public class ProductController : Controller
    {
        private readonly EcommerceDbContext dbContext;
        private readonly IAPI_Response response;

        public ProductController(EcommerceDbContext dbContext, IAPI_Response response)
        {
            this.dbContext = dbContext;
            this.response = response;
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10)
        {
            var result = await response.GetProducts("https://fakestoreapi.com/products", page, pageSize);
            if(result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return Ok();
            }

        }

        [HttpGet("GetSingleProduct")]
        public async Task<IActionResult> GetSingleProduct()
        {
            var result = await response.GetSingleProduct("https://fakestoreapi.com/products/1");
            return Ok(result);
        }
    }
}
