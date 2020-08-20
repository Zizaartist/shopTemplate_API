using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    public class ImagesController : ControllerBase
    {
        ClickContext _context = new ClickContext();

        // POST: api/Users
        [Route("api/PostImage")]
        [HttpPost]
        public async Task<IActionResult> PostImage(IFormFileCollection uploads)
        {
            foreach (var uploadedFile in uploads)
            {
                // путь к папке Files
                string path = "/Images/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path };
                _context.Files.Add(file);
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
