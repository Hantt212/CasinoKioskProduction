using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Areas.Admin.Controllers
{
    public class PlayersController : Controller
    {
        // GET: Admin/Players
        PlayerDao dao = new PlayerDao();

        //[Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult Index()
        {
            return View();
        }

       
        public JsonResult List()
        {
            return Json(dao.ListAllPlayers(), JsonRequestBehavior.AllowGet);
        }
   
        //[Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult PlayerList(int page = 1)
        {
            var dao = new PlayerDao();
            //this.Session["page"] = page;
            var model = dao.ListPlayersDetail(page, 100);
            return View(model);
        }
       
        public JsonResult Add(CIFE_Players player)
        {
            return Json(dao.Insert(player), JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetbyID(int ID)
        {
            var player = dao.ListAllPlayers().Find(x => x.ID.Equals(ID));
            return Json(player, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult Update(CIFE_Players player)
        {
            return Json(dao.Update(player), JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult Delete(int ID)
        {
            return Json(dao.Delete(ID), JsonRequestBehavior.AllowGet);
        }
    }
}