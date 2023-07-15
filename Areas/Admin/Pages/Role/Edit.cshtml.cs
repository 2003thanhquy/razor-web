using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using App.Models;
namespace App.Admin.Role
{
    [Authorize(Policy = "AllowEditRole")]
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager,AppDbContext myBlogContext) :base(roleManager,myBlogContext){

        }
        public class InputModel{
            [DisplayName("Ten cua Role")]
            [Required(ErrorMessage ="phai nhap {0}")]
            [StringLength(256,MinimumLength =3,ErrorMessage ="{0} phai dai tu {2} den {1}")]
            public string Name{get;set;}


        }
        [BindProperty]
        public InputModel Input{get;set;}

        public List<IdentityRoleClaim<string>>Claims {get;set;}
        public IdentityRole Role {get;set;}
        public async Task<IActionResult> OnGet(string roleid)
        {
            if(roleid ==null) return NotFound("Khong tim thay role");

            Role =  await _roleManager.FindByIdAsync(roleid);
            if(Role !=  null){
                Input = new InputModel(){
                    Name = Role.Name
                };
                Claims =await _myBlogContext.RoleClaims.Where(rc => rc.RoleId == Role.Id).ToListAsync();  
                return Page();
            }
            return NotFound("Khong tim thay role");
        }
        public async Task<IActionResult> OnPost(string roleid){

            if(roleid ==null) return NotFound("Khong tim thay role");
             Role =  await _roleManager.FindByIdAsync(roleid);
              if(roleid ==null) return NotFound("Khong tim thay role");

            Claims =await _myBlogContext.RoleClaims.Where(rc => rc.RoleId == Role.Id).ToListAsync(); 
            
            if(!ModelState.IsValid){
                return Page();
            }
            Role.Name= Input.Name;

           
           var result =await _roleManager.UpdateAsync(Role);

           if(result.Succeeded){
            StatusMessage =$"Ban vua cap nhat thanh cong Role {Input.Name}";
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
