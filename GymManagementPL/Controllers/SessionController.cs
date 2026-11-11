using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementSystemBLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SessionController(ISessionService _sessionService) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(Sessions);
        }

        public async Task<IActionResult> SessionDetails(int Id, CancellationToken ct)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Session can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Session = await _sessionService.GetSessionByIdAsync(Id, ct);

            if(Session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(Session);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await LoadDropDownForCategoriesAsync(ct);
            await LoadDropDownForTrainersAsync(ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel createSession, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropDownForTrainersAsync(ct);
                await LoadDropDownForCategoriesAsync(ct);
                return View(createSession);
            }

            var result = await _sessionService.CreateSessionAsync(createSession, ct);

            if (result)
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Session Failed To Create";
                await LoadDropDownForCategoriesAsync(ct);
                await LoadDropDownForTrainersAsync(ct);
                return View(createSession);
            }
        }

        public async Task<IActionResult> Edit(int Id, CancellationToken ct)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "Id of Session can`t 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Session = await _sessionService.GetSessionToUpdateAsync(Id, ct);
            await LoadDropDownForTrainersAsync();

            if (Session is null)
            {
                TempData["ErrorMessage"] = "Session Can not be Updated";
                return RedirectToAction(nameof(Index));
            }

            return View(Session);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, UpdateSessionViewModel updateSession, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropDownForTrainersAsync(ct);
                return View(updateSession);
            }

            var result = await _sessionService.UpdateSessionAsync(Id, updateSession, ct);

            if (result)
                TempData["SuccessMessage"] = "Session Updated Successfully";
            else
                TempData["ErrorMessage"] = "Session Failed To Update";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id, CancellationToken ct)
        {
            var Session = await _sessionService.GetSessionByIdAsync(Id, ct);
            if(Session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = Session.Id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int Id, CancellationToken ct)
        {
            var result = await _sessionService.RemoveSessionAsync(Id, ct);
            if (result)
                TempData["SuccessMessage"] = "Session Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Session Can not be Deleted";

            return RedirectToAction(nameof(Index));
        }

        #region Helper Method
        private async Task LoadDropDownForCategoriesAsync(CancellationToken ct = default)
        {
            var Categories = await _sessionService.GetCategoryForDropDownAsync(ct);
            ViewBag.Categories = new SelectList(Categories,"Id","Name");    
        }

        private async Task LoadDropDownForTrainersAsync(CancellationToken ct = default)
        {
            var Trainers = await _sessionService.GetTrainerForDropDownAsync(ct);
            ViewBag.Trainers = new SelectList(Trainers,"Id","Name");
        }
        #endregion
    }
}
