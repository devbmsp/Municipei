using Municipei.Models;
namespace Municipei.Models
{
    public class AdminViewModel
    {
        public HomeModel? UsuarioLogado { get; set; }
        public List<HomeModel>? Usuarios { get; set; }
    }
}
