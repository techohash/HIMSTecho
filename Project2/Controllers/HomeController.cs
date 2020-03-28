using Microsoft.Reporting.WebForms;
using Project2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;

namespace Project2.Controllers
{
    public class HomeController : Controller
    {
        PathologyEntities db = new PathologyEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult User_Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult User_Login(TBL_USER_D objUser)
        {

            var user = db.TBL_USER_D.Where(x => x.USER_ID == objUser.USER_ID && x.USER_PASSWORD == objUser.USER_PASSWORD).FirstOrDefault();
            if (user != null)
            {
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                ViewBag.Message = "Unsuccessfull!...Try Again";
                return View();
            }
        }
        public ActionResult AdminDashboard()
        {
            var user = db.TBL_USER_D.ToList();
            return View(user);
        }

        public ActionResult Add_Departments()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add_Departments(TBL_R_DEPT objdept)
        {
            db.TBL_R_DEPT.Add(objdept);
            db.SaveChanges();
            return RedirectToAction("Departments");
        }
        public ActionResult Departments()
        {
            var dept = db.TBL_R_DEPT.ToList();
            return View(dept);
        }


        [HttpGet]
        public ActionResult Edit_Departments(int id)
        {
            var dept = db.TBL_R_DEPT.Find(id);
            return View(dept);
        }
        [HttpPost]
        public ActionResult Edit_Departments(TBL_R_DEPT objdept)
        {
            db.Entry(objdept).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Departments");
        }

        public ActionResult Delete_Departments(int id)
        {
            var dept = db.TBL_R_DEPT.Find(id);

            db.TBL_R_DEPT.Remove(dept);
            db.SaveChanges();
            return RedirectToAction("Departments");
        }
        [HttpGet]
        public ActionResult Patient_Visit()
        {
            ViewBag.departments = new SelectList(db.TBL_R_DEPT, "DEPT_ID", "DEPT_NAME");
            ViewBag.consultants = new SelectList(db.TBL_CONSULTANT, "CON_JOB_ID", "CON_NAME");
            return View();
        }
        [HttpPost]
        public ActionResult Patient_Visit(TBL_R_RECEIPT objRecept)
        {
            ViewBag.departments = new SelectList(db.TBL_R_DEPT, "DEPT_ID", "DEPT_NAME");
            ViewBag.consultants = new SelectList(db.TBL_CONSULTANT, "CON_JOB_ID", "CON_NAME");

            TBL_R_PATIENT patient = new TBL_R_PATIENT();
            patient.AGE = objRecept.TBL_R_PATIENT.AGE;
            patient.FIRST_NAME = objRecept.TBL_R_PATIENT.FIRST_NAME;
            patient.ADDRESS1 = objRecept.TBL_R_PATIENT.ADDRESS1;
            patient.TEL_NO = objRecept.TBL_R_PATIENT.TEL_NO;
            patient.GENDER = objRecept.TBL_R_PATIENT.GENDER;
            patient.F_NAME = objRecept.TBL_R_PATIENT.F_NAME;
            patient.CNIC_NO = objRecept.TBL_R_PATIENT.CNIC_NO;
            patient.DATEOFBIRTH = objRecept.TBL_R_PATIENT.DATEOFBIRTH;

            db.TBL_R_PATIENT.Add(patient);
            db.SaveChanges();

            int last_insert_id = patient.MR_NO;

            TBL_R_RECEIPT recept = new TBL_R_RECEIPT();
            recept.MR_NO = last_insert_id;
            recept.DEPT_ID = objRecept.DEPT_ID;
            recept.CONSULTANT_ID = objRecept.CONSULTANT_ID;
            db.TBL_R_RECEIPT.Add(recept);
            db.SaveChanges();

            int last_insert_idd = recept.R_ID;
            return RedirectToAction("reporting", new { id = "pdf", sub = last_insert_idd });

        }

        public ActionResult List_Patients()
        {
            var patient = db.TBL_R_RECEIPT.ToList();
            return View(patient);
        }

        [HttpPost]
        public ActionResult List_Patients(String searchTxt)
        {
            var patient = db.TBL_R_RECEIPT.ToList();
            if (searchTxt != null)
            {
                patient = db.TBL_R_RECEIPT.Where(x => x.MR_NO.ToString().Equals(searchTxt) || x.TBL_R_PATIENT.FIRST_NAME.Contains(searchTxt)||x.TBL_R_PATIENT.TEL_NO.Contains(searchTxt)).ToList();
                
            }
            return View(patient);
        }

