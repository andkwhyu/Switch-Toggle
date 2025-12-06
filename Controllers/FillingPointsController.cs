using Microsoft.AspNetCore.Mvc;

public class FillingPointsController : Controller
{
    private readonly AppDbContext _db;

    public FillingPointsController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var data = _db.fillingpoints.ToList();
        return View(data);
    }

}
