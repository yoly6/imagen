using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prueb.Data;
using prueb.Models;


namespace prueb.Controllers
{
    public class ImageController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

        public ImageController(ApplicationDbContext context,IWebHostEnvironment hostEnvironment)
    {
        _context = context;
            this._hostEnvironment = hostEnvironment;
        }

    // GET: Image
    public async Task<IActionResult> Index()
    { 
        return View(await _context.Images.ToListAsync()); 
    }

    // GET: Image/Create
    public IActionResult Create()
    { 
        return View();
    }

   [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ImageId,Title,ImageFile")] ImageModel imageModel)
    {
        if (ModelState.IsValid)
        {
            //Save image to wwwroot/image
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
            string extension = Path.GetExtension(imageModel.ImageFile.FileName);
            imageModel.ImageName=fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/Image/", fileName);
            using (var fileStream = new FileStream(path,FileMode.Create))
            {
                await imageModel.ImageFile.CopyToAsync(fileStream);
            }
            //Insert record
            _context.Add(imageModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(imageModel);
    }

    // GET: Image/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }
        var imageModel = await _context.Images
            .FirstOrDefaultAsync(m => m.ImageId == id);
            if (imageModel == null)
            {
               return NotFound();
            }

            return View(imageModel);
    }

    // POST: Image/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
     public async Task<IActionResult> DeleteConfirmed(int id)
    {
    var imageModel = await _context.Images.FindAsync(id);

    //delete image from wwwroot/image
    var imagePath = Path.Combine(_hostEnvironment.WebRootPath,"image",imageModel.ImageName);
    if (System.IO.File.Exists(imagePath))
        System.IO.File.Delete(imagePath);
    //delete the record
    _context.Images.Remove(imageModel);
    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
    }

    private bool ImageModelExists(int id)
    {
        return _context.Images.Any(e => e.ImageId == id);
    }
  }
}

