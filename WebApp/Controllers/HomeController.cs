using Microsoft.Reporting.WebForms;
using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : BaseController
    {
        Entities db = new Entities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        //[DllImport("ImageOperations.dll", CallingConvention = CallingConvention.Cdecl)]
        public ActionResult Export()
        {
            LocalReport localReport = new LocalReport()//;
            {
                ReportPath = Server.MapPath("/Report/TestReport.rdlc")
            };

            var user = Session["USER_SESSION"] as UserLogin;
            ReportDataSource rds = new ReportDataSource("DataSetUser", db.Users.ToList());
            localReport.DataSources.Add(rds);
            var abc = new ReportParameter("UserName", user.UserName);
            localReport.SetParameters(abc);

            string reportType = "PDF";
            string mimeType, encoding, fileNameExtension;

            string deviceInfo =
                            "<DeviceInfo>" +
                            "   <OutputFormat>" + reportType + "</OutputFormat>" +
                            "   <PageWidth>8.5in</PageWidth>" +
                            "   <PageHeight>11in</PageHeight>" +
                            "   <MarginTop>0.5in</MarginTop>" +
                            "   <MarginLeft>0in</MarginLeft>" +
                            "   <MarginRight>0in</MarginRight>" +
                            "   <MarginBottom>0in</MarginBottom>" +
                            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            
            byte[] renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding,  out fileNameExtension, out streams, out warnings);

            return File(renderedBytes, mimeType);
        }
    }
}