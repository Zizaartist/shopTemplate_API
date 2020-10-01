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
    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _appEnvironment;
        ClickContext _context = new ClickContext();
        public ImagesController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        // POST: api/Users
        [Route("api/PostImage")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<List<ImageCl>>> PostImage(IFormFileCollection uploadedFiles)
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

            List<ImageCl> imageRegisters = new List<ImageCl>();

            foreach (IFormFile uploadedFile in uploadedFiles)
            {
                // путь к папке Files, ЗАМЕНИТЬ Path.GetTempFileName на более надежный генератор
                string path = Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + Path.GetExtension(uploadedFile.FileName);

                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + "/Images/" + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                // создаем энтити image для учета файла, чтобы срач с блоба быстрее убирать
                var imageEntity = new ImageCl()
                {
                    UserId = identityToUser(User.Identity).UserId,
                    Path = path
                };
                _context.ImageCl.Add(imageEntity);
                imageRegisters.Add(imageEntity);
                await _context.SaveChangesAsync();
            }
            return imageRegisters; 
        }

        [Route("api/AddFile")]
        [HttpPost]
        //api/<FilesController>
        public async Task<ActionResult<List<ImageCl>>> AddFile(Stream uploadedFile)
        {
            List<ImageCl> imageRegisters = new List<ImageCl>();
            if (uploadedFile != null)
            {
                string path = Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".png";
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + "/Images/" + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                // создаем энтити image для учета файла, чтобы срач с блоба быстрее убирать
                var imageEntity = new ImageCl()
                {
                    UserId = identityToUser(User.Identity).UserId,
                    Path = path
                };
                _context.ImageCl.Add(imageEntity);
                imageRegisters.Add(imageEntity);
                await _context.SaveChangesAsync();
            }
            return imageRegisters;
        }

        private UserCl identityToUser(IIdentity identity)
        {
            return _context.UserCl.FirstOrDefault(u => u.Phone == identity.Name);
        }
    }
}
