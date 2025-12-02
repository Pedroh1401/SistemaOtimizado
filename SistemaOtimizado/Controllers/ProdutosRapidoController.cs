using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaOtimizado.Data;
using SistemaOtimizado.Models;
using System.Diagnostics;

namespace SistemaOtimizado.Controllers
{
    public class ProdutosRapidoController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutosRapidoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(bool iniciarTeste = false, int pagina = 1)
        {
            if (!iniciarTeste) return View(new List<Produto>());

            var sw = new Stopwatch();
            sw.Start();

            int tamanhoPagina = 100;

            var query = _context.Produtos
                .AsNoTracking()
                .Include(p => p.Categoria);

            var totalItens = query.Count();

            var produtos = query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToList();

            sw.Stop();

            ViewBag.Tempo = sw.ElapsedMilliseconds + " ms";
            ViewBag.Tipo = "Versão Otimizada (Com Paginação)";
            ViewBag.TesteExecutado = true;
            ViewBag.PaginaAtual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalItens / tamanhoPagina);

            return View(produtos);
        }

        [HttpPost]
        public IActionResult GerarDadosMassivos()
        {
            if (_context.Produtos.Any()) return RedirectToAction("Index");

            var categorias = new List<Categoria>();
            for (int i = 1; i <= 50; i++) categorias.Add(new Categoria { Nome = $"Categoria {i}" });
            _context.Categorias.AddRange(categorias);
            _context.SaveChanges();

            var produtos = new List<Produto>();
            var random = new Random();
            var listaCats = _context.Categorias.ToList();

            for (int i = 1; i <= 2000; i++)
            {
                produtos.Add(new Produto
                {
                    Nome = $"Produto {i}",
                    Preco = random.Next(10, 500),
                    CategoriaId = listaCats[random.Next(listaCats.Count)].Id
                });
            }
            _context.Produtos.AddRange(produtos);
            _context.SaveChanges();

            TempData["Mensagem"] = "Dados gerados!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LimparDados()
        {
            _context.Produtos.RemoveRange(_context.Produtos);
            _context.Categorias.RemoveRange(_context.Categorias);
            _context.SaveChanges();

            TempData["Mensagem"] = "Dados limpos!";
            return RedirectToAction("Index");
        }
    }
}