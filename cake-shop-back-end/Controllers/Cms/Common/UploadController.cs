using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cake_shop_back_end.Controllers.Cms.Common;

public class ImageBase64Upload
{
    public string? ImageData { get; set; }
}
public class FileDetail
{
    public string? Name { get; set; }
    public string? Url { get; set; }
}

[Route("api/upload")]
[ApiController]
public class UploadController(IConfiguration _config) : ControllerBase
{
    [Route("uploadfile")]
    [HttpPost]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        if (files != null && files.Count() > 0)
        {
            long size = files.Sum(f => f.Length);
            List<FileDetail> filePaths = new List<FileDetail>();
            int i = 0;
            string url = _config.GetSection("config")["Url"];
            string uploadFolder = "";
            uploadFolder = _config.GetSection("config")["Directory"] + "\\cakeshop";
            foreach (var formFile in files)
            {
                i += 1;
                if (formFile.Length > 0)
                {
                    try
                    {
                        //name folder
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        //name file
                        //var uniqueFileName = Strings.RemoveDiacriticUrls(Path.GetFileNameWithoutExtension(formFile.FileName).Replace(" ", "")) + "_" + DateTime.Now.ToString("ssmmhhddMMyyyy") + i.ToString() + Path.GetExtension(formFile.FileName);
                        var uniqueFileName = "FileCakeShop" + "_" + DateTime.Now.ToString("ssmmhhddMMyyyy") + i.ToString() + Path.GetExtension(formFile.FileName);
                        //path
                        var filePath = Path.Combine(uploadFolder, uniqueFileName);
                        if (!String.IsNullOrEmpty("cakeshop"))
                        {
                            var x = new FileDetail();
                            x.Name = formFile.FileName;
                            x.Url = url + "cakeshop" + "/" + uniqueFileName;
                            filePaths.Add(x);
                        }
                        else
                        {
                            var x = new FileDetail();
                            x.Name = formFile.FileName;
                            x.Url = url + uniqueFileName;
                            filePaths.Add(x);
                        }

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new JsonResult(new { status = 400, code = "ERROR", data = ex.Message });
                    }

                }
            }
            return new JsonResult(new { status = 200, code = "OK", data = filePaths });
        }
        else
        {
            return new JsonResult(new { status = 400, code = "EMPTY_FILE" });
        }

    }

    [Route("uploadImageBase64")]
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> uploadImageBase64(ImageBase64Upload request)
    {
        if (request.ImageData == null || request.ImageData.Length == 0)
        {
            return new JsonResult(new { status = 400, code = "ERROR_IMAGE_DATA_MISSING", data = "" });
        }

        FileDetail fileDetail = new FileDetail();
        string url = _config.GetSection("config")["Url"];
        string uploadFolder = "";
        uploadFolder = _config.GetSection("config")["Directory"] + "\\cakeshop";
        Byte[] imageBytes = Convert.FromBase64String(request.ImageData);
        try
        {
            //name folder
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            //name file
            var uniqueFileName = "SIGN_IMAGE" + "_" + DateTime.Now.ToString("ssmmhhddMMyyyy") + ".png";
            //path
            var filePath = Path.Combine(uploadFolder, uniqueFileName);
            if (!String.IsNullOrEmpty("cakeshop"))
            {
                var x = new FileDetail();
                x.Name = uniqueFileName;
                x.Url = url + "cakeshop" + "/" + uniqueFileName;
                fileDetail = x;
            }
            else
            {
                var x = new FileDetail();
                x.Name = uniqueFileName;
                x.Url = url + uniqueFileName;
                fileDetail = x;
            }
            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
        }
        catch (Exception ex)
        {
            return new JsonResult(new { status = 400, code = "ERROR", data = ex.Message });
        }
        return new JsonResult(new { status = 200, code = "OK", data = fileDetail });

    }
}
