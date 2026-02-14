using AutoMapper;
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
    public class TrainerService(IUnitOfWork _unitOfWork, IMapper _mapper) : ITrainerService
    {
        public IEnumerable<TrainerViewModel> GetAllTrainers()
        {
            var Trainers = _unitOfWork.GetRepository<Trainer>().GetAll();
            if (Trainers is null || !Trainers.Any()) return [];

            return _mapper.Map<IEnumerable<TrainerViewModel>>(Trainers);
        }

        public TrainerViewModel? GetTrainerDetails(int TrainerId)
        {
            var Trainer = _unitOfWork.GetRepository<Trainer>().GetById(TrainerId);
            if (Trainer is null) return null!;

            return _mapper.Map<TrainerViewModel>(Trainer);
        }

        public bool CreateTrainer(CreateTrainerViewModel createTrainer)
        {
            try
            {
                if (IsEmailExists(createTrainer.Email) || IsPhoneExists(createTrainer.Phone)) return false;

                var Trainer = _mapper.Map<Trainer>(createTrainer);

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

            return _mapper.Map<UpdateTrainerViewModel>(Trainer);
        }

        public bool UpdateTrainer(int TrainerId, UpdateTrainerViewModel updateTrainer)
        {
            var TrainerRepo = _unitOfWork.GetRepository<Trainer>();
            var Trainer = TrainerRepo.GetById(TrainerId);
            if (Trainer is null || IsEmailExists(updateTrainer.Email) || IsPhoneExists(updateTrainer.Phone)) return false;
            try
            {
                _mapper.Map(updateTrainer, Trainer);

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
