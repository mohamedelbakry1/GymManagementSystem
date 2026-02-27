using GymManagementBLL.ViewModels.TrianerViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainers();
        Task<bool> CreateTrainer(CreateTrainerViewModel createTrainer);
        Task<TrainerViewModel?> GetTrainerDetails(int TrainerId);
        Task<UpdateTrainerViewModel?> GetTrainerToUpdate(int TrainerId);
        Task<bool> UpdateTrainer(int TrainerId, UpdateTrainerViewModel updateTrainer);
        Task<bool> RemoveTrainer(int TrainerId);
    }
}
