# Product API

Este proyecto forma parte del backend de un sistema de **E-Commerce**.

La **Product API** permite realizar operaciones CRUD (Crear, Leer, Actualizar y Eliminar) sobre los productos disponibles.

## Tecnologías utilizadas

- ASP.NET Core Web API
- Entity Framework Core
- Clean Architecture (Separación de capas: Entities, DTOs, Repository, Application)
- SQL Server (para persistencia de datos)
- JWT Bearer Authentication (validación de acceso)
- Unit Testing con xUnit y FakeItEasy (pruebas con dependencias mockeadas)
- Ocelot API Gateway (para enrutamiento general de APIs en el proyecto completo)

## Funcionalidades principales

- Obtener todos los productos
- Obtener un producto por ID
- Crear un nuevo producto
- Actualizar un producto existente
- Eliminar un producto
- Validaciones y control de errores integrados
- Autenticación mediante tokens JWT

## Cómo ejecutar

