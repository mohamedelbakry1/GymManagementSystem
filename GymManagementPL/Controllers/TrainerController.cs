using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.TrianerViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class TrainerController(ITrainerService _trainerService) : Controller
    {
        public IActionResult Index()
        {
            var Trainers = _trainerService.GetAllTrainers();

            return View(Trainers);
        }

        public IActionResult TrainerDetails(int Id)
        {
            if(Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Trainer can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Trainer = _trainerService.GetTrainerDetails(Id);

            if(Trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(Trainer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTrainer(CreateTrainerViewModel createTrainer)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataInvalid", "Check Data and Missing Fields");
                return RedirectToAction(nameof(Create), createTrainer);
            }

            var result = _trainerService.CreateTrainer(createTrainer);

            if (result)
                TempData["SuccessMessage"] = "Trainer Created Successfully";
            else
                TempData["ErrorMessage"] = "Trainer Failed To Create";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult TrainerEdit(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Trainer can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Trainer = _trainerService.GetTrainerToUpdate(Id);

            if (Trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(Trainer);
        }

        [HttpPost]
        public IActionResult TrainerEdit([FromRoute] int Id, UpdateTrainerViewModel updateTrainer)
        {
            if (!ModelState.IsValid)
                return View(updateTrainer);

            var result = _trainerService.UpdateTrainer(Id, updateTrainer);

            if (result)
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
            else
                TempData["ErrorMessage"] = "Trainer Failed To Update";

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Trainer can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Trainer = _trainerService.GetTrainerDetails(Id);

            if (Trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TrainerId = Trainer.Id;
            return View();
        }

        [HttpPost]
        public IActionResult DeleteConfirmed([FromForm] int Id)
        {
            var result = _trainerService.RemoveTrainer(Id);

            if (result)
                TempData["SuccessMessage"] = "Trainer Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Trainer Can not Delete";

            return RedirectToAction(nameof(Index));
        }
    }
}
