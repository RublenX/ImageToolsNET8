using ImageToolsWebApiNet8.DTOs;
using IronSoftware.Drawing;
using Microsoft.AspNetCore.Mvc;

namespace ImageToolsWebApiNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessImageIronDrawingController : ControllerBase
    {
        #region Endpoints Públicos
        [HttpPost("ConvertoToBmpBytes")]
        public IActionResult ConvertoToBmpBytes([FromForm] FileUploadApi objImages)
        {
            try
            {
                if (objImages == null || objImages.Images == null)
                {
                    throw new ArgumentException($"El parámetro {nameof(objImages)} viene a nulo o no contiene ninguna imagen");
                }

                AnyBitmap bitmap = AnyBitmap.FromStream(objImages.Images.OpenReadStream());
                byte[] byteArray = bitmap.ExportBytes(AnyBitmap.ImageFormat.Bmp);
                return File(byteArray, "image/bmp", $"imagen{DateTime.Now.ToString("yyMMddHHmmss")}.bmp");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ConvertoToBmpStream")]
        public IActionResult ConvertoToBmpStream([FromForm] FileUploadApi objImages)
        {
            try
            {
                if (objImages == null || objImages.Images == null)
                {
                    throw new ArgumentException($"El parámetro {nameof(objImages)} viene a nulo o no contiene ninguna imagen");
                }

                var memoryStream = ConvertToBmpStream(objImages.Images.OpenReadStream());
                return File(memoryStream, "image/bmp", $"imagen{DateTime.Now.ToString("yyMMddHHmmss")}.bmp");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GenerateJpgRandom")]
        public IActionResult GenerateJpgRandom(int width = 80, int height = 24)
        {
            try
            {
                byte[] byteArray = GenerateRandomLinealGradientImage(width, height);
                return File(byteArray, "image/jpg", $"imagen{DateTime.Now.ToString("yyMMddHHmmss")}.jpg");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 
        #endregion

        #region Métodos Auxiliares
        private byte[] GenerateRandomLinealGradientImage(int width, int height)
        {
            // Crear un objeto AnyBitmap de 80x24 píxeles
            using var bitmap = new AnyBitmap(width, height);

            // Generar colores aleatorios para el degradado
            var random = new Random();
            var color1R = random.Next(256);
            var color1G = random.Next(256);
            var color1B = random.Next(256);
            var color2R = random.Next(256);
            var color2G = random.Next(256);
            var color2B = random.Next(256);

            var saltoColorR = (decimal)(color2R - color1R) / width;
            var saltoColorG = (decimal)(color2G - color1G) / width;
            var saltoColorB = (decimal)(color2B - color1B) / width;

            // Rellenar el degradado
            for (int x = 0; x < width; x++)
            {
                Color blendedColor;
                int red = color1R + (int)(saltoColorR * x);
                int green = color1G + (int)(saltoColorG * x);
                int blue = color1B + (int)(saltoColorB * x);

                blendedColor = Color.FromArgb(red, green, blue);

                for (int y = 0; y < height; y++)
                {
                    bitmap.SetPixel(x, y, blendedColor);
                }
            }


            // Guardar la imagen como JPEG en un byte[]
            byte[] byteArray = bitmap.ExportBytes(AnyBitmap.ImageFormat.Jpeg);
            return byteArray;
        }

        private static MemoryStream ConvertToBmpStream(Stream stream)
        {
            AnyBitmap bitmap = AnyBitmap.FromStream(stream);

            var memoryStream = new MemoryStream();
            bitmap.ExportStream(memoryStream, AnyBitmap.ImageFormat.Bmp);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;

        } 
        #endregion
    }
}
