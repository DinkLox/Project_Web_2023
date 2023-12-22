using Microsoft.AspNetCore.Mvc;

namespace WEB_2023.Areas.Admin.Models
{
    [BindProperties]
    public class UpdateCategoryModel : CreateCategoryModel
    {
        public int Id { get; set; }
    }
}