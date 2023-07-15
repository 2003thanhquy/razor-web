using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Admin.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        public readonly AppDbContext _context;
        public readonly UserManager<AppUser> _userManager;  
        public EditUserRoleClaimModel(AppDbContext myBlogContext,UserManager<AppUser> userManager){
            _context = myBlogContext;
            _userManager = userManager;

        }
        [TempData]
        public string StatusMessage{get;set;}

         public class InputModel
        {
            [DisplayName("Ten cua ClaimType")]
            [Required(ErrorMessage = "phai nhap {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phai dai tu {2} den {1}")]
            public string ClaimType { get; set; }

            [DisplayName("Ten cua ClaimValue")]
            [Required(ErrorMessage = "phai nhap {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phai dai tu {2} den {1}")]
            public string ClaimValue { get; set; }


        }
        [BindProperty]
        public InputModel Input { get; set; }
        public NotFoundObjectResult OnGet() => NotFound("Khong the truy cap");

        public AppUser user{get;set;}
         public IdentityUserClaim<string> userclaim {get;set;}
        
        public async Task<IActionResult> OnGetAddClaimAsync(string userid){

            user =  await _userManager.FindByIdAsync(userid);
            if(user == null){
                return NotFound("Khong tim thay user");
            }

            return Page();
        } 
       
         
        public async Task<IActionResult> OnPostAddClaimAsync(string userid){

            user =  await _userManager.FindByIdAsync(userid);
            if(user == null){
                return NotFound("Khong tim thay user");
            }

            if(!ModelState.IsValid) return Page();

            var claims = _context.UserClaims.Where( uc =>uc.UserId ==   user.Id);
            if(claims.Any(c=>c.ClaimType ==Input.ClaimType&&c.ClaimValue==Input.ClaimValue)){
                ModelState.AddModelError(string.Empty,"Dac tinh da co");
                return Page();
            }
              
            await _userManager.AddClaimAsync(user,new Claim(Input.ClaimType,Input.ClaimValue));
            StatusMessage ="Da them User claim thanh cong";

            return RedirectToPage("./AddRole",new {id = user.Id});
        } 
        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid){

            if(claimid == null){
                return NotFound("khong thay claim");
            }

            userclaim =  _context.UserClaims.Where(uc => uc.Id ==claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if(userclaim == null){
                return NotFound("Khong tim thay user");
            }
            Input = new InputModel(){
                ClaimType = userclaim.ClaimType,
                ClaimValue = userclaim.ClaimValue,
            };
            return Page();
        } 
        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid){

            if(claimid == null){
                return NotFound("khong thay claim");
            }

            userclaim =  _context.UserClaims.Where(uc => uc.Id ==claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if(user == null){
                return NotFound("Khong tim thay user");
            }
            if(!ModelState.IsValid) return Page();
            
            if( _context.UserClaims.Any(c=>c.UserId == user.Id&&c.ClaimType ==Input.ClaimType
            &&c.ClaimValue==Input.ClaimValue&& c.Id != userclaim.Id)){
                ModelState.AddModelError(string.Empty,"Dac tinh da co");
                return Page();
            }

            userclaim.ClaimType = Input.ClaimType;  
            userclaim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();
            StatusMessage ="Ban vua cap nhat thanh cong";
            return RedirectToPage("./AddRole",new {id = user.Id});
        } 

        public async Task<IActionResult> OnPostDeleteClaimAsync(int? claimid){

            if(claimid == null){
                return NotFound("khong thay claim");
            }

            userclaim =  _context.UserClaims.Where(uc => uc.Id ==claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if(user ==null)  return NotFound("khong thay user");

           
            await _userManager.RemoveClaimAsync(user,new Claim(userclaim.ClaimType,userclaim.ClaimValue));
            StatusMessage ="Ban vua xoa thanh cong";
            return RedirectToPage("./AddRole",new {id = user.Id});
        } 
        
    }
}
