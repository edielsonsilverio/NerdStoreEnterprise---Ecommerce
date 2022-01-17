namespace NSE.WebApp.MVC.Extensions;

public class SummaryViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}