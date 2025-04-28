using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
namespace UnitTest.ProductApi.Repositories;

public class ProductRepositoryTest
{
    private readonly ProductDbContext productDbContext;
    private readonly ProductRepository productRepository;

    public ProductRepositoryTest()
    {
        //Simulacion en memoria de la base de datos
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: "ProductDB").Options;

        productDbContext = new ProductDbContext(options);
        productRepository = new ProductRepository(productDbContext);
    }

    //CreateProduct
    [Fact]
    public async Task CreateAsync_WhenProductAlreadyExist_ReturnError()
    {
        var existingProduct = new Product { Name = "ExistingProduct" };
        productDbContext.Products.Add(existingProduct);
        await productDbContext.SaveChangesAsync();

        //ACT
        var result = await productRepository.CreateAsync(existingProduct);

        //Verificar

        result.Should().NotBeNull();
        result.Flag.Should().BeFalse();
        result.Message.Should().Be($"{existingProduct.Name} ya esta agregado");
    }

    [Fact]
    public async Task CreateAsync_WhenProductDoesNotExist_AddProductAndReturnSuccess()
    {
        var product = new Product { Name = "Product" };
        //ACT
        var result = await productRepository.CreateAsync(product);

        //Verificar

        result.Should().NotBeNull();
        result.Flag.Should().BeTrue();
        result.Message.Should().Be($"{product.Name} Fue añadido a la base de datos");
    }

    //DELETE PRODUCT

    //When it not exits - > Error
    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_ReturnError()
    {
        var notExistingProduct = new Product { Name = "NotExistingProduct" };


        //ACT
        var result = await productRepository.DeleteAsync(notExistingProduct);

        //Verificar

        result.Should().NotBeNull();
        result.Flag.Should().BeFalse();
        result.Message.Should().Be($"{notExistingProduct.Name} no existe");
    }

    [Fact]

    public async Task DeleteAsync_WhenProductExist_ReturnSuccessfully()
    {
        var existingProduct = new Product { Name = "ExistingProduct" };
        productDbContext.Products.Add(existingProduct);
        await productDbContext.SaveChangesAsync();

        //ACT
        var result = await productRepository.DeleteAsync(existingProduct);

        //Verificar

        result.Should().NotBeNull();
        result.Flag.Should().BeTrue();
        result.Message.Should().Be($"{existingProduct.Name} Fue eliminado");
    }

    //GET PRODUCT BY ID

    [Fact]
    public async Task FindByAsync_WhereProductIsFound_ReturnProduct()
    {
        var product = new Product() { Id = 1, Name = "Product", Price = 1.1m, Quantity = 5 };
        productDbContext.Products.Add(product);
        await productDbContext.SaveChangesAsync();
        //ACT
        var result = await productRepository.FindByIdAsync(product.Id);
        //veRIfy
        result.Should().NotBeNull();
        result.Should().Be(product);
    }

    [Fact]
    public async Task FindByAsync_WhereProductIsNotFound_ReturnNull()
    {
        var product = new Product() { Id = 1, Name = "Product", Price = 1.4m, Quantity = 5 };

        //ACT
        var result = await productRepository.FindByIdAsync(product.Id);
        //veRIfy
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WhenProductsAreFound_ReturnProducts()
    {
        var products = new List<Product>
            {
            new() { Id = 1, Name = "Product 1", Price = 1.4m, Quantity = 5 },
            new() { Id = 2, Name = "Product 2", Price = 1.4m, Quantity = 5 },
            new() { Id = 3, Name = "Product 3", Price = 1.4m, Quantity = 5 },
            };
       
        
            productDbContext.Products.AddRange(products);
            await productDbContext.SaveChangesAsync();
        var result = await productRepository.GetAllAsync();
        result.Should().NotBeNull();
        result.Count().Should().Be(products.Count);
        result.Should().Contain(p => p.Name == "Product 1");
        result.Should().Contain(p => p.Name == "Product 2");
        result.Should().Contain(p => p.Name == "Product 3");



    }
}
