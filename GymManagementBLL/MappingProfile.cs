using AutoMapper;
using GymManagementBLL.ViewModels.MemberViewModels;
using GymManagementBLL.ViewModels.PlanViewModels;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementBLL.ViewModels.TrianerViewModels;
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
            MapSession();
            MapMember();
            MapTrainer();
            MapPlan();
        }

        private void MapSession()
        {
            CreateMap<Session, SessionViewModel>()
            .ForMember(dest => dest.CategoryName, options => options.MapFrom(src => src.SessionCategory))
            .ForMember(dest => dest.TrainerName, options => options.MapFrom(src => src.SessionTrainer))
            .ForMember(dest => dest.AvailableSlots, options => options.Ignore());

            CreateMap<CreateSessionViewModel, Session>();

            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();
        }

        private void MapMember()
        {
            CreateMap<Member, MemberViewModel>()
                .ForMember(dest => dest.Gender, option => option.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.DateOfBirth, option => option.MapFrom(src => src.DateOfBirth.ToShortDateString()))
                .ForMember(dest => dest.Address, option => option.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"));

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.Address, option => option.MapFrom(src => src))
                .ForMember(dest => dest.HealthRecord, option => option.MapFrom(src => src.HealthRecordViewModel));

            CreateMap<CreateMemberViewModel, Address>()
                .ForMember(dest => dest.BuildingNumber, option => option.MapFrom(src => src.BuildingNumber))
                .ForMember(dest => dest.Street, option => option.MapFrom(src => src.Street))
                .ForMember(dest => dest.City, option => option.MapFrom(src => src.City));

            CreateMap<Member, UpdateMemberViewModel>()
                .ForMember(dest => dest.BuildingNumber, option => option.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(dest => dest.Street, option => option.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, option => option.MapFrom(src => src.Address.City));

            CreateMap<UpdateMemberViewModel, Member>()
                .ForMember(dest => dest.Name, option => option.Ignore())
                .ForMember(dest => dest.Photo, option => option.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.Street = src.Street;
                    dest.Address.City = src.City;
                });
        }

        private void MapTrainer()
        {
            CreateMap<Trainer, TrainerViewModel>()
                .ForMember(dest => dest.Specialities, option => option.MapFrom(src => src.Specialties.ToString()))
                .ForMember(dest => dest.DateOfBirth, option => option.MapFrom(src => src.DateOfBirth.ToShortDateString()))
                .ForMember(dest => dest.Address, option => option.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"));

            CreateMap<CreateTrainerViewModel, Trainer>()
                .ForMember(dest => dest.Address, option => option.MapFrom(src => src));

            CreateMap<CreateTrainerViewModel, Address>()
                .ForMember(dest => dest.BuildingNumber, option => option.MapFrom(src => src.BuildingNumber))
                .ForMember(dest => dest.Street, option => option.MapFrom(src => src.Street))
                .ForMember(dest => dest.City, option => option.MapFrom(src => src.City));

            CreateMap<Trainer, UpdateTrainerViewModel>()
                .ForMember(dest => dest.BuildingNumber, option => option.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(dest => dest.Street, option => option.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, option => option.MapFrom(src => src.Address.City));

            CreateMap<UpdateTrainerViewModel, Trainer>()
                .ForMember(dest => dest.Name, option => option.Ignore())
                .AfterMap((src,dest) =>
                {
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.Street = src.Street;
                    dest.Address.City = src.City;
                });

        }

        private void MapPlan()
        {
            CreateMap<Plan, PlanViewModel>();

            CreateMap<Plan, UpdatePlanViewModel>();

            CreateMap<UpdatePlanViewModel, Plan>()
                .ForMember(dest => dest.Name, option => option.Ignore());
        }
    }
}
