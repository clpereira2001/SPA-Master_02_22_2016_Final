using System.Web.Mvc;
using System.Web.UI;
using Vauction.Models;
using Vauction.Utils;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Autorization;

namespace Vauction.Controllers
{
  [HandleError, CrossSessionCheck]
  public class EventController : BaseController
  {
    IEventRepository EventRepository;
    public EventController()
    {
      EventRepository = dataProvider.EventRepository;
    }

    #region Index
    //Index
    [Compress, NoCache]
    public ActionResult Index()
    {
      InitCurrentEvent();
      return View();
    }
    //pIndex
    [ChildActionOnly] //ActionOutputCache(Consts.CachingTime_30min)
    public ActionResult pIndex(long event_id, bool isa, bool iscurrent)
    {      
      ViewData["EventID"] = event_id;
      return View("PartialEventIndex", EventRepository.GetUpcomingList(isa));
    }
    #endregion

    [VauctionAuthorize, RequireSslFilter(IsRequired = "false"), Compress, HttpGet, NoCache]
    public ActionResult Register(int? id, long? id_auction)
    {
      if (!id.HasValue) return RedirectToAction("General", "Error");      
      EventRepository.RegisterUserForEvent(id.GetValueOrDefault(0), AppHelper.CurrentUser.ID);
      if (id_auction.HasValue)
        return RedirectToAction("AuctionDetail", "Auction", new { id=id_auction.Value });
      InitCurrentEvent();
      return View(); 
    }

  }
}
