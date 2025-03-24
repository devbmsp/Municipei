using Municipei.Models;

namespace Municipei.Interface
{
public interface ISessao
    {
        void RemoverSessaoUsuario();
        void CriarSessaoDoUsuario(HomeModel usuario);
        HomeModel BuscarSessaoDoUsuario();

    }
}