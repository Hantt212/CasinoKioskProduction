using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using CKDatabaseConnection.Models;
using iTextSharp.text.pdf;

using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class CasinoTGSController : Controller
    {
        UserDao dao = new UserDao();
        ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities();

        // GET: CasinoTGS
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult Index()
        {
            using (ITHoTram_CustomReportEntities entity = new ITHoTram_CustomReportEntities())
            {
                List<TGSModel> model = new List<TGSModel>();

                foreach (var player in entity.CasinoTheGrandSignaturePlayerQualifieds.Select(e => new { e.PID, e.PlayerName }).Distinct())
                {
                    model.Add(new TGSModel
                    {
                        PID = player.PID,
                        PlayerName = player.PlayerName,
                        CasinoTGSPlayers = entity.CasinoTheGrandSignaturePlayerQualifieds.Where(p => p.PID == player.PID).ToList()
                    });
                }

                return View(model);
            }
        }
        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public JsonResult FindTGSPlayerByID(int id)
        {
            var user = dao.FindTGSPlayerByID(id);
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public JsonResult Redeem(CasinoTheGrandSignaturePlayerQualified player)
        {                             
            return Json(dao.RedemmTGS(player), JsonRequestBehavior.AllowGet);                       
        }               
    }
}