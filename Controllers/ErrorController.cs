using System;
using System.Web.Mvc;
using Vauction.Utils;
using Vauction.Utils.Autorization;

namespace Vauction.Controllers
{
  [CrossSessionCheck]
  public class ErrorController : BaseController
  {
    public ActionResult HttpError()
    {
      return RedirectToAction("HttpError404", "Error");
    }

    public ActionResult HttpError404()
    {
      try
      {
        SessionUser cuser = AppHelper.CurrentUser;
        InitCurrentEvent();
        ViewData["UpcomingEvents"] =
          dataProvider.EventRepository.GetUpcomingList(Request.IsAuthenticated && cuser != null && cuser.IsAccessable);
        Response.StatusCode = 404;
      }
      catch (Exception ex)
      {
        ViewData["UpcomingEvents"] = ViewData["CurrentEvent"] = null;
        ViewData["UserRegisterForEvent"] = false;
      }
      return View();
    }

    public ActionResult HttpError500(string error)
    {
      ViewData["Title"] = "Sorry, an error occurred while processing your request. (500)";
      Response.StatusCode = 500;
      return View("Error");
    }

    public ActionResult General(string error)
    {
      ViewData["Title"] = "Sorry, an error occurred while processing your request.";
      return View("Error");
    }

  }
}
