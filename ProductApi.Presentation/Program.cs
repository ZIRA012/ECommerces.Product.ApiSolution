using ProductApi.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureService(builder.Configuration); 
var app = builder.Build();


app.UseInfrastucturePolicy();  //Agregamos Las polices de seguridad en este caso solo los middlwware
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection(); //fuerza el uso de HTTPS 
app.UseAuthorization();   //Habilita los middlewares
app.MapControllers();       //Hace el enrutado que los controladores dirgieron
app.Run();