using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementSystemBLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class SessionController(ISessionService _sessionService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var Sessions = await _sessionService.GetAllSessions();
            return View(Sessions);
        }

        public async Task<IActionResult> SessionDetails(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Session can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Session = await _sessionService.GetSessionById(Id);

            if(Session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(Session);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropDownForCategories();
            await LoadDropDownForTrainers();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel createSession)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropDownForTrainers();
                await LoadDropDownForCategories();
                return View(createSession);
            }

            var result = await _sessionService.CreateSession(createSession);

            if (result)
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Session Failed To Create";
                await LoadDropDownForCategories();
                await LoadDropDownForTrainers();
                return View(createSession);
            }
        }

        public async Task<IActionResult> Edit(int Id)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Session can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Session = await _sessionService.GetSessionToUpdate(Id);
            await LoadDropDownForTrainers();

            if (Session is null)
            {
                TempData["ErrorMessage"] = "Session Can not be Updated";
                return RedirectToAction(nameof(Index));
            }

            return View(Session);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, UpdateSessionViewModel updateSession)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropDownForTrainers();
                return View(updateSession);
            }

            var result = await _sessionService.UpdateSession(Id, updateSession);

            if (result)
                TempData["SuccessMessage"] = "Session Updated Successfully";
            else
                TempData["ErrorMessage"] = "Session Failed To Update";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var Session = await _sessionService.GetSessionById(Id);
            if(Session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = Session.Id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            var result = await _sessionService.RemoveSession(Id);
            if (result)
                TempData["SuccessMessage"] = "Session Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Session Can not be Deleted";

            return RedirectToAction(nameof(Index));
        }

        #region Helper Method
        private async Task LoadDropDownForCategories()
        {
            var Categories = await _sessionService.GetCategoryForDropDown();
            ViewBag.Categories = new SelectList(Categories,"Id","Name");    
        }

        private async Task LoadDropDownForTrainers()
        {
            var Trainers = await _sessionService.GetTrainerForDropDown();
            ViewBag.Trainers = new SelectList(Trainers,"Id","Name");
        }
        #endregion
    }
}
