using CKDatabaseConnection.DAO;
using CKDatabaseConnection.EF;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CasinoKiosk.Areas.Admin.Controllers
{
    public class ItemsController : Controller
    {
        // GET: Admin/Items

        [Authorize(Roles = "SuperAdmin, HTRAdmin, HTRStaff")]
        public ActionResult Index(int page = 1, int pageSize = 1000)
        {
            var dao = new ItemDao();
            var model = dao.ListAllPaging(page, pageSize);
            return View(model);

            
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        public ActionResult Edit(int id)
        {
            var item = new ItemDao().ViewDetail(id);
            return View(item);
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        [HttpPost]
        public ActionResult Create(CasinoKiosk_Item item, HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {
                var dao = new ItemDao();
                var db = new CKdbContext();
                var allowedExtensions = new[] {
            ".Jpg", ".png", ".jpg", "jpeg", ".PNG", ".JPG"
        };


                if (db.CasinoKiosk_Item.Any(x => x.ItemName == item.ItemName))
                {
                    ModelState.AddModelError("WarningItemName", "This item name already exists!");
                }

                if (item.ItemName == "" || item.ItemName == null)
                {
                    ModelState.AddModelError("WarningItemName", "Item Name required!");
                }

                if (item.ItemPoints.ToString() == "" || item.ItemPoints < 0)
                {
                    ModelState.AddModelError("WarningItempoints", "Wrong format");
                }

                if (item.Quantity.ToString() == "" || item.Quantity < 0)
                {
                    ModelState.AddModelError("WarningQuantity", "Wrong format");
                }


                if (file == null)
                {
                    ModelState.AddModelError("WarningImageUrl", "Image required");
                }
                if (file != null)
                {

                    //item.imageURL = file.ToString();


                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-filename.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = name + ext; // store the file inside ~/project folder(Images)  
                        Stream strm = file.InputStream;
                        using (var image = System.Drawing.Image.FromStream(strm))
                        {
                            int newWidth = 277; // New Width of Image in Pixel  
                            int newHeight = 143; // New Height of Image in Pixel  
                            var thumbImg = new Bitmap(newWidth, newHeight);
                            var thumbGraph = Graphics.FromImage(thumbImg);
                            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            var imgRectangle = new Rectangle(0, 0, newWidth, newHeight);
                            thumbGraph.DrawImage(image, imgRectangle);

                            var path = Path.Combine(Server.MapPath("~/Assets/Images"), myfile);
                            item.imageURL = Path.GetFileName(file.FileName);

                            thumbImg.Save(path, image.RawFormat);
                            //file.SaveAs(path);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Warning", "Please choose correct image format: .Jpg, .png, .jpg, .jpeg,.PNG !");
                    }

                    long id = dao.Insert(item);
                    if (id > 0)
                    {
                        return RedirectToAction("Index", "Items");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Can not create item.");
                    }


                }


            }
            return View("Create");
        }

        [Authorize(Roles = "SuperAdmin, HTRAdmin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(CasinoKiosk_Item item, HttpPostedFileBase file)
        {
            var dao = new ItemDao();
            var db = new CKdbContext();
            var changeLogDao = new ChangeLogDao();

            var allowedExtensions = new[] {
            ".Jpg", ".png", ".jpg", "jpeg", ".PNG", ".JPG" };
            //var log = new LogDao();
            //var changelog = new ChangeLog();


            if (item.ItemPoints.ToString() == "" || item.ItemPoints < 0)
            {
                ModelState.AddModelError("WarningItemPoints", "Wrong format");
            }

            if (item.Quantity.ToString() == "" || item.ItemPoints < 0)
            {
                ModelState.AddModelError("WarningQuantity", "Wrong format");
            }

            if (ModelState.IsValid)
            {
                CasinoKiosk_Item i = dao.GetItemByID(item.ID);

                if (file == null)
                {
                    item.imageURL = getItemImageUrl(item.ID);
                }
                if (file != null)
                {
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-filename.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = name + ext; // store the file inside ~/project folder(Images)  
                        Stream strm = file.InputStream;
                        using (var image = System.Drawing.Image.FromStream(strm))
                        {
                            int newWidth = 277; // New Width of Image in Pixel  
                            int newHeight = 143; // New Height of Image in Pixel  
                            var thumbImg = new Bitmap(newWidth, newHeight);
                            var thumbGraph = Graphics.FromImage(thumbImg);
                            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            var imgRectangle = new Rectangle(0, 0, newWidth, newHeight);
                            thumbGraph.DrawImage(image, imgRectangle);

                            var path = Path.Combine(Server.MapPath("~/Assets/Images"), myfile);
                            item.imageURL = Path.GetFileName(file.FileName);

                            thumbImg.Save(path, image.RawFormat);
                            //file.SaveAs(path);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Warning", "Please choose correct image format: .Jpg, .png, .jpg, .jpeg,.PNG !");
                    }
                }

                CasinoKiosk_ChangeLog changeLog = new CasinoKiosk_ChangeLog();

                changeLog.ItemID = item.ID;
                changeLog.OldImageURL = i.imageURL;
                changeLog.NewImageUrl = item.imageURL;
                changeLog.OldIsActive = i.isActive;
                changeLog.NewIsActive = item.isActive;
                changeLog.OldName = i.ItemName;
                changeLog.NewName = item.ItemName;
                changeLog.OldPoints = i.ItemPoints;
                changeLog.NewPoints = item.ItemPoints;
                changeLog.OldQuantity = i.Quantity;
                changeLog.NewQuantity = item.Quantity;
                changeLog.ChangedDate = System.DateTime.Now;
                changeLog.ChangedPerson = Session["userName"].ToString();

                changeLogDao.Insert(changeLog);

                bool id = dao.Update(item);

                if (id == true)
                {
                    return RedirectToAction("Index", "Items");
                }
                else
                {
                    ModelState.AddModelError("", "Edit Error.");
                }


                // }

            }
            return View("Edit");
        }

        public string getItemImageUrl(int ID)
        {
            string imageUrl = "";
            DataSet ds;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CKdbContext"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("CasinoKiosk_spGetItemImageUrl", con))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.Add(new SqlParameter("@ID", ID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {

                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        imageUrl = ds.Tables[0].Rows[0]["imageUrl"].ToString();
                    }
                }
            }

            return imageUrl;
        }

    }



}