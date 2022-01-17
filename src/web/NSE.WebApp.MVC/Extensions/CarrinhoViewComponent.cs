using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;

namespace NSE.WebApp.MVC.Extensions;

public class CarrinhoViewComponent : ViewComponent
{
    private readonly IComprasBffService _carrinhoService;

    public CarrinhoViewComponent(IComprasBffService carrinhoService)
    {
        _carrinhoService = carrinhoService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var carrinho = await _carrinhoService.ObterQuantidadeCarrinho();
        return View(carrinho);
    }
}