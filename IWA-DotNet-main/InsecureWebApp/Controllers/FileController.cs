using Microsoft.AspNetCore.Mvc;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Tar;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MicroFocus.InsecureWebApp.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private const string PRESCRIPTION_LOCATION = "Files\\Prescriptions\\";

        [HttpPost("UploadFile")]
public async Task<IActionResult> UploadFile(IFormFile file, string zipFileName, string targetDir = "")
{
    string baseDir = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Prescriptions");
    if (!Directory.Exists(baseDir))
    {
        return BadRequest("Base directory does not exist.");
    }

    if (string.IsNullOrEmpty(zipFileName) || !Regex.IsMatch(zipFileName, @"^[a-zA-Z0-9]+\.(zip)$"))
    {
        return BadRequest("Invalid file name.");
    }

    string zipFilePath = Path.Combine(baseDir, zipFileName);
    using (var stream = new FileStream(zipFilePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    if (string.IsNullOrEmpty(targetDir))
    {
        targetDir = baseDir;
    }

    string targetPath = Path.GetFullPath(Path.Combine(baseDir, targetDir));
    if (!targetPath.StartsWith(baseDir))
    {
        return BadRequest("Invalid target directory.");
    }

    FastZip fastZip = new FastZip();
    fastZip.ExtractZip(zipFilePath, targetPath, null);

    return Ok("File extracted at : " + targetPath);
}
    }
}
