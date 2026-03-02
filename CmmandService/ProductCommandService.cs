using CmmandService.Helper;
using CmmandService.Interfaces;
using CmmandService.ModelsCommand;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Models;
using OfficeOpenXml;
using Repositorys.Interfaces;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using Utils;

namespace CmmandService
{
    public class ProductCommandService: IProductCommandService
    {
        private readonly ILogger<ProductCommandService> _logger;
        //private readonly IProductRepository _productRepo;
        public IGenericRepository commandGeneric;
        public string imagesPath = Path.Combine(
                                     @"C:\Uploads",
                                     "images",
                                     "products"
                                 );

        public string tempPath = Path.Combine(
                             @"C:\Uploads",
                             "images",
                             "products-temp"
                         );
        public ProductCommandService( IGenericRepository command, 
            ILogger<ProductCommandService> logger)
        {
            this.commandGeneric = command;
            //_productRepo = productRepo;
            _logger = logger;
        }

        public async Task<ResultApp<Product?>> Add(ProductCreateCommand command)
        {
            var res = new ResultApp<Product?>();
            try
            {
                var product = MapToEntity(command);
                res.objectResult = await this.commandGeneric.Add<Product>(product);
                res.Succeeded = true;
                res.message = "Creado";
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogWarning(msg);
                res.Succeeded = false;
                res.message = msg;
            }
            return res;
        }

        private async Task<ResultApp<Product?>> AddList(List<Product> products)
        {
            var res = new ResultApp<Product?>();
            try
            {
                //await this.DeleteProduct();
                var numberAgregated = await this.commandGeneric.AddRange<Product>(products);
                if (numberAgregated)
                {
                    res.Succeeded = true;
                    res.message = "Creado";

                    if (Directory.Exists(tempPath))
                        Directory.Delete(tempPath, true);
                }
                else
                {
                    if (Directory.Exists(tempPath))
                        ExcelImageHelper.MoveImageBackup(tempPath, imagesPath);

                    res.Succeeded = false;
                    res.message = "No se han podido agregar los productos";
                }
               
            }
            catch (Exception ex)
            {
                if (Directory.Exists(tempPath))
                {
                    ExcelImageHelper.MoveImageBackup(tempPath, imagesPath);
                    Directory.Delete(tempPath, true);
                }
                   
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogWarning(msg);
                res.Succeeded = false;
                res.message = msg;
            }
            return res;
        }

        private void DeleteFolderImagePreview(string imagesPath, string tempPath)
        {

            try
            {
                if (!Directory.Exists(imagesPath)) 
                {
                    Directory.CreateDirectory(imagesPath); // recrear carpeta
                }
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }

                  Directory.CreateDirectory(tempPath); // recrear carpeta

                if (Directory.GetFiles(imagesPath).Length > 0)
                {
                    ExcelImageHelper.MoveImageBackup(imagesPath, tempPath);
                    ExcelImageHelper.LimpiarCarpetaSafe(imagesPath);
                }

            }
            catch (IOException e)
            {
                //// fallback seguro
                //ExcelImageHelper.LimpiarCarpetaSafe(imagesPath);
                _logger.LogWarning(e.Message);
                throw;
            }
        }

        public Product MapToEntity(ProductCreateCommand command_)
        {
            Product entity = new Product();
              entity.Name = command_.Name;
              entity.Codigo = command_.Codigo;
              entity.Description = command_.Description; 
              //entity.Fauvorite = command_.Fauvorite;
              entity.State = command_.State; 
              entity.DateInit = DateTime.UtcNow; 
              entity.image = command_.image; 
              entity.Price = command_.Price; 
              entity.DiscountId = command_.DiscountId;
              entity.TotalPrice  = command_.TotalPrice;
              entity.Category = command_.Category;


            return entity;
        }

        public async Task ChargeData(IFormFile file)
        {

            var dataList = new List<Product>();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            using (var stream = file.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet == null)
                    throw new Exception("La hoja de cálculo está vacía.");

                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                // Mapear headers
                var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int c = 1; c <= colCount; c++)
                {
                    var header = worksheet.Cells[1, c].Text?.Trim();
                    if (!string.IsNullOrEmpty(header) && !headerMap.ContainsKey(header))
                        headerMap[header] = c;
                }

                headerMap.TryGetValue("Imagen", out var colImagen);
                headerMap.TryGetValue("Categoria", out var colCategoria);
                headerMap.TryGetValue("Producto", out var colProducto);
                headerMap.TryGetValue("Codigo", out var colCodigo);
                headerMap.TryGetValue("Descripcion", out var colDescripcion);
                headerMap.TryGetValue("Precio", out var colPrecio);

                string? imageName = null;
                int imgenCombinaRow = 0;

                

                DeleteFolderImagePreview(imagesPath, tempPath);

                for (int row = 2; row <= rowCount; row++)
                    {

                        imageName = ExcelImageHelper.SaveImageFromCell(
                                                    worksheet,
                                                    row,
                                                    colImagen,
                                                    imagesPath
                                                    );

                        string categoriaName = colCategoria > 0 ? worksheet.Cells[row, colCategoria].Text?.Trim() ?? "" : "";

                        string name = colProducto > 0 ? worksheet.Cells[row, colProducto].Text?.Trim() ?? "" : "";
                        string codigo = colCodigo > 0 ? worksheet.Cells[row, colCodigo].Text?.Trim() ?? "" : "";

                        string descripcion = (colDescripcion > 0 && string.IsNullOrEmpty(worksheet.Cells[row, colDescripcion].Text))
                            ? string.Join(" ",
                                (worksheet.Cells[row, colDescripcion].Text ?? "")
                                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)) : "";

                        decimal precio = 0m;
                        if (colPrecio > 0)
                        {
                            string precioText = worksheet.Cells[row, colPrecio].Text?.Trim() ?? "0";
                            precio = ParseCellPrice(precioText);
                        }

                        var model = new ProductCreateCommand
                        {
                            image = imageName,
                            Category = categoriaName,
                            Name = name,
                            Codigo = codigo,
                            Description = descripcion,
                            //Price = precio,
                            TotalPrice = precio,
                            State = true

                        };

                        dataList.Add(MapToEntity(model));
                    }
            }

            await this.AddList(dataList);

        }

        decimal ParseCellPrice(string price)
        {
          
            price = price.Replace("\u2019", "'");

            var cleaned = Regex.Replace(price, @"[^\d\.,\-]", "");
            return decimal.Parse(cleaned);
        }


    }
}

