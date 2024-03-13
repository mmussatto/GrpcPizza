using PizzaOrdering;
using PizzaOrdering.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterServices();

var app = builder.Build();

app.RegisterMiddlewares();

app.RegisterOrderingEndpoints();

app.Run();
