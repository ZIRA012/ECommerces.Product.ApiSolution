using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommmerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

namespace UnitTest.ProductApi.Controllers;

public class ProductControllerTest
{
    private readonly IProduct productInterface;
    private readonly ProductsController productsController;


    public ProductControllerTest()
    {
       //Dependencias
        productInterface = A.Fake<IProduct>();

        //Set Up System Under Test -SUT

        productsController = new ProductsController(productInterface);
    }
    //GET ALL Products

    [Fact]
    public async Task GetProduct_WhenProductExists_ReturnOKResponseWithProducts()
    {
        var products = new List<Product>()
        {
            new(){Id=1, Name="Product 1", Quantity=10, Price=700.0m},
            new(){Id=2, Name="Product 2", Quantity=10, Price=700.0m},
            
        };

        //set up fake Response for GetAllsync
        A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

        //act
        var result = await productsController.GetProducts();

        //var
        var okResult = result.Result as OkObjectResult;
        okResult!.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var ReturnedProducts = okResult.Value as IEnumerable<ProductDTO>;
        ReturnedProducts.Should().NotBeNull();
        ReturnedProducts.Should().HaveCount(2);
        ReturnedProducts.First().Id.Should().Be(1);
        ReturnedProducts.Last().Id.Should().Be(2);

    }
    [Fact]
    public async Task GetProduct_WhenProductNotExists_ReturnNotFound()
    {
        var products = new List<Product>();



        A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

        var result = await productsController.GetProducts();

        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var message = notFoundResult.Value as string;
        message.Should().Be("sin productos en la base de datos");
    }


    //////////CREATE PRODUCT
    [Fact]
    public async Task CreatProduct_WhenModelStateIsInvalid_ReturnBadRequest()
    {
        var productDTO = new ProductDTO(1, "Product 1", 15, 95.0m);


        productsController.ModelState.AddModelError("Name", "Required");

        //ACT

        var result = await productsController.CreateProduct(productDTO);

        var badRequestResult = result.Result as BadRequestObjectResult;

        badRequestResult!.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
    [Fact]
    public async Task CreatProduct_WhenCreateSuccesfully_ReturnOkResponse()
    {
        //ARRANGE
        var productDTO = new ProductDTO(1, "Product 1", 15, 95.0m);
        var response = new Response(true, "Creado");

        A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);

        //ACT 
        var result = await productsController.CreateProduct(productDTO);
        //Resultados
        var okResult = result.Result as OkObjectResult;
        okResult!.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);


        var responseResult = okResult.Value as Response;

        responseResult!.Message.Should().Be("Creado");
        responseResult!.Flag.Should().BeTrue(); 
    }

    [Fact]
    public async Task  CreateProduct_WhenCreateFailed_ReturnBadRequest()
    {
        var productDTO = new ProductDTO(1, "Product 1", 15, 95.0m);
        var response = new Response(false, "Error al crear");

        A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);

        //ACT
        var result = await productsController.CreateProduct(productDTO);
        var okResult = result.Result as BadRequestObjectResult;

        okResult!.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);


        var responseResult = okResult.Value as Response;

        responseResult!.Message.Should().Be("Error al crear");
        responseResult!.Flag.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateProduct_WhenUpdateSuccesfully_ReturnOk()
    {
        var productDTO = new ProductDTO(1, "Product 1", 15, 95.0m);
        var response = new Response(true, "Producto Actualizado");

        A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);

        //ACT
        var result = await productsController.UpdateProduct(productDTO);
        var okResult = result.Result as OkObjectResult;

        okResult!.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);


        var responseResult = okResult.Value as Response;

        responseResult!.Message.Should().Be("Producto Actualizado");
        responseResult!.Flag.Should().BeTrue();
    }


    [Fact]
    public async Task UpdateProduct_WhenUpdateFailed_ReturnBadRequest()
    {
        var productDTO = new ProductDTO(1, "Product 1", 15, 95.0m);
        var response = new Response(false, "Error al Actualizar");

        A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);

        //ACT
        var result = await productsController.UpdateProduct(productDTO);
        var okResult = result.Result as BadRequestObjectResult;



        //VERIFICAMOS
        okResult!.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);


        var responseResult = okResult.Value as Response;

        responseResult!.Message.Should().Be("Error al Actualizar");
        responseResult!.Flag.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteProduct_WhenDeleteSuccesfully_ReturnOkResponse()
    {
        //ARRANGE
        var productDTO = new ProductDTO(1, "Product 1", 15, 95.0m);
        var response = new Response(true, "Eliminado");

        A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

        //ACT 
        var result = await productsController.DeleteProduct(productDTO);
        //Resultados
        var okResult = result.Result as OkObjectResult;
        okResult!.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);


        var responseResult = okResult.Value as Response;

        responseResult!.Message.Should().Be("Eliminado");
        responseResult!.Flag.Should().BeTrue();
    }


    [Fact]
    public async Task DeleteProduct_WhenDeleteFailed_ReturnBadRequest()
    {
        //ARRANGE
        var productDTO = new ProductDTO(1, "Product 1", 15, 95.0m);
        var response = new Response(false, "Error al Eliminar");

        A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

        //ACT 
        var result = await productsController.DeleteProduct(productDTO);
        //Resultados
        var okResult = result.Result as BadRequestObjectResult;
        okResult!.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);


        var responseResult = okResult.Value as Response;

        responseResult!.Message.Should().Be("Error al Eliminar");
        responseResult!.Flag.Should().BeFalse();
    }
}