        [HttpGet]
        public ActionResult Edit_Patient(int id)
        {
            ViewBag.departments = new SelectList(db.TBL_R_DEPT, "DEPT_ID", "DEPT_NAME");
            ViewBag.consultants = new SelectList(db.TBL_CONSULTANT, "CON_JOB_ID", "CON_NAME");

            var rcpt = db.TBL_R_RECEIPT.Find(id);
            if(rcpt!= null)
            {
                TempData["Patient_ID"] = rcpt.MR_NO;
                TempData.Keep();
                TempData["Reciept_Id"] = rcpt.R_ID;
                TempData.Keep();
                return View(rcpt);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Edit_Patient(TBL_R_RECEIPT objrcpt)
        {
            ViewBag.departments = new SelectList(db.TBL_R_DEPT, "DEPT_ID", "DEPT_NAME");
            ViewBag.consultants = new SelectList(db.TBL_CONSULTANT, "CON_JOB_ID", "CON_NAME");
            
            Int32 Patient_Id = (int)TempData["Patient_ID"];
            var Patient_Data = db.TBL_R_PATIENT.Where(x => x.MR_NO == Patient_Id).FirstOrDefault();
            if (Patient_Data != null)
            {
                Patient_Data.FIRST_NAME = objrcpt.TBL_R_PATIENT.FIRST_NAME;
                Patient_Data.AGE = objrcpt.TBL_R_PATIENT.AGE;
                Patient_Data.TEL_NO = objrcpt.TBL_R_PATIENT.TEL_NO;
                Patient_Data.ADDRESS1 = objrcpt.TBL_R_PATIENT.ADDRESS1;
                Patient_Data.DATEOFBIRTH = objrcpt.TBL_R_PATIENT.DATEOFBIRTH;
                Patient_Data.F_NAME = objrcpt.TBL_R_PATIENT.F_NAME;
                Patient_Data.CNIC_NO = objrcpt.TBL_R_PATIENT.CNIC_NO;
                db.Entry(Patient_Data).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                int last_insert_id = Patient_Data.MR_NO;

                Int32 Reciept_Id = (int)TempData["Reciept_Id"];
                var Reciept_Data = db.TBL_R_RECEIPT.Where(x => x.R_ID == Reciept_Id).FirstOrDefault();
                Reciept_Data.MR_NO = last_insert_id;
                Reciept_Data.DEPT_ID = objrcpt.DEPT_ID;
                Reciept_Data.CONSULTANT_ID = objrcpt.CONSULTANT_ID;
                db.Entry(Reciept_Data).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                int last_insert_idd = objrcpt.R_ID;
                return RedirectToAction("reporting", new { id = "pdf", sub = last_insert_idd });
            }
            else
            {
                return RedirectToAction("List_Patients");
            }
        }


        public ActionResult Add_Consultants()
        {
            ViewBag.departments = new SelectList(db.TBL_R_DEPT, "DEPT_ID", "DEPT_NAME");
            return View();
        }

        [HttpPost]
        public ActionResult Add_Consultants(TBL_CONSULTANT objConslt)
        {
            ViewBag.departments = new SelectList(db.TBL_R_DEPT, "DEPT_ID", "DEPT_NAME");

            db.TBL_CONSULTANT.Add(objConslt);
            db.SaveChanges();
            return RedirectToAction("Consultants");
        }

        public ActionResult Consultants()
        {
            var conslt = db.TBL_CONSULTANT.ToList();
            return View(conslt);
        }


        [HttpGet]
        public ActionResult Edit_Consultants(int id)
        {
            ViewBag.departments = new SelectList(db.TBL_R_DEPT, "DEPT_ID", "DEPT_NAME");

            var conslt = db.TBL_CONSULTANT.Find(id);
            return View(conslt);
        }
        [HttpPost]
        public ActionResult Edit_Consultants(TBL_CONSULTANT objConslt)
        {
            ViewBag.departments = new SelectList(db.TBL_R_DEPT, "DEPT_ID", "DEPT_NAME");

            db.Entry(objConslt).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Consultants");
        }

        public ActionResult Delete_Consultants(int id)
        {
            var conslt = db.TBL_CONSULTANT.Find(id);

            db.TBL_CONSULTANT.Remove(conslt);
            db.SaveChanges();
            return RedirectToAction("Consultants");
        }
        public ActionResult reporting(int sub,string id)
        {
            LocalReport lr = new LocalReport();
            String path = Path.Combine(Server.MapPath("~/Report"), "Report_Patient.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("index");
            }
            List<View_Visit> att = new List<View_Visit>();
            using (PathologyEntities entities = new PathologyEntities())
            {
                att = entities.View_Visit.Where(a => a.R_ID == sub).ToList();
            }
            ReportDataSource rds = new ReportDataSource("DataSet1", att);
            lr.DataSources.Add(rds);
            string reporttype = id;
            string minetype;
            string encoding;
            string filenameextension = id;
            string deviceinfo =
            "<DeviceInfo>" +
                "<OutputFormat>DD</OutputFormat>" +
                "<PageWidth>8.5in</PageWidth>" +
                "<PageHeight>11in</PageHeight>" +
                "<MarginTop>0.5in</MarginTop>" +
                "<MarginLeft>11in</MarginLeft>" +
                "<MarginRight>11in</MarginRight>" +
                "<MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warning;
            string[] stream;
            byte[] renderedbytes;
            renderedbytes = lr.Render("PDF", deviceinfo, out minetype, out encoding, out filenameextension, out stream, out warning);
            return File(renderedbytes, minetype);
        }
    }
}