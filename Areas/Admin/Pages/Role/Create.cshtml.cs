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
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager,AppDbContext myBlogContext) :base(roleManager,myBlogContext){

        }
        public class InputModel{
            [DisplayName("Ten cua Role")]
            [Required(ErrorMessage ="phai nhap {0}")]
            [StringLength(256,MinimumLength =3,ErrorMessage ="{0} phai dai tu {2} den {1}")]
            public string Name{get;set;}


        }
        [BindProperty]
        public InputModel Input{get;set;}
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost(){

            if(!ModelState.IsValid){
                return Page();
            }


            IdentityRole role = new IdentityRole(Input.Name);
           var result =await _roleManager.CreateAsync(role);

           if(result.Succeeded){
            StatusMessage =$"Ban vua tao thanh cong Role {Input.Name}";
            return RedirectToPage("./index");
           }else{
                result.Errors.ToList().ForEach(error =>{
                    ModelState.AddModelError(string.Empty, error.Description);
                });
           }
            return Page();
        }
    }
}
