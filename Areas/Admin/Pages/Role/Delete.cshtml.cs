using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using App.Models;
namespace App.Admin.Role
{
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, AppDbContext myBlogContext) : base(roleManager, myBlogContext)
        {

        }

        public IdentityRole Role { get; set; }
        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null) return NotFound("Khong tim thay role");

            Role = await _roleManager.FindByIdAsync(roleid);
            if (Role == null)
            {

                return NotFound("Khong tim thay role");

            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string roleid)
        {

            if (roleid == null) return NotFound("Khong tim thay role");
            Role = await _roleManager.FindByIdAsync(roleid);
            if (roleid == null) return NotFound("Khong tim thay role");



            
           


            var result = await _roleManager.DeleteAsync(Role);

            if (result.Succeeded)
            {
                StatusMessage = $"Ban vua Xoa thanh cong Role {Role.Name}";
                return RedirectToPage("./index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }
    }
}
