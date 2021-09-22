using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.IO;
using TextImageApp.Controllers;
using TextImageApp.Models;
using System.Threading.Tasks;
using TextImageApp;
using System.IO.Compression;
using System.Web.UI;

namespace UploadFiles.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Files = Directory.EnumerateFiles(Server.MapPath("~/uploads"));
            return View();
        }

        [HttpPost]
        public FileResult Download(List<string> files)
        {
            var archive = Server.MapPath("~/archive.zip");
            var temp = Server.MapPath("~/temp");

            // очистить все существующие архивы
            if (System.IO.File.Exists(archive))
            {
                System.IO.File.Delete(archive);
            }
            // очистить папку temp
            Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));

            // скопировать выбранные чекбоксом файлы в папку temp
            files.ForEach(f => System.IO.File.Copy(f, Path.Combine(temp, Path.GetFileName(f))));

            // создать новый архив
            ZipFile.CreateFromDirectory(temp, archive);

            return File(archive, "application/zip", "archive.zip");
        }

        // Заполнение строки в файле TEXTON.txt
        [HttpPost]
        public ActionResult Index(string textOut)
        {
            string writePath = "/TEXTON.txt";
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLineAsync(textOut);
                }  
            }
            catch 
            {
                goto ExceptRedir;
            }
            var temp = Server.MapPath("~/uploads");
            Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));
            ExceptRedir:
            return Redirect("/Home/Index");
        }

        // "упаковка" зип-архива
        [HttpPost]
        public ActionResult ZipCrutch(string ZipCrutchUpdate)
        {
            Response.Redirect("/home/index");

            return View();
        }

        public string textOnImage { get; set; }// наш будущий текст

        [HttpPost]
        public JsonResult Upload()
        {
            // извлечь строку из TEXTON.txt
            string path1 = "/TEXTON.txt";
            using (StreamReader sr = new StreamReader(path1, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    textOnImage = line;
                }
            }

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
                        TextMark.SetText(file, textOnImage, path);

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