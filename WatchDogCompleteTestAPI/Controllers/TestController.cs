using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using WatchDogCompleteTestAPI.Models;

namespace WatchDogCompleteTestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : Controller
    {
        [HttpGet("testGet")]
        public Product TestGet(string reference)
        {
            return new Product { Id = 1, Name = "Get Test Product", Description = $"This is the response from testGet - {reference}", IsOnSale = true };
            throw new Exception("O get o, then forget");
        }

        [HttpPost("testPost")]
        public Product TestPost([FromBody] Product product)
        {
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
            return Json(new { Code = "00", Message = $"Product {id} patched successfully with name change {name}", product});
        }

        [HttpDelete("testDelete")]
        public string TestDelete(int id)
        {
            return $"Product with ID: {id} deleted successfully";
            throw new NullReferenceException("Pass the id oponu");
        }
    }
}
