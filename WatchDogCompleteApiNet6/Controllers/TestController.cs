using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WatchDog;
using WatchDogCompleteApiNet6.Models;

namespace WatchDogCompleteApiNet6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly ILogger _logger;
        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }
        [HttpGet("testGet")]
        public Product TestGet(string reference)
        {
            _logger.LogInformation("Omo");
            WatchLogger.Log("...TestGet Started...");
            // Some lines of code
            WatchLogger.Log("...TestGet Ended...", eventId: reference);
            return new Product { Id = 1, Name = "Get Test Product", Description = $"This is the response from testGet - {reference}", IsOnSale = true };
            throw new Exception("O get o, then forget");

        }

        [HttpPost("testPost")]
        public Product TestPost([FromBody] Product product)
        {
            WatchLogger.Log($"Create new product");
            WatchLogger.Log($"New product created with ID: {product.Id}, Name: {product.Name}, Description: {product.Description}");
            return product;
            throw new ArgumentException("Mumu, pass the right argument");
        }

        [HttpPut("testPut")]
        public string TestPut(Product product)
        {
            throw new NotImplementedException("Ask yourself, did you implement this?");
            return $"Put action for {product.Name} successful!";
        }

        [HttpPatch("testPatch")]
        public JsonResult TestPatch([Required] int id, string name)
        {
            throw new AccessViolationException("That one there was a violation, personally i wouldn't have it");
            var product = new Product { Id = id, Name = name, Description = $"This is the response from testPatch", IsOnSale = false };
            return Json(new { Code = "00", Message = $"Product {id} patched successfully with name change {name}", product });
        }

        [HttpDelete("testDelete")]
        public string TestDelete(int id)
        {
            return $"Product with ID: {id} deleted successfully";
            throw new NullReferenceException("Pass the id oponu");
        }

        [HttpPost("testFile")]
        public string TestFile([FromForm] FileTest model)
        {
            WatchLogger.LogError("File Upload Error");
            return $"File with id {model.Id} created successfully"; 
        }


        [HttpGet("simulate400")]
        public IActionResult Simulate400()
        {
            return BadRequest();
        }
    }
}
