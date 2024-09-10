using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingMeChat.CMS.Entities.Users;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        //private readonly UserManager<Account> _userManager;
        //private readonly RoleManager<Role> _roleManager;
        //public RoleRepository(AppDBContext context,
        //    UserManager<Account> userManager,
        //    RoleManager<Role> roleManager) : base(context)
        //{
        //    _userManager = userManager;
        //    _roleManager = roleManager;
        //}

        //public async Task<bool> AddRolesForUser(string userId, List<string> roles)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user != null)
        //    {
        //        foreach (var role in roles)
        //        {
        //            var result = await _userManager.AddToRoleAsync(user, role);
        //            if (!result.Succeeded)
        //            {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        //public async Task<Role> CreateRoleByNameAsync(string name)
        //{
        //    var isRoleName = await _roleManager.RoleExistsAsync(name);
        //    if (isRoleName)
        //    {
        //        return new Role();
        //    }
        //    var role = new Role()
        //    {
        //        Name = name
        //    };

        //    var result = await _roleManager.CreateAsync(role);

        //    if (result.Succeeded)
        //    {
        //        return role;
        //    }
        //    return new Role();
        //}

        //public async Task<List<Role>> GetAllAsync()
        //{
        //    var roles = await _roleManager.Roles.ToListAsync();
        //    if(roles.Count() > 0)
        //    {
        //        return roles;
        //    }
        //    return new List<Role>();
        //}

        //public async Task<Role> GetRoleById(string id)
        //{
        //    var role = await _roleManager.FindByIdAsync(id);
        //    if(role == null)
        //    {
        //        return new Role();
        //    }
        //    return role;
        //}

        //public async Task<bool> IsRoleName(string name)
        //{
        //    var result = await _roleManager.RoleExistsAsync(name);
        //    if (result)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public async Task<bool> RemoveRolesForUser(string userId, List<string> roles)
        //{
        //   if(roles.Count() > 0)
        //    {
        //        var user = await _userManager.FindByIdAsync(userId);
        //        var result = await _userManager.RemoveFromRolesAsync(user, roles);
        //        if (!result.Succeeded)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        //public async Task<Role> UpdateNameRole(string id, string name)
        //{
        //    var role = await _roleManager.FindByIdAsync(id);
        //    role.Name = name;

        //    await _roleManager.UpdateAsync(role);
        //    return role;

        //}
        public RoleRepository(AppDBContext context) : base(context)
        {
        }
    }
}
