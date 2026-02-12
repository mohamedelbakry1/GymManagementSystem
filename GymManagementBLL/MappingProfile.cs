using AutoMapper;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementDAL.Entities;
using GymManagementSystemBLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Session,SessionViewModel>()
                .ForMember(dest => dest.CategoryName, options => options.MapFrom(src => src.SessionCategory))
                .ForMember(dest => dest.TrainerName, options => options.MapFrom(src => src.SessionTrainer))
                .ForMember(dest => dest.AvailableSlots, options => options.Ignore());

            CreateMap<CreateSessionViewModel, Session>();

            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();
        }
    }
}
