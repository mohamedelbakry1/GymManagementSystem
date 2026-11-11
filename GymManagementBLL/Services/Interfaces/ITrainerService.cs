using GymManagementBLL.ViewModels.TrianerViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel createTrainer, CancellationToken ct = default);
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int TrainerId, CancellationToken ct = default);
        Task<UpdateTrainerViewModel?> GetTrainerToUpdateAsync(int TrainerId, CancellationToken ct = default);
        Task<bool> UpdateTrainerAsync(int TrainerId, UpdateTrainerViewModel updateTrainer, CancellationToken ct = default);
        Task<bool> RemoveTrainerAsync(int TrainerId, CancellationToken ct = default);
    }
}
