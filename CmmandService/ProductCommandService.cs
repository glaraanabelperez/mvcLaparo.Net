using CmmandService.Helper;
using CmmandService.Interfaces;
using CmmandService.ModelsCommand;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using mvc.Laparoscopy.Persistence;
using OfficeOpenXml;
using Repositorys;
using Repositorys.Interfaces;
using System.Text.RegularExpressions;
using Utils;

namespace CmmandService
{
    public class ProductCommandService: IProductCommandService
    {
        private IGenericRepository commandGeneric;
        private IProductRepository _productRepositor;
        private readonly ApplicationDbContext dbContext;
        private readonly IOptions<PathsOptions> _options;
        private string tempPath; 
        private string imagesPath;

        public ProductCommandService(IOptions<PathsOptions> options, IProductRepository productRepositor,
            IGenericRepository command, ApplicationDbContext _dbContext)
        {
            _productRepositor = productRepositor;
            commandGeneric = command;
            dbContext = _dbContext;
            _options= options;
            tempPath = _options.Value.tempPath;
            imagesPath = _options.Value.imagesPath;
            
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
                throw;
            }
            return res;
        }

        private async Task AddListCategorys(List<Category> categorys)
        {
            var res = new ResultApp<Category?>();
            try
            {
                await this.commandGeneric.AddRange<Category>(categorys);             
            }
            catch (Exception ex)
            {            
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw;
            }
        }//
        private async Task<ResultApp<Product?>> AddList(List<Product> products)
        {
            var res = new ResultApp<Product?>();
            try
            {
                var numberAgregated = await this._productRepositor.AddRangeAndCleanProduct(products);
                if (numberAgregated)
                {
                    res.Succeeded = true;
                    res.message = "Creado";
                }
                else
                {
                    res.Succeeded = false;
                    res.message = "Hay un error en el excel y no se pudieron actualizar los datos";
                }

                    RestoreImages(numberAgregated);

            }
            catch (Exception ex)
            {
                if (Directory.Exists(tempPath))
                {
                    ExcelImageHelper.MoveImageBackup(tempPath, imagesPath);
                    Directory.Delete(tempPath, true);
                }
                   
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw;
            }
            return res;
        }//

        public void RestoreImages(bool agregated)
        {
            try
            {
                if (agregated)
                    Directory.Delete(tempPath, true);
                else
                {

                    ExcelImageHelper.MoveImageBackup(tempPath, imagesPath);
                    Directory.Delete(tempPath, true);
                }
            }
            catch (IOException e)
            {
                throw;
            }          
        }
        private void BackupImages(string imagesPath, string tempPath)
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
              if(command_.Category != null)
              {
               entity.Category_ = command_.Category;
              }

            return entity;
        }
        public async Task<Dictionary<string, Category>> GetCategoys()//
        {
            var categories =  await dbContext.Category
                    .ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);
            return categories;
        }
        public async Task<ResultApp<Product?>> ChargeData(IFormFile file)
        {
            var dataList = new List<Product>();
            var dataListCategory = new List<Category>();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var categorias_dictionary = await GetCategoys();

            using (var stream = file.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
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
                
                if(colCategoria <= 0 || colProducto <= 0 || colCodigo <= 0 || colDescripcion <= 0)
                    throw new Exception("Faltan algunas de estas columnas obligatorias: Especialidad, Producto, Codigo, Descripcion .");


                //Guarda imagenes en carpeta temporal antes de limpiar la carpeta definitiva para evitar perdida de imagenes en caso de error
                BackupImages(imagesPath, tempPath);
                string? imageName = null;
                int imgenCombinaRow = 0;

                //Obtiene categorias de la bbdd para evitar duplicados y mejorar rendimiento-

                for (int row = 2; row <= rowCount; row++)
                {

                   imageName = ExcelImageHelper.SaveImageFromCell(
                                               worksheet,
                                               row,
                                               colImagen,
                                               imagesPath
                                               );


                   string categoriaName = colCategoria > 0 ? worksheet.Cells[row, colCategoria].Text?.Trim() ?? "" : "";

                   //Agrega Categoria en la bbdd o en el diccionario si no existe para evitar duplicados-
                   if (!categorias_dictionary.TryGetValue(categoriaName, out var category))
                   {
                        category = new Category { Name = categoriaName.ToUpper() };
                        categorias_dictionary[categoriaName] = category;
                        dataListCategory.Add(category);

                    }

                   //Continua obteniendo el resto de campos-
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
                        Category = category,
                        Name = name,
                        Codigo = codigo,
                        Description = descripcion,
                        State = true
                    };

                   dataList.Add(MapToEntity(model));
                }
            }
            if(dataListCategory.Count>0)
                await this.AddListCategorys(dataListCategory);
            return await this.AddList(dataList);

        }

        decimal ParseCellPrice(string price)
        {
          
            price = price.Replace("\u2019", "'");

            var cleaned = Regex.Replace(price, @"[^\d\.,\-]", "");
            return decimal.Parse(cleaned);
        }


    }
}

