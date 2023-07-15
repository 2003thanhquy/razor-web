using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using App.Models;
namespace App.Admin.Role
{
    public class AddRoleClaimModel : RolePageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager,AppDbContext myBlogContext) :base(roleManager,myBlogContext){

        }
        public class InputModel{
            [DisplayName("Ten cua ClaimType")]
            [Required(ErrorMessage ="phai nhap {0}")]
            [StringLength(256,MinimumLength =3,ErrorMessage ="{0} phai dai tu {2} den {1}")]
            public string ClaimType{get;set;}

             [DisplayName("Ten cua ClaimValue")]
            [Required(ErrorMessage ="phai nhap {0}")]
            [StringLength(256,MinimumLength =3,ErrorMessage ="{0} phai dai tu {2} den {1}")]
            public string ClaimValue{get;set;}


        }
        [BindProperty]
        public InputModel Input{get;set;}
        public IdentityRole role {get;set;}
        public async Task<IActionResult> OnGet(string roleid)
        {
           role=await _roleManager.FindByIdAsync(roleid);
           if(role ==null) return  NotFound("Khong tim thay trang ");
           return Page();
        }
        public async Task<IActionResult> OnPost(string roleid){

            role=await _roleManager.FindByIdAsync(roleid);
           if(role ==null) return  NotFound("Khong tim thay trang ");
            if(!ModelState.IsValid){
                return Page();
            }
            if((await _roleManager.GetClaimsAsync(role)).Any(c  => c.Type ==Input.ClaimType && c.Value == Input.ClaimValue)){
                ModelState.AddModelError(string.Empty, "Da co claims trong role");
                return Page();
            }
            var newClaim =  new Claim(Input.ClaimType,Input.ClaimValue );
            var result =await _roleManager.AddClaimAsync(role, newClaim);
            if(!result.Succeeded){
                result.Errors.ToList().ForEach(error =>{
                    ModelState.AddModelError(string.Empty,error.Description);
                });
                return Page();
            }
            StatusMessage ="Vua them dac tinh (claim) moi";

       
            return RedirectToPage("./Edit",new {roleid=role.Id});
        }
    }
}
