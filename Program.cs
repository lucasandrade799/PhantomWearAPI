using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Porta para o Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();

// Banco de dados em memória
var produtos = new List<Produto> {
    new Produto { Id = 1, Nome = "Camiseta Branca - Gorillaz", Cat = "Camiseta", Preco = 89.90m, Estoque = 20, Img = "camiseta_demondays.png" },
    new Produto { Id = 2, Nome = "Moletom Branco - Gojo", Cat = "Moletom", Preco = 189.90m, Estoque = 3, Img = "moletom_gojo.png" },
    new Produto { Id = 3, Nome = "Moletom Preto - Gemaplys", Cat = "Moletom", Preco = 199.90m, Estoque = 15, Img = "moletom_chicken.png" },
    new Produto { Id = 4, Nome = "Moletom Branco - Yunli", Cat = "Moletom", Preco = 179.90m, Estoque = 7, Img = "moletom_yunli.png" },
    new Produto { Id = 5, Nome = "Moletom Preto - Miles Morales", Cat = "Moletom", Preco = 210.00m, Estoque = 10, Img = "moletom_spider.png" },
    new Produto { Id = 6, Nome = "Moletom Preto - Saiko", Cat = "Moletom", Preco = 195.00m, Estoque = 2, Img = "moletom_saiko.png" },
    new Produto { Id = 7, Nome = "Camiseta Preta - JF Classics", Cat = "Camiseta", Preco = 109.90m, Estoque = 30, Img = "camiseta_jf.png" },
    new Produto { Id = 8, Nome = "Camiseta Branca - Choso", Cat = "Camiseta", Preco = 99.90m, Estoque = 12, Img = "camiseta_choso.png" },
    new Produto { Id = 9, Nome = "Camiseta Preta - Toji", Cat = "Camiseta", Preco = 119.90m, Estoque = 4, Img = "camiseta_toji.png" },
    new Produto { Id = 10, Nome = "Camiseta Branca - Korn", Cat = "Camiseta", Preco = 89.90m, Estoque = 18, Img = "camiseta_korn.png" }
};

var pedidos = new List<Pedido>();

// --- ROTAS API ---

app.MapGet("/api/produtos", () => produtos);
app.MapGet("/api/pedidos", () => pedidos);

// Cliente cria o pedido (Status inicial: Pendente)
app.MapPost("/api/pedidos", (Pedido novoPedido) => {
    novoPedido.Id = pedidos.Count + 1001;
    novoPedido.Status = "PENDENTE";
    pedidos.Add(novoPedido);
    return Results.Ok(novoPedido);
});

// Admin envia para entrega e dá baixa no estoque
app.MapPost("/api/pedidos/{id}/entregar", (int id) => {
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    if (pedido == null || pedido.Status != "PENDENTE") return Results.BadRequest("Pedido não encontrado ou já enviado.");

    foreach (var item in pedido.Itens) {
        var p = produtos.FirstOrDefault(x => x.Id == item.Id);
        if (p != null) p.Estoque = Math.Max(0, p.Estoque - 1);
    }

    pedido.Status = "ENVIADO";
    return Results.Ok(new { mensagem = "Estoque atualizado e pedido enviado!", pedido });
});

// Atualização manual do estoque (Botão Salvar)
app.MapPatch("/api/produtos/{id}", (int id, [FromBody] int novaQtd) => {
    var p = produtos.FirstOrDefault(x => x.Id == id);
    if (p == null) return Results.NotFound();
    p.Estoque = novaQtd;
    return Results.Ok(p);
});

app.Run();

public class Produto {
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Cat { get; set; } = "";
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public string Img { get; set; } = "";
}

public class Pedido {
    public int Id { get; set; }
    public List<Produto> Itens { get; set; } = new();
    public string Endereco { get; set; } = "";
    public decimal Total { get; set; }
    public string Status { get; set; } = "PENDENTE";
}
