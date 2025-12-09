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
        var data = _db.fillingpoints
            .OrderBy(x => x.id)
            .ToList();
        return View(data);
    }


    [HttpPost]
    public IActionResult Create(FillingPoints model)
    {
        var rawName = model.fillingbay;

        //Cek duplikat berdasarkan fillingbay saja
        if (_db.fillingpoints.Any(x => x.fillingbay.ToLower() == model.fillingbay!.ToLower()))
        {
            TempData["error"] = $"Data {model.fillingbay} Is Already Exist, Try Again!";
            return RedirectToAction("Index");
        }

        if (rawName.Contains(" "))
        {
            TempData["error"] = "Space Detected";
            return RedirectToAction(nameof(Index));
        }
        
        if (ModelState.IsValid)
        {
            _db.fillingpoints.Add(model);
            _db.SaveChanges();

            TempData["success"] = $"Data {model.fillingbay} Success Added";
            return RedirectToAction("Index");
        }

        TempData["refresh"] = true;
        return RedirectToAction("Index");
        
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, FillingPoints model)
    {
        if (id != model.id)
            return NotFound();

        if (ModelState.IsValid)
        {
            //Validasi Ambil input asli (tanpa trim)
            var rawName = model.fillingbay;

            //Validasi Cek spasi di mana pun (awal, tengah, akhir)
            if (rawName.Contains(" "))
            {
                TempData["error"] = "Space Detected";
                return RedirectToAction(nameof(Index));
            }

            //Validasi Normalisasi setelah validasi
            var newName = rawName.Trim();
            var lowerName = newName.ToLower();

            //Validasi Tidak boleh angka semua
            if (newName.All(char.IsDigit))
            {
                TempData["error"] = "You Cannot Input Only Number!";
                return RedirectToAction(nameof(Index));
            }

            //Cek duplikat (case-insensitive)
            bool exists = _db.fillingpoints
                .Any(x => x.fillingbay.Trim().ToLower() == lowerName
                    && x.id != model.id);

            if (exists)
            {
                TempData["error"] = $"Filling Bay {model.fillingbay} Is Already Exist, Try Again!";
                return RedirectToAction(nameof(Index));
            }

            //Overwrite biar spasi hilang
            model.fillingbay = newName;

            //Update
            _db.Update(model);
            await _db.SaveChangesAsync();

            TempData["success"] = "Data updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult UpdateStatus([FromBody] FillingPoints model)
    {
        var data = _db.fillingpoints.FirstOrDefault(x => x.id == model.id);
        if (data == null) return NotFound();

        data.statusfilling = model.statusfilling;
        _db.SaveChanges();

        return Ok();
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var data = _db.fillingpoints.Find(id);
        if (data == null)
            return Json(new { success = false });

        _db.fillingpoints.Remove(data);
        _db.SaveChanges();

        return Json(new { success = true });
    }

    [HttpGet]
    public IActionResult GetDashboardCounts()
    {
        var total = _db.fillingpoints.Count();
        var on = _db.fillingpoints.Count(fp => fp.statusfilling == true);
        var off = _db.fillingpoints.Count(fp => fp.statusfilling == false);

        return Json(new {
            totalPoint = total,
            statusOn = on,
            statusOff = off
        });
    }
    
}
