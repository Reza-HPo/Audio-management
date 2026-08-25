using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Categories;

public class CategoryCreateViewModel
{
    [Required(
        ErrorMessage = "نام دسته‌بندی را وارد کنید.")]
    [StringLength(
        100,
        ErrorMessage = "نام دسته‌بندی نمی‌تواند بیشتر از 100 کاراکتر باشد.")]
    public string Name { get; set; } = string.Empty;


    [StringLength(
        500,
        ErrorMessage = "توضیحات نمی‌تواند بیشتر از 500 کاراکتر باشد.")]
    public string? Description { get; set; }


    [StringLength(
        150,
        ErrorMessage = "Slug نمی‌تواند بیشتر از 150 کاراکتر باشد.")]
    public string? Slug { get; set; }


    public bool IsActive { get; set; } = true;


    [Range(
        0,
        1000,
        ErrorMessage = "ترتیب نمایش باید بین 0 تا 1000 باشد.")]
    public int DisplayOrder { get; set; }
}