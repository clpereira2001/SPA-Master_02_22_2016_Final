using System;
using System.Web.Mvc;
using Vauction.Models;
using Vauction.Utils;

namespace Vauction.Controllers
{
    public class TrckController : BaseController
    {
      #region init
      private IVariableRepository VariableRepository;
      public TrckController()
      {
        VariableRepository = dataProvider.VariableRepository;
      }
      #endregion

      //TrckEmail
      public ActionResult TrckEmail(string event_id, long? user_id, string type)
      {
        VariableRepository.TrackEmail(String.Empty, event_id, user_id, type);
        return RedirectToAction("Image", "Zip", new { path = "/public/images/1px.png" });
      }

      //TrckFU
      public ActionResult TrckFU(string event_id, string url, long? user_id, string type)
      {
        if (!String.IsNullOrEmpty(url))
        {
          VariableRepository.TrackForwardingURL(Consts.UsersIPAddress, event_id, url, user_id, type);
          return Redirect(url);
        }
        return RedirectToAction("Index", "Home");
      }

    }
}
