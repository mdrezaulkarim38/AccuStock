using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers;

public class ProductController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}