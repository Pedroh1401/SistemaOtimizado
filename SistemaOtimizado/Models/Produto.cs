using SistemaOtimizado.Models;

namespace SistemaOtimizado.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }

        // Chaves para o Banco
        public int CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }
    }
}