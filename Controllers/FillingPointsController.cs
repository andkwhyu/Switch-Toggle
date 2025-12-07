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

    [HttpPost]
    public IActionResult Create(FillingPoints model)
    {
        // Cek duplikat berdasarkan fillingbay saja
        if (_db.fillingpoints.Any(x => x.fillingbay.ToLower() == model.fillingbay!.ToLower()))
        {
            TempData["error"] = $"Data {model.fillingbay} Is Already Exist, Try Again!";
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            _db.fillingpoints.Add(model);
            _db.SaveChanges();

            TempData["success"] = $"Data {model.fillingbay} Success Added";
            return RedirectToAction("Index");
        }

        TempData["error"] = "Gagal input data";
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var data = _db.fillingpoints.FirstOrDefault(x => x.id == id);

        if (data == null)
        {
            TempData["error"] = "Data Not Found!";
            return RedirectToAction("Index");
        }

        _db.fillingpoints.Remove(data);
        _db.SaveChanges();

        TempData["success"] = $"Data {data.fillingbay} Deleted";
        return RedirectToAction("Index");
    }

}
