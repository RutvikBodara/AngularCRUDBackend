using BAL.Contacts.Interface;
using BAL.Contacts.Repository;
using DAL.Contacts.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ConactsDBContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("ContactsDB")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder.WithOrigins("http://localhost:4200") // Replace with your Angular app URL
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});
builder.Services.AddScoped<IBAL_Contacts_CRUD, BAL_Contacts_CRUDRepo>();
builder.Services.AddScoped<ICommonMethods,CommonMethodsRepo>();
builder.Services.AddScoped<IBAL_Contacts_Type_CRUD, BAL_Contacts_Type_CRUDRepo>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowOrigin");
app.UseAuthorization();

app.MapControllers();

app.Run();
