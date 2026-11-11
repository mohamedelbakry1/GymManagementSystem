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
    internal class TrainerService(IUnitOfWork _unitOfWork, IMapper _mapper) : ITrainerService
    {
        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var Trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            if (Trainers is null || !Trainers.Any()) return [];

            return _mapper.Map<IEnumerable<TrainerViewModel>>(Trainers);
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int TrainerId, CancellationToken ct = default)
        {
            var Trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct: ct);
            if (Trainer is null) return null!;

            return _mapper.Map<TrainerViewModel>(Trainer);
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel createTrainer, CancellationToken ct = default)
        {
            try
            {
                if (await IsEmailExistsAsync(createTrainer.Email, ct) || await IsPhoneExistsAsync(createTrainer.Phone, ct)) return false;

                var Trainer = _mapper.Map<Trainer>(createTrainer);

                _unitOfWork.GetRepository<Trainer>().Add(Trainer);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Create Trainer Failed {ex}");
                return false;
            }
        }

        public async Task<UpdateTrainerViewModel?> GetTrainerToUpdateAsync(int TrainerId, CancellationToken ct = default)
        {
            var Trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct: ct);
            if (Trainer is null) return null;

            return _mapper.Map<UpdateTrainerViewModel>(Trainer);
        }

        public async Task<bool> UpdateTrainerAsync(int TrainerId, UpdateTrainerViewModel updateTrainer, CancellationToken ct = default)
        {
            var TrainerRepo = _unitOfWork.GetRepository<Trainer>();

            var emailExist = (await TrainerRepo.GetAllAsync(X => X.Email == updateTrainer.Email && X.Id != TrainerId, ct)).Any();

            var phoneExist = (await TrainerRepo.GetAllAsync(X => X.Phone == updateTrainer.Phone && X.Id != TrainerId, ct)).Any();

            var Trainer = await TrainerRepo.GetByIdAsync(TrainerId, ct: ct);
            if (Trainer is null || emailExist || phoneExist) return false;
            try
            {
                _mapper.Map(updateTrainer, Trainer);

                Trainer.UpdatedAt = DateTime.Now;

                TrainerRepo.Update(Trainer);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Trainer Failed {ex}");
                return false;
            }
        }

        public async Task<bool> RemoveTrainerAsync(int TrainerId, CancellationToken ct = default)
        {
            var Trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct: ct);

            var ActiveSessions = (await _unitOfWork.GetRepository<Session>()
                                            .GetAllAsync(X => X.TrainerId == TrainerId && X.StartDate > DateTime.Now, ct)).Any();
            if(Trainer is null || ActiveSessions) return false;
            try
            {
                _unitOfWork.GetRepository<Trainer>().Delete(Trainer);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Remove Trainer Failed {ex}");
                return false;
            }
        }

        #region Helper Methods
        private async Task<bool> IsEmailExistsAsync(string email, CancellationToken ct = default)
        {
            return (await _unitOfWork.GetRepository<Trainer>().GetAllAsync(X => X.Email == email, ct)).Any();
        }

        private async Task<bool> IsPhoneExistsAsync(string phone, CancellationToken ct = default)
        {
            return (await _unitOfWork.GetRepository<Trainer>().GetAllAsync(X => X.Phone == phone, ct)).Any();
        }
        #endregion
    }
}
