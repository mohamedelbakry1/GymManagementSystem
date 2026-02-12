using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.TrianerViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Classes
{
    public class TrainerService(IUnitOfWork _unitOfWork) : ITrainerService
    {
        public IEnumerable<TrainerViewModel> GetAllTrainers()
        {
            var Trainers = _unitOfWork.GetRepository<Trainer>().GetAll();
            if (Trainers is null || !Trainers.Any()) return [];

            return Trainers.Select(T => new TrainerViewModel()
            {
                Id = T.Id,
                Name = T.Name,
                Email = T.Email,
                Phone = T.Phone,
                Specialities = T.Specialties.ToString(),
            });
        }

        public TrainerViewModel? GetTrainerDetails(int TrainerId)
        {
            var Trainer = _unitOfWork.GetRepository<Trainer>().GetById(TrainerId);
            if (Trainer is null) return null!;

            return new TrainerViewModel()
            {
                Name = Trainer.Name,
                Specialities = Trainer.Specialties.ToString(),
                Email = Trainer.Email,
                Phone = Trainer.Phone,
                DateOfBirth = Trainer.DateOfBirth.ToShortDateString(),
                Address = $"{Trainer.Address.BuildingNumber} - {Trainer.Address.Street} - {Trainer.Address.City}"
            };
        }

        public bool CreateTrainer(CreateTrainerViewModel createTrainer)
        {
            try
            {
                if (IsEmailExists(createTrainer.Email) || IsPhoneExists(createTrainer.Phone)) return false;

                var Trainer = new Trainer()
                {
                    Name = createTrainer.Name,
                    Email = createTrainer.Email,
                    Phone = createTrainer.Phone,
                    DateOfBirth = createTrainer.DateOfBirth,
                    Gender = createTrainer.Gender,
                    Address = new Address
                    {
                        BuildingNumber = createTrainer.BuildingNumber,
                        Street = createTrainer.Street,
                        City = createTrainer.City,
                    },
                    Specialties = createTrainer.Specialties,
                };
                _unitOfWork.GetRepository<Trainer>().Add(Trainer);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Create Trainer Failed {ex}");
                return false;
            }
        }

        public UpdateTrainerViewModel? GetTrainerToUpdate(int TrainerId)
        {
            var Trainer = _unitOfWork.GetRepository<Trainer>().GetById(TrainerId);
            if (Trainer is null) return null;

            return new UpdateTrainerViewModel()
            {
                Name = Trainer.Name,
                Email = Trainer.Email,
                Phone = Trainer.Phone,
                BuildingNumber = Trainer.Address.BuildingNumber,
                Street = Trainer.Address.Street,
                City = Trainer.Address.City,
                Specialties = Trainer.Specialties,
            };
        }

        public bool UpdateTrainer(int TrainerId, UpdateTrainerViewModel updateTrainer)
        {
            var TrainerRepo = _unitOfWork.GetRepository<Trainer>();
            var Trainer = TrainerRepo.GetById(TrainerId);
            if (Trainer is null || IsEmailExists(updateTrainer.Email) || IsPhoneExists(updateTrainer.Phone)) return false;
            try
            {
                Trainer.Email = updateTrainer.Email;
                Trainer.Phone = updateTrainer.Phone;
                Trainer.Address.BuildingNumber = updateTrainer.BuildingNumber;
                Trainer.Address.Street = updateTrainer.Street;
                Trainer.Address.City = updateTrainer.City;
                Trainer.UpdatedAt = DateTime.Now;

                TrainerRepo.Update(Trainer);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Trainer Failed {ex}");
                return false;
            }
        }

        public bool RemoveTrainer(int TrainerId)
        {
            var Trainer = _unitOfWork.GetRepository<Trainer>().GetById(TrainerId);

            var ActiveSessions = _unitOfWork.GetRepository<Session>()
                                            .GetAll(X => X.TrainerId == TrainerId && X.StartDate > DateTime.Now).Any();
            if(Trainer is null || ActiveSessions) return false;
            try
            {
                _unitOfWork.GetRepository<Trainer>().Delete(Trainer);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Remove Trainer Failed {ex}");
                return false;
            }
        }

        #region Helper Methods
        private bool IsEmailExists(string email)
        {
            return _unitOfWork.GetRepository<Trainer>().GetAll(X => X.Email == email).Any();
        }

        private bool IsPhoneExists(string phone)
        {
            return _unitOfWork.GetRepository<Trainer>().GetAll(X => X.Phone == phone).Any();
        }
        #endregion
    }
}
