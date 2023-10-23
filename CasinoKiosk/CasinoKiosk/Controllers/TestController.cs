using CKDatabaseConnection.DAO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Areas.Admin.Controllers
{
    public class TestController : Controller
    {
        // GET: Admin/Test

        string connectionString = ConfigurationManager.ConnectionStrings["CKdbContext"].ConnectionString;
        CKFunction function = new CKFunction();

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult GetPoints()
        {
            var dao = new TestDao();
            WebServiceCasino.WebServiceCasino wsc = new WebServiceCasino.WebServiceCasino();
            //wsc.GetPoints(playerID);

            var model = dao.ListAllPagingPoints(1, 50, 0);
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult GetPoints(int ID)
        {
            if (HttpContext.Request.Form["ID"] != null)
            {
                ID = Convert.ToInt32(HttpContext.Request.Form["ID"]);
                WebServiceCasino.WebServiceCasino wsc = new WebServiceCasino.WebServiceCasino();
                wsc.GetPoints(ID);
                var dao = new TestDao();
                var model = dao.ListAllPagingPoints(1, 50, ID);
                return View(model);
            }
            else
            {
                var dao = new TestDao();
                var model1 = dao.ListAllPagingPoints(1, 50, 0);
                return View(model1);
            }
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult GetPlayerInfo()
        {
            var dao = new TestDao();

            var model = dao.ListAllPagingPlayerInfo(1, 50, 0, "");
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        [HttpPost]
        public ActionResult GetPlayerInfo(int ID, string IDNumber)
        {
            if (HttpContext.Request.Form["ID"] != null && HttpContext.Request.Form["IDNumber"] != null)
            {
                ID = Convert.ToInt32(HttpContext.Request.Form["ID"].ToString().Trim());
                IDNumber = HttpContext.Request.Form["IDNumber"].ToString().Trim();
                
                var dao = new TestDao();
                var model = dao.ListAllPagingPlayerInfo(1, 100, ID, IDNumber);
                return View(model);
            }
            else
            {
                var dao = new TestDao();
                var model1 = dao.ListAllPagingPoints(1, 100, 0);
                return View(model1);
            }
        }
    }
}