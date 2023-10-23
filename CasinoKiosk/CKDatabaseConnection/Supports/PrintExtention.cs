
using CKDatabaseConnection.DAO;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Printing;

namespace CKDatabaseConnection.Supports
{
    public static class PrintExtention
    {
        
        public static void Print(LocalReport report)
        {
            var pageSettings = new PageSettings();
            pageSettings.PaperSize = report.GetDefaultPageSettings().PaperSize;
            pageSettings.Landscape = report.GetDefaultPageSettings().IsLandscape;
            pageSettings.Margins = report.GetDefaultPageSettings().Margins;
            Print(report, pageSettings);
        }

        public static void Print(this LocalReport report, PageSettings pageSettings)
        {
            string deviceInfo =
                $@"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>{pageSettings.PaperSize.Width * 100}in</PageWidth>
                <PageHeight>{pageSettings.PaperSize.Height * 100}in</PageHeight>
                <MarginTop>{pageSettings.Margins.Top * 100}in</MarginTop>
                <MarginLeft>{pageSettings.Margins.Left * 100}in</MarginLeft>
                <MarginRight>{pageSettings.Margins.Right * 100}in</MarginRight>
                <MarginBottom>{pageSettings.Margins.Bottom * 100}in</MarginBottom>
            </DeviceInfo>";

            Warning[] warnings;
            var streams = new List<Stream>();
            var currentPageIndex = 0;

            report.Render("Image", deviceInfo,
                (name, fileNameExtension, encoding, mimeType, willSeek) =>
                {
                    var stream = new MemoryStream();
                    streams.Add(stream);
                    return stream;
                }, out warnings);

            foreach (Stream stream in streams)
                stream.Position = 0;

            if (streams == null || streams.Count == 0)
                throw new Exception("Error: no stream to print.");

            var printDocument = new PrintDocument();
            printDocument.DefaultPageSettings = pageSettings;
            if (!printDocument.PrinterSettings.IsValid)
                throw new Exception("Error: cannot find the default printer.");
            else
            {
                printDocument.PrintPage += (sender, e) =>
                {
                    Metafile pageImage = new Metafile(streams[currentPageIndex]);
                    Rectangle adjustedRect = new Rectangle(
                        e.PageBounds.Left - (int)e.PageSettings.HardMarginX,
                        e.PageBounds.Top - (int)e.PageSettings.HardMarginY,
                        e.PageBounds.Width,
                        e.PageBounds.Height);
                    e.Graphics.FillRectangle(Brushes.White, adjustedRect);
                    e.Graphics.DrawImage(pageImage, adjustedRect);
                    //currentPageIndex++;
                    //e.HasMorePages = (currentPageIndex < streams.Count);
                    e.Graphics.DrawRectangle(Pens.Red, adjustedRect);
                };
                printDocument.EndPrint += (Sender, e) =>
                {
                    if (streams != null)
                    {
                        foreach (Stream stream in streams)
                            stream.Close();
                        streams = null;
                    }
                };
                printDocument.PrinterSettings.PrinterName = System.Configuration.ConfigurationManager.AppSettings["printerName"];
                printDocument.Print();
               
            }
        }

        public static LocalReport ExportLocalReport(LocalReport report, int ID)
        {
            UserDao dao = new UserDao();
            DataTable dt = dao.GetData(ID);

            //ReportParameter reportParam1;

            //ReportParameter[] reportParameters = new ReportParameter[1];
            //reportParam1 = new ReportParameter("ReportParameter1", reportTitle);
            //reportParameters[0] = reportParam1;

            ReportDataSource rds = new ReportDataSource("CasinoTGSGetLogByID", dt);
            report.DataSources.Add(rds);
            report.ReportPath = @"Assets\Reports\CasinoTGSTicket.rdlc";
            //report.SetParameters(reportParam1);
            return report;
        }

        public static LocalReport ExportLocalReportTicketPromotion(LocalReport report, int ID)
        {
            UserDao dao = new UserDao();
            DataTable dt = dao.GetDataTicketPromotion(ID);

            //ReportParameter reportParam1;

            //ReportParameter[] reportParameters = new ReportParameter[1];
            //reportParam1 = new ReportParameter("ReportParameter1", reportTitle);
            //reportParameters[0] = reportParam1;

            ReportDataSource rds = new ReportDataSource("TicketPromotionByPID", dt);
            report.DataSources.Add(rds);
            report.ReportPath = @"Assets\Reports\HTRTicketPromotion.rdlc";
            //report.SetParameters(reportParam1);
            return report;
        }

        //Add 20230413 Hantt start
        public static LocalReport ExportReportGoldenHour(LocalReport report, int ticketID)
        {
            UserDao dao = new UserDao();
            DataTable dt = dao.GetDataGoldenHourByID(ticketID);

            ReportDataSource rds = new ReportDataSource("HTRGoldenHourPromotionByID", dt);
            report.DataSources.Add(rds);
            report.ReportPath = @"Assets\Reports\HTRGoldenHourPromotion.rdlc";
            report.ReportPath = "Assets/Reports/HTRGoldenHourPromotion.rdlc";
            return report;
        }
        //Add 20230413 Hantt end

        public static void SavePDF(LocalReport report, string filename)
        {
            System.IO.Directory.CreateDirectory("C:\\ProgramData\\CasinoTGSReprint");

            byte[] Bytes = report.Render(format: "PDF", deviceInfo: "");

            using (FileStream stream = new FileStream("C:\\ProgramData\\CasinoTGSReprint\\" + filename + ".pdf", FileMode.Create))
            {
                stream.Write(Bytes, 0, Bytes.Length);
            }
        }

        public static void SavePDFTicketPromotion(LocalReport report, string filename)
        {
            System.IO.Directory.CreateDirectory("C:\\ProgramData\\TicketPromotion");

            byte[] Bytes = report.Render(format: "PDF", deviceInfo: "");

            using (FileStream stream = new FileStream("C:\\ProgramData\\TicketPromotion\\" + filename + ".pdf", FileMode.Create))
            {
                stream.Write(Bytes, 0, Bytes.Length);
            }
        }
    }
}
