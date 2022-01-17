using NSE.WebApp.MVC.Models;
using Refit;

namespace NSE.WebApp.MVC.Services;

public interface ICatalogoService
{
    Task<PagedViewModel<ProdutoViewModel>> ObterTodos(int pageSize, int pageIndex, string query = null);
    Task<ProdutoViewModel> ObterPorId(Guid id);
}

//Interface usando o Refit, tudo será gerenciado pelo Refit
public interface ICatalogoServiceRefit
{
    [Get("/catalogo/produtos/")]
    Task<IEnumerable<ProdutoViewModel>> ObterTodos();

    [Get("/catalogo/produtos/{id}")]
    Task<ProdutoViewModel> ObterPorId(Guid id);
}