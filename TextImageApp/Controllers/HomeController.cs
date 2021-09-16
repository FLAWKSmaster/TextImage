using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.IO;
using TextImageApp.Controllers;
using TextImageApp.Models;

namespace UploadFiles.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
            //return View(new TextModel());
        }


        //string textOnImage;


        /*[HttpPost]
        public ActionResult Index(TextModel txt, string TextOut)
        {
            txt.TextInput = txt.TextOutput;
            textOnImage = txt.TextOutput;
            return View(txt);
        }*/
        string textOnImage;

        public string txtimg(string TxtImg)
        {
            return TxtImg;
        }

        [HttpPost]
        public string Index(TextModel txt)
        {
            textOnImage = "abobo";
            txtimg(txt.TextOnImage);
            //textOnImage = txt.TextOnImage;
            //return txt.TextOnImage;
            Aboba = txt.TextOnImage;
            return Aboba;
        }

        public string Aboba { get; set; }

        

        [HttpPost]
        public JsonResult Upload()
        {

            string __filepath = Server.MapPath("~/uploads");
            int __maxSize = 2 * 1024 * 1024;    // максимальный размер файла 2 Мб
            // допустимые MIME-типы для файлов
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            var result = new Result
            {
                Files = new List<string>()
            };

            // текст картинки
            //string textOnImage = "AAAA";
            

            if (Request.Files.Count > 0)
            {
                foreach (string f in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[f];

                    // Выполнить проверки на допустимый размер файла и формат
                    if (file.ContentLength > __maxSize)
                    {
                        result.Error = "Размер файла не должен превышать 2 Мб";
                        break;
                    }
                    else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                    {
                        result.Error = "Недопустимый формат файла";
                        break;
                    }

                    // Сохранить файл и вернуть URL
                    if (Directory.Exists(__filepath))
                    {
                        Guid guid = Guid.NewGuid();
                        string path = $@"{__filepath}\{guid}.{file.FileName}";

                        // Добавить подпись для картинки и сохранить
                        TextMark.SetText(file, Aboba, path);

                        result.Files.Add($"/uploads/{guid}.{file.FileName}");
                    }
                }
            }



            return Json(result);
        }
    }

    public class Result
    {
        public string Error { get; set; }
        public List<string> Files { get; set; }
    }


}