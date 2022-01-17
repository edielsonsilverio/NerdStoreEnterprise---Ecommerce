using NSE.WebApp.Blazor.Models;
using Refit;

namespace NSE.WebApp.Blazor.Services;

public interface ICatalogoService
{
    Task<IEnumerable<ProdutoViewModel>> ObterTodos();
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