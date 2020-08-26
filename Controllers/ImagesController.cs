using ApiClick.Models;
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
        public async Task<ActionResult<ImageCl>> PostImage(IFormFile uploadedFile)
        {
            // путь к папке Files, потом заменю на генератор имен
            string path = uploadedFile.FileName;
            // сохраняем файл в папку Files в каталоге wwwroot
            using (var fileStream = new FileStream(_appEnvironment.WebRootPath + "/Images/" + path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }
            // создаем энтити image для учета файла, чтобы срач с блоба быстрее убирать
            var imageEntity = new ImageCl()
            {
                UserId = identityToUser(User.Identity).UserId,
                path = path
            };
            _context.ImageCls.Add(imageEntity);
            await _context.SaveChangesAsync();

            return imageEntity; 
        }
        private UserCl identityToUser(IIdentity identity)
        {
            return _context.UserCl.FirstOrDefault(u => u.Phone == identity.Name);
        }
    }
}
