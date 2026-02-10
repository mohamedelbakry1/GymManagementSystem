using GymManagementBLL.ViewModels.TrianerViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface ITrainerService
    {
        IEnumerable<TrainerViewModel> GetAllTrainers();
        bool CreateTrainer(CreateTrainerViewModel createTrainer);
        TrainerViewModel? GetTrainerDetails(int TrainerId);
        UpdateTrainerViewModel? GetTrainerToUpdate(int TrainerId);
        bool UpdateTrainer(int TrainerId, UpdateTrainerViewModel updateTrainer);
        bool RemoveTrainer(int TrainerId);
    }
}
