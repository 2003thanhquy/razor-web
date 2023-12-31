// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

       

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
       
        public AppUser user { get; set; }

        [BindProperty]
        [DisplayName("Cac role gan cho user")]
        public string[] RoleNames {get;set;}
        public SelectList allRoles {get;set;}

        public List< IdentityRoleClaim<string>> claimsInRole{get;set;}
        public List<IdentityUserClaim<string>> claimsInUser{get;set;}
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"khong co user");
            }
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"khong thay user voi id = {id}");
            }

            RoleNames = (await  _userManager.GetRolesAsync(user)).ToArray<string>();

            
            await GetClaims(id);
            


            return Page();
        }
        async Task GetClaims(string id){

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles =  new SelectList(roleNames);

            var listRoles = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;

            var _claimsInRole = from c in _context.RoleClaims
                                join r in listRoles on c.RoleId equals r.Id
                                select c;

            claimsInRole = await _claimsInRole.ToListAsync();

            claimsInUser =await (from c in _context.UserClaims
            where c.UserId ==id 
            select c).ToListAsync();

        }
        public async Task<IActionResult> OnPostAsync(string id)
        {

           
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"khong co user");
            }
            user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound($"khong thay user voi id = {id}");
            }

            await GetClaims(id);
            //Role Names

            var oldRoleNames =(await _userManager.GetRolesAsync(user)).ToArray();
           
           var deleteRoles = oldRoleNames.Where(r => !RoleNames.Contains(r));
           var addRoles = RoleNames.Where(r => !oldRoleNames.Contains(r));
            
             List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles =  new SelectList(roleNames);
           
           var resultDelete =await _userManager.RemoveFromRolesAsync(user, deleteRoles);
          

           if(!resultDelete.Succeeded){
                resultDelete.Errors.ToList().ForEach(error =>{
                    ModelState.AddModelError(string.Empty, error.Description);
                });

                return Page();
           }
            var resultAdd =await _userManager.AddToRolesAsync(user, addRoles);
          

           if(!resultAdd.Succeeded){
                resultAdd.Errors.ToList().ForEach(error =>{
                    ModelState.AddModelError(string.Empty, error.Description);
                });

                return Page();
           }


            if (!ModelState.IsValid)
            {
                return Page();
            }

            
            

            
            StatusMessage = $"Cap nhat Role cho user: {user.UserName}";

            return RedirectToPage("./index");
        }
    }
}
