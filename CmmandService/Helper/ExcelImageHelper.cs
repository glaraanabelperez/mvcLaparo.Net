using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using Serilog.Core;
using System.Drawing.Imaging;

namespace CmmandService.Helper
{
    public static class ExcelImageHelper
    {
        public static string? SaveImageFromCell(
            ExcelWorksheet worksheet,
            int row,
            int colImagen,
            string outputFolder)
        {
            if (colImagen <= 0)
                return null;

            Directory.CreateDirectory(outputFolder);

            foreach (var drawing in worksheet.Drawings)
            {
                if (drawing is ExcelPicture picture)
                {
                    int picRow = picture.From.Row + 1;
                    int picCol = picture.From.Column + 1;

                    if (picRow == row && picCol == colImagen)
                    {
                        using var image = picture.Image;

                        if (image == null)
                            return null;

                        string fileName = $"{Guid.NewGuid()}.png";
                        string path = Path.Combine(outputFolder, fileName);

                        image.Save(path, ImageFormat.Png);

                        return fileName;
                    }
                }
            }

            return null;
        }

        public static void LimpiarCarpetaSafe(string path)
        {
            if (!Directory.Exists(path))
                return;

            foreach (var file in Directory.GetFiles(path))
            {
                try
                {
                    File.Delete(file);
                }
                catch {  }
            }

            foreach (var dir in Directory.GetDirectories(path))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch { /* log */ }
            }
        }

        public static void MoveImageBackup(string pathOrigin, string pathfinal)
        {
            if (!Directory.Exists(pathOrigin) || !Directory.Exists(pathfinal))
                return;

            // 🔹 Mover archivos
            foreach (var file in Directory.GetFiles(pathOrigin))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(pathfinal, fileName);
             
                File.Move(file, destFile);
            }
        }
    }

}
