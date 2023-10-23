using CKDatabaseConnection.DAO;
using CKDatabaseConnection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasinoKiosk.Controllers
{
    public class FBCasinoController : Controller
    {
        // GET: Admin/FBCasino
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FBCasinoManagement()
        {
            if (Session["userName"] == null)
                return RedirectToAction("LogIn", "Account");
            return View(getitemlist());
        }

        public ActionResult splitItem(string ticketid, string splitvalue)
        {
            var lstitem = new List<FBItem>();
            var splitBy = Session["userName"].ToString();
            try
            {
                var fbfunction = new FBCasino();
                if (HttpContext.Request.Form["inputSplitValue"] != null && HttpContext.Request.Form["inputSplitValue"] != ""
                   && HttpContext.Request.Form["txtTicketID"] != null && HttpContext.Request.Form["txtTicketID"].ToString() != ""
                   && HttpContext.Request.Form["cbOutlets"] != "All"
                   && HttpContext.Request.Form["txtplayerid"] != null && HttpContext.Request.Form["txtplayerid"] != ""
                    )
                {
                    var ticketID = Convert.ToInt32(HttpContext.Request.Form["txtTicketID"].ToString());
                    var splitValue = Convert.ToInt32(HttpContext.Request.Form["inputSplitValue"].ToString());
                    var itemNumber = HttpContext.Request.Form["cbOutlets"].ToString();
                    var playerid = HttpContext.Request.Form["txtplayerid"].ToString();
                    //check point
                    var rs = fbfunction.checkPointByID(ticketID.ToString(), splitValue.ToString());
                    if (rs != 0)
                    {
                        fbfunction.splitItem(ticketID, splitValue, splitBy);
                        fbfunction.compFB(int.Parse(playerid), int.Parse(itemNumber), splitValue, "", splitBy);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Response.Redirect("FBCasinoManagement");
            return View("FBCasinoManagement", getitemlist());
        }


        public JsonResult viewDetail(requestDetailID rqDetailo)
        {
            var lstitem = new List<FBSplitItem>();
            try
            {
                var fbfunction = new FBCasino();
                lstitem = fbfunction.getFBSplitItems(rqDetailo.ticketid);
            }
            catch (Exception ex)
            { }
            return Json(new JsonResult()
            {
                Data = lstitem
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult checkTicketValue(requestDetailID rqDetailo)
        {
            var lstitem = 0;
            try
            {
                var fbfunction = new FBCasino();
                lstitem = fbfunction.checkPointByID(rqDetailo.ticketid.ToString(), rqDetailo.splitvalue.ToString());
            }
            catch (Exception ex)
            { }
            return Json(new JsonResult()
            {
                Data = lstitem
            }, JsonRequestBehavior.AllowGet);
        }

        private List<FBItem> getitemlist()
        {
            var lstitem = new List<FBItem>();
            try
            {
                var fbfunction = new FBCasino();
                if (HttpContext.Request.Form["ID"] != null && (HttpContext.Request.Form["ID"] != ""))
                {
                    var playerID = Convert.ToInt32(HttpContext.Request.Form["ID"].ToString());
                    lstitem = fbfunction.getFBItemsByDate(playerID.ToString());
                }
                else
                if (HttpContext.Request.Form["dfromdate"] != null
                    && HttpContext.Request.Form["dfromdate"] != ""
                    && HttpContext.Request.Form["dtodate"] != null
                    && HttpContext.Request.Form["dtodate"] != "")
                {
                    var fdate = Convert.ToString(HttpContext.Request.Form["dfromdate"]);
                    var tdate = Convert.ToString(HttpContext.Request.Form["dtodate"]);
                    lstitem = fbfunction.getFBItemsByDate("", fdate, tdate);
                }
                else
                {
                    lstitem = fbfunction.getFBItemsByDate("");
                }
            }
            catch (Exception ex)
            { }
            return lstitem;
        }



    }
}