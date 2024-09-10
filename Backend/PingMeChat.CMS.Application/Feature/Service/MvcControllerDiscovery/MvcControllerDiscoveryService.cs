using PingMeChat.CMS.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.MvcControllerDiscovery
{
    // Định nghĩa interface cho service khám phá các controller MVC
    public interface IMvcControllerDiscoveryService
    {
        Task<IEnumerable<MvcControllerInfo>> GetControllers();
    }

    // Triển khai service khám phá các controller MVC
    public class MvcControllerDiscoveryService : IMvcControllerDiscoveryService
    {
        private List<MvcControllerInfo> _mvcControllers;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        // Constructor, nhận dependency IActionDescriptorCollectionProvider
        public MvcControllerDiscoveryService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        // Phương thức để lấy danh sách các controller
        public Task<IEnumerable<MvcControllerInfo>> GetControllers()
        {
            return Task.Factory.StartNew(() =>
            {
                // Kiểm tra cache, nếu đã có dữ liệu thì trả về ngay
                if (_mvcControllers != null)
                    return _mvcControllers as IEnumerable<MvcControllerInfo>;

                _mvcControllers = new List<MvcControllerInfo>();

                // Lọc và nhóm các action descriptor theo controller
                var items = _actionDescriptorCollectionProvider
                    .ActionDescriptors.Items
                    .Where(descriptor => descriptor.GetType() == typeof(ControllerActionDescriptor))
                    .Select(descriptor => (ControllerActionDescriptor)descriptor)
                    .GroupBy(descriptor => descriptor.ControllerTypeInfo.FullName)
                    .ToList();

                foreach (var actionDescriptors in items)
                {
                    if (!actionDescriptors.Any())
                        continue;

                    var actionDescriptor = actionDescriptors.First();
                    var controllerTypeInfo = actionDescriptor.ControllerTypeInfo;

                    // Tạo đối tượng MvcControllerInfo cho mỗi controller
                    var currentController = new MvcControllerInfo
                    {
                        AreaName = controllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                        DisplayName = controllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                        Name = actionDescriptor.ControllerName,
                    };

                    var actions = new List<MvcActionInfo>();

                    // Lặp qua các action trong controller
                    foreach (var descriptor in actionDescriptors.GroupBy(a => a.ActionName).Select(g => g.First()))
                    {
                        var methodInfo = descriptor.MethodInfo;
                        // Kiểm tra xem action có được bảo vệ không
                        if (IsProtectedAction(controllerTypeInfo, methodInfo))
                            actions.Add(new MvcActionInfo
                            {
                                ControllerId = currentController.Id,
                                Name = descriptor.ActionName,
                                DisplayName = methodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                            });
                    }

                    // Nếu có action được bảo vệ, thêm controller vào danh sách
                    if (actions.Any())
                    {
                        currentController.Actions = actions;
                        _mvcControllers.Add(currentController);
                    }
                }

                return _mvcControllers;
            });
        }

        // Phương thức kiểm tra xem một action có được bảo vệ không
        private static bool IsProtectedAction(MemberInfo controllerTypeInfo, MemberInfo actionMethodInfo)
        {
            // Action không được bảo vệ nếu có [AllowAnonymous]
            if (actionMethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(true) != null)
                return false;

            // Action được bảo vệ nếu controller hoặc action có [Authorize]
            if (controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>(true) != null)
                return true;
            if (actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>(true) != null)
                return true;

            // Mặc định, action không được bảo vệ
            return false;
        }
    }
}
