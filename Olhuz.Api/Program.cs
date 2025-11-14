// --------------------------------------------------------------------------------
// [INICIALIZAÇÃO DA APLICAÇÃO] Program.cs
// --------------------------------------------------------------------------------

using Olhuz.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o serviço para reconhecer os Controladores
builder.Services.AddControllers();

// Adiciona os serviços do Swagger/OpenAPI da forma correta para Controladores
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adiciona o repositório 'UserRepository' no container de serviços para injeção de dependência (Dependency Injection - DI) por HTTP request
// O 'AddScoped' cria uma nova instância do repositório para cada requisição HTTP
// Assim cada requisição terá sua própria instância/objeto do 'UserRepository'
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 3. Habilita o middleware do Swagger e da interface do Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisições HTTP para HTTPS
app.UseHttpsRedirection();

// Habilita o middleware de autenticação para proteger as rotas da API
app.UseAuthorization();

// Mapeia as rotas definidas nos seus arquivos de Controller (como o AccountController)
app.MapControllers();

// Inicia a aplicação web API e começa a escutar requisições HTTP
app.Run();