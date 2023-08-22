using Microsoft.AspNetCore.Mvc;
using RedisWithNet.Data;
using RedisWithNet.Models;
using RedisWithNet.Services.CacheService;

namespace RedisWithNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly ICacheService cacheService;
        private readonly string key = "product";

        public ProductController(DataContext dataContext, ICacheService cacheService)
        {
            this.dataContext = dataContext;
            this.cacheService = cacheService;
        }

        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            var cacheData = cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData is not null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            cacheData = dataContext.Products.ToList();
            cacheService.SetData<IEnumerable<Product>>("product", cacheData, expirationTime);
            return cacheData;
        }

        [HttpGet("{id}")]
        public Product Get(int id)
        {
            Product filteredData;
            var cacheData = cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData is not null)
            {
                filteredData = cacheData.Where(p => p.Id == id).FirstOrDefault();
                return filteredData;
            }
            filteredData = dataContext.Products.Where(p => p.Id == id).FirstOrDefault();
            return filteredData;
        }

        [HttpPost]
        public async Task<Product> Add(Product newProduct)
        {
            var obj = await dataContext.Products.AddAsync(newProduct);
            cacheService.RemoveData(key);
            dataContext.SaveChanges();
            return obj.Entity;
        }

        [HttpPut]
        public void Update(Product product)
        {
            dataContext.Products.Update(product);
            cacheService.RemoveData(key);
            dataContext.SaveChanges();
        }

        [HttpDelete]
        public void Delete(int id)
        {
            var product = dataContext.Products.FirstOrDefault(p => p.Id == id);
            dataContext.Products.Remove(product);
            cacheService.RemoveData(key);
            dataContext.SaveChanges();
        }
    }
}
