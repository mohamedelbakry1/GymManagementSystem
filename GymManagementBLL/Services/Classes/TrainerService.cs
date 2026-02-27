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
        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainers()
        {
            var Trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();
            if (Trainers is null || !Trainers.Any()) return [];

            return _mapper.Map<IEnumerable<TrainerViewModel>>(Trainers);
        }

        public async Task<TrainerViewModel?> GetTrainerDetails(int TrainerId)
        {
            var Trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId);
            if (Trainer is null) return null!;

            return _mapper.Map<TrainerViewModel>(Trainer);
        }

        public async Task<bool> CreateTrainer(CreateTrainerViewModel createTrainer)
        {
            try
            {
                if (await IsEmailExists(createTrainer.Email) || await IsPhoneExists(createTrainer.Phone)) return false;

                var Trainer = _mapper.Map<Trainer>(createTrainer);

                await _unitOfWork.GetRepository<Trainer>().AddAsync(Trainer);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Create Trainer Failed {ex}");
                return false;
            }
        }

        public async Task<UpdateTrainerViewModel?> GetTrainerToUpdate(int TrainerId)
        {
            var Trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId);
            if (Trainer is null) return null;

            return _mapper.Map<UpdateTrainerViewModel>(Trainer);
        }

        public async Task<bool> UpdateTrainer(int TrainerId, UpdateTrainerViewModel updateTrainer)
        {
            var TrainerRepo = _unitOfWork.GetRepository<Trainer>();

            var emailExist = (await TrainerRepo.GetAllAsync(X => X.Email == updateTrainer.Email && X.Id != TrainerId)).Any();

            var phoneExist = (await TrainerRepo.GetAllAsync(X => X.Phone == updateTrainer.Phone && X.Id != TrainerId)).Any();

            var Trainer = await TrainerRepo.GetByIdAsync(TrainerId);
            if (Trainer is null || emailExist || phoneExist) return false;
            try
            {
                _mapper.Map(updateTrainer, Trainer);

                Trainer.UpdatedAt = DateTime.Now;

                TrainerRepo.Update(Trainer);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Trainer Failed {ex}");
                return false;
            }
        }

        public async Task<bool> RemoveTrainer(int TrainerId)
        {
            var Trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId);

            var ActiveSessions = (await _unitOfWork.GetRepository<Session>()
                                            .GetAllAsync(X => X.TrainerId == TrainerId && X.StartDate > DateTime.Now)).Any();
            if(Trainer is null || ActiveSessions) return false;
            try
            {
                _unitOfWork.GetRepository<Trainer>().Delete(Trainer);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Remove Trainer Failed {ex}");
                return false;
            }
        }

        #region Helper Methods
        private async Task<bool> IsEmailExists(string email)
        {
            return (await _unitOfWork.GetRepository<Trainer>().GetAllAsync(X => X.Email == email)).Any();
        }

        private async Task<bool> IsPhoneExists(string phone)
        {
            return (await _unitOfWork.GetRepository<Trainer>().GetAllAsync(X => X.Phone == phone)).Any();
        }
        #endregion
    }
}
