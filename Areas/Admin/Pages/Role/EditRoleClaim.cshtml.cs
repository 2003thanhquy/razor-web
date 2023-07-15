using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using App.Models;
namespace App.Admin.Role
{
   
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, AppDbContext myBlogContext) : base(roleManager, myBlogContext)
        {

        }
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
        public IdentityRole role { get; set; }
        public IdentityRoleClaim<string> claim { get; set; }
        public async Task<IActionResult> OnGet(int? claimid)
        {
            if (claimid == null) return NotFound("Khong tim thay trang");
            claim = _myBlogContext.RoleClaims.Where(rc => rc.Id == claimid.Value).FirstOrDefault();

            if (claim == null) return NotFound("Khong tim thay role ");
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Khont tim thay role");

            Input = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };
            return Page();
        }
        public async Task<IActionResult> OnPost(int? claimid)
        {

            if (claimid == null) return NotFound("Khong tim thay trang");
            claim = _myBlogContext.RoleClaims.Where(rc => rc.Id == claimid.Value).FirstOrDefault();

            if (claim == null) return NotFound("Khong tim thay role ");
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Khont tim thay role");

            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (_myBlogContext.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType
            && c.ClaimValue == Input.ClaimValue && c.Id == claimid.Value))
            {
                ModelState.AddModelError(string.Empty, "Da co claims trong role");
                return Page();
            }

            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;
            await _myBlogContext.SaveChangesAsync();

            StatusMessage = "Vua cap nhap Claim";


            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
        public async Task<IActionResult> OnPostDelete(int? claimid)
        {
            if (claimid == null) return NotFound("Khong tim thay trang");
            claim = _myBlogContext.RoleClaims.Where(rc => rc.Id == claimid.Value).FirstOrDefault();

            if (claim == null) return NotFound("Khong tim thay role ");
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Khont tim thay role");

            if (!ModelState.IsValid)
            {
                return Page();
            }
            await _roleManager.RemoveClaimAsync(role, new Claim(
                claim.ClaimType,
                claim.ClaimValue
            ));

            StatusMessage = "Vua xoa Claim";


            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
    }
}
