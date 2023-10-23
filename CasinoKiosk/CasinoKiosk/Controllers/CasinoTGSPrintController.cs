using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CasinoKiosk.Controllers
{
    public class CasinoTGSPrintController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ExportToPDF()
        {
            //Report  
            ReportViewer reportViewer = new ReportViewer();

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Server.MapPath("~/Assets/Reports/TicketMFDailyBonus.rdlc");

            //Byte  
            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, filenameExtension;

            byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            //File  
            string FileName = "Test_" + DateTime.Now.Ticks.ToString() + ".pdf";
            string FilePath = HttpContext.Server.MapPath("C:/temp/") + FileName;

            //create and set PdfReader  
            PdfReader reader = new PdfReader(bytes);
            FileStream output = new FileStream(FilePath, FileMode.Create);

            string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

            //create and set PdfStamper  
            PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);

            if (Agent.Contains("Firefox"))
                pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
            else
                pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

            pdfStamper.FormFlattening = false;
            pdfStamper.Close();
            reader.Close();

            //return file path  
            string FilePathReturn = @"TempFiles/" + FileName;
            return Content(FilePathReturn);
        }
    }
}