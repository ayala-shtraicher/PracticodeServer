using TodoApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      policy  =>
                      {
                          policy.WithOrigins("https://practicodeclient-g08q.onrender.com/").AllowAnyHeader().AllowAnyMethod();
                      });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My ToDoAPI", Version = "v1" });
});

builder.Services.AddDbContext<ToDoDbContext>();

var app = builder.Build();

app.UseCors("MyAllowSpecificOrigins");

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My ToDoAPI V1");
});

app.MapGet("/", (ToDoDbContext db) =>{
 return db.Items.ToList();
});



app.MapPost("/",async (ToDoDbContext db,[FromBody]Item item) => {
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return item;
 });

 app.MapPut("/{id}",async ([FromBody] Item item,ToDoDbContext db,int id) => {
    Item i=await db.Items.FindAsync(id);
    i.IsComplete=item.IsComplete;
    await db.SaveChangesAsync();
 });

 app.MapDelete("/{id}",async (ToDoDbContext db, int id) => {
   Item i=await db.Items.FindAsync(id);
     db.Items.Remove(i);
    await db.SaveChangesAsync();
 });

app.Run();
