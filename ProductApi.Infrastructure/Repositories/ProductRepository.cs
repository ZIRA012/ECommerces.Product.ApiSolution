using System.Linq.Expressions;
using ECommmerce.SharedLibrary.Logs;
using ECommmerce.SharedLibrary.Responses;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


//esta se encarga de guardar en la base de datos cuando recibimos un pedido, en todo caso
//Es un itermediario entre la base de datos y nuestras peticiones
namespace ProductApi.Infrastructure.Repositories
{
    //el tipo de entrada de dato es Genereic, se especifica que al menos sea una clase
    public class ProductRepository(ProductDbContext context) : IProduct // Iproduct herada de IGENERIC la iterfaz en la que esta
    {

        
        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)//buscamos por algun atributo del objeto
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                //Log la excepcion original
                LogException.LogExceptions(ex);

                // Mostrar un mensaje libre de miedo
                throw new InvalidOperationException("Error al buscar el Item");
            }
        }

        //CREATE
        public async Task<Response> CreateAsync(Product entity)
        {
            
            

            try
            {
                // El producto ya existe entonces no lo agregamos
                var getProduct = await GetByAsync (_=> _.Name!.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} ya esta agregado"); // ya esta agregado
                
                //El producto no existe
                var currentEntity = context.Products.Add(entity).Entity;// Agregamos el producto a la base de datos
                await context.SaveChangesAsync();

                //Devolvemos el resultado si se añadio el produc o esto solo verifica si el producto ya se añadio a la base de datos
                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"{entity.Name} Fue añadido a la base de datos");
                else
                    return new Response(false, $"Ecurrio un error al agregar el producto{entity.Name}");

            }
            catch (Exception ex) { 
            //Log la excepcion original
            LogException.LogExceptions(ex);

                // Mostrar un mensaje libre de miedo
                return new Response(false, "Erro al agregar el Item en creando");
            }
        }


        //READ, read by id or read all Items
        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {   
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null! ;
            }
            catch (Exception ex)
            {
                //Log la excepcion original
                LogException.LogExceptions(ex);

                // Mostrar un mensaje libre de miedo
                throw new Exception("Error al buscar el Item");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                //Log la excepcion original
                LogException.LogExceptions(ex);

                // Mostrar un mensaje libre de miedo
                throw new InvalidOperationException("Error al agregar el Item en conseguir todos");
            }
        }

        //UPDATE with id
        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {

                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} no se encontro");

                context.Entry(product!).State = EntityState.Detached;
                context.Products.Update(entity);

                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} Actualizado correctamente");
            }
            catch (Exception ex)
            {
                //Log la excepcion original
                LogException.LogExceptions(ex);

                // Mostrar un mensaje libre de miedo
                return new Response(false ,"Erro al actualizar el Item");
            }
        }

        //DELETE with id
        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {

                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} no existe");



                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} Fue eliminado");
            }
            catch (Exception ex)
            {
                //Log la excepcion original
                LogException.LogExceptions(ex);

                // Mostrar un mensaje libre de miedo
                return new Response(false, "Error al eliminar el Item");
            }
        }
    }
}
