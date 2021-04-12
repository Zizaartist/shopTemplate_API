using ApiClick.Controllers.FrequentlyUsed;
using ApiClick.Models;
using ApiClick.Models.EnumModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _appEnvironment;
        
        public ImagesController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        // POST: api/Images
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<string>>> PostImage(IFormFileCollection uploadedFiles)
        {
            if (uploadedFiles == null || uploadedFiles.Count < 1) 
            {
                return BadRequest();
            }

            //Проверить все ли являются изображениями и имеют реальный размер
            foreach (IFormFile uploadedFile in uploadedFiles) 
            {
                if (uploadedFile.Length <= 0) 
                {
                    return BadRequest(); //probably wrong code
                }
                if (!uploadedFile.ContentType.Contains("image")) 
                {
                    return BadRequest(); //probably wrong code    
                } 
            }

            var filePaths = new List<string>();

            foreach (IFormFile uploadedFile in uploadedFiles)
            {
                // путь к папке Files, ЗАМЕНИТЬ Path.GetTempFileName на более надежный генератор
                string path = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(uploadedFile.FileName);
                filePaths.Add(path);

                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + "/Images/" + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
            }
            return filePaths; 
        }
    }
}
