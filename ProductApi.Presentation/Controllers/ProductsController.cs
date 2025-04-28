using Microsoft.AspNetCore.Mvc;

using ProductApi.Application.Interfaces;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using Response = ECommmerce.SharedLibrary.Responses.Response;
using Microsoft.AspNetCore.Authorization;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("sin productos en la base de datos");

            var (_, list) = ProductConversion.FromEntity(null, products);
            return list!.Any() ? Ok(list) : NotFound("sin productos encontrados");
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int Id)
        {
            var product = await productInterface.FindByIdAsync(Id);
            if (product == null)
                return NotFound($"Producto con {Id} no encontrado");

            //Entidad a DTO conversion y return
            var (_product, _) = ProductConversion.FromEntity(product, null);
            return _product is not null? Ok(_product): NotFound("Producto no encontrado");
        }

        [HttpPost]
        //[Authorize(Roles ="Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            // ver si el modelo todas las anotacionse de data se cumplieron
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //convertir a entidad
            var getEntity = ProductConversion.ToEntity(product); // al crear el producto de manera automatica se genera en la base de datos
            var response = await productInterface.CreateAsync(getEntity);//
            return response.Flag is true? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }

}
