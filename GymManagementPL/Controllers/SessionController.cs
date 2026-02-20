using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementSystemBLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class SessionController(ISessionService _sessionService) : Controller
    {
        public IActionResult Index()
        {
            var Sessions = _sessionService.GetAllSessions();
            return View(Sessions);
        }

        public IActionResult SessionDetails(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Session can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Session = _sessionService.GetSessionById(Id);

            if(Session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(Session);
        }

        public IActionResult Create()
        {
            LoadDropDownForCategories();
            LoadDropDownForTrainers();
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateSessionViewModel createSession)
        {
            if (!ModelState.IsValid)
            {
                LoadDropDownForTrainers();
                LoadDropDownForCategories();
                return View(createSession);
            }

            var result = _sessionService.CreateSession(createSession);

            if (result)
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Session Failed To Create";
                LoadDropDownForCategories();
                LoadDropDownForTrainers();
                return View(createSession);
            }
        }

        public IActionResult Edit(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Session can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Session = _sessionService.GetSessionToUpdate(Id);
            LoadDropDownForTrainers();

            if (Session is null)
            {
                TempData["ErrorMessage"] = "Session Can not be Updated";
                return RedirectToAction(nameof(Index));
            }

            return View(Session);
        }

        [HttpPost]
        public IActionResult Edit(int Id, UpdateSessionViewModel updateSession)
        {
            if (!ModelState.IsValid)
            {
                LoadDropDownForTrainers();
                return View(updateSession);
            }

            var result = _sessionService.UpdateSession(Id, updateSession);

            if (result)
                TempData["SuccessMessage"] = "Session Updated Successfully";
            else
                TempData["ErrorMessage"] = "Session Failed To Update";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int Id)
        {
            var Session = _sessionService.GetSessionById(Id);
            if(Session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = Session.Id;
            return View();
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int Id)
        {
            var result = _sessionService.RemoveSession(Id);
            if (result)
                TempData["SuccessMessage"] = "Session Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Session Can not be Deleted";

            return RedirectToAction(nameof(Index));
        }

        #region Helper Method
        private void LoadDropDownForCategories()
        {
            var Categories = _sessionService.GetCategoryForDropDown();
            ViewBag.Categories = new SelectList(Categories,"Id","Name");    
        }

        private void LoadDropDownForTrainers()
        {
            var Trainers = _sessionService.GetTrainerForDropDown();
            ViewBag.Trainers = new SelectList(Trainers,"Id","Name");
        }
        #endregion
    }
}
