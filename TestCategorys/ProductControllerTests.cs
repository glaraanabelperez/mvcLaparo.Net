//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging.Abstractions;
//using mvc.Laparoscopy.Controllers;
//using mvc.Laparoscopy.Models;
//using QueryService;
//using QueryService.Interfaces;
//using QueryService.Models;
//using Utils;
//using Xunit;

//namespace mvc.Laparoscopy.Tests
//{
//    public class ProductControllerTests
//    {
//        [Fact]
//        public async Task Index_Populates_ViewBag_Categories()
//        {
//            // Arrange
//            var categoryService = new FakeCategoryService();
//            var productService = new FakeProductService();
//            var mapper = new FakeMapper();
//            var logger = NullLogger<ProductController>.Instance;

//            var controller = new ProductController(productService, categoryService, mapper, logger);

//            // Act
//            var result = await controller.Index(null, null, null, null, null, 1);

//            // Assert
//            Assert.IsType<ViewResult>(result);
//            Assert.NotNull(controller.ViewBag.Categories);
//            var categories = controller.ViewBag.Categories as List<CategoryDto>;
//            Assert.NotNull(categories);
//            Assert.Single(categories);
//            Assert.Equal("cat-1", categories[0].Id);
//        }
//    }

//    // Fakes mínimos para el test (SRP: cada fake tiene una única responsabilidad)
//    class FakeCategoryService : ICategoryServiceQuery
//    {
//        public Task<List<CategoryDto>> GetAll()
//        {
//            var list = new List<CategoryDto> {
//                new CategoryDto { Id = "cat-1", Name = "Categoría 1" }
//            };
//            return Task.FromResult(list);
//        }
//    }

//    class FakeProductService : IProductServiceQuery
//    {
//        public Task<DataCollection<ProductDto>> GetAll(string categoryId, bool? Fauvorite, bool? Discount, bool? State, string? Search, int From, int Length)
//        {
//            // No necesitamos productos para este test; devolver null o una instancia vacía está bien porque FakeMapper lo maneja
//            return Task.FromResult<DataCollection<ProductDto>>(null);
//        }
//    }

//    class FakeMapper : IMapper
//    {
//        public TDestination Map<TDestination>(object source)
//        {
//            // Para el controller sólo necesitamos devolver un PagedResponse<ProductViewModel> vacío
//            if (typeof(TDestination) == typeof(PagedResponse<ProductViewModel>))
//            {
//                object obj = new PagedResponse<ProductViewModel>();
//                return (TDestination)obj;
//            }

//            return Activator.CreateInstance<TDestination>();
//        }

//        // Interfaces restantes no usadas en el test
//        public object Map(object source, Type sourceType, Type destinationType) => throw new NotImplementedException();
//        public TDestination Map<TSource, TDestination>(TSource source) => throw new NotImplementedException();
//    }
//}
