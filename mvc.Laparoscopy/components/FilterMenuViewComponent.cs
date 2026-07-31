using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QueryService.Interfaces;

public class FilterMenuViewComponent : ViewComponent
{
    private readonly ICategoryServiceQuery _categoryService;
    public IMapper mapper;

    public FilterMenuViewComponent(ICategoryServiceQuery categoryService, IMapper mapper_) 
        { 
        _categoryService = categoryService; 
        mapper = mapper_;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? selectedCategoryId)
    {
        var categories = await _categoryService.GetAll();


        var model = categories.Select(c => new SelectListItem
        {
            Text = c.Name,
            Value = c.Name
        }).ToList();

        return View(model);

    }
}