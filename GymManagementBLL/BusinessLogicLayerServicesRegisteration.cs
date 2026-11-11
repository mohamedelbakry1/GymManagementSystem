using GymManagementBLL.Services.AttachmentService;
using GymManagementBLL.Services.Classes;
using GymManagementBLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL
{
    public static class BusinessLogicLayerServicesRegisteration
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services)
        {
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<ITrainerService, TrainerService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddAutoMapper(X => X.AddProfile(new MappingProfile()));


            return services;
        }
    }
}
