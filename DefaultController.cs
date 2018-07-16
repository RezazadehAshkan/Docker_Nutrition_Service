using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ContainerProd.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContainerProd.Controllers
{
    [Route("/api/[controller]/[action]")]
    public class DefaultController : Controller
    {
        private ApplicationDbContext db;

        public DefaultController(ApplicationDbContext option)
        {
            db = option;
        }
        public IActionResult Index()
        {
            return Json("Index runs perfectly2 date is : " + DateTime.Now);
        }

        [HttpGet]
        public ActionResult ShowNextDaysFoods(long studentNumber, string session, DateTime? date)
        {
            List<Food> lstFoods = new List<Food>();

            if (!IsAuthenticated(studentNumber, session))
                return Json("NotAuthenticated");

            if (!date.HasValue)
                date = DateTime.Now;

            string Today = date.Value.DayOfWeek.ToString();
            string Tomorrow = date.Value.AddDays(1).DayOfWeek.ToString();
            string AfterTomorrow = date.Value.AddDays(2).DayOfWeek.ToString();
            //foreach (var food in db.Foods.Where(e => e.DayOfWeek == Today).ToList())
            //    lstFoods.Add(food);
            //foreach (var food in db.Foods.Where(e => e.DayOfWeek == Tomorrow).ToList())
            //    lstFoods.Add(food);

            //lstFoods = db.Foods.Where(e => e.DayOfWeek == Today || e.DayOfWeek == Tomorrow || e.DayOfWeek == AfterTomorrow).ToList();
            lstFoods = db.Foods.Where(e => e.DayOfWeek == Today || e.DayOfWeek.Trim() == Tomorrow.Trim() || e.DayOfWeek.Trim() == AfterTomorrow.Trim()).ToList();
            return Json(lstFoods);
        }

        [HttpGet]
        public ActionResult ReserveFood(long studentNumber, string session, int foodid)
        {
            string resultstatus = "Unsuccess";
            if (IsAuthenticated(studentNumber, session))
            {
                try
                {
                    if (db.ReservedFoods.Where(e => e.StudentNumber == studentNumber && e.FoodId == foodid && e.Status == true).Any())
                        resultstatus = "Duplicate";
                    else
                    {

                        db.ReservedFoods.Add(new ReservedFood
                        {
                            FoodId = foodid,
                            ReserveDate = DateTime.Now,
                            Status = true,
                            StudentNumber = studentNumber
                        });
                        db.SaveChanges();
                        resultstatus = "Success";
                    }
                }
                catch (Exception)
                {
                    resultstatus = "Unsuccess";
                }
            }
            return Json(resultstatus);
        }

        [HttpGet]
        public ActionResult GetReservedFoods(long studentNumber, string session)
        {
            List<ReservedFood> lstRsvFoods = new List<ReservedFood>();
            if (IsAuthenticated(studentNumber, session))
            {
                lstRsvFoods = db.ReservedFoods.Where(e => e.Status == true).ToList();
            }
            return Json(lstRsvFoods);
        }


        [HttpGet]
        public ActionResult GetFoodNameById(long studentNumber, string session, int foodid)
        {
            string result = string.Empty;
            if (IsAuthenticated(studentNumber, session))
            {
                result = db.Foods.Where(e => e.ID == foodid).FirstOrDefault().Name;
            }
            return Json(result);
        }


        [HttpGet]
        public ActionResult CancelReservation(long studentNumber, string session, int reserveid)
        {
            string result = "Unsuccess";
            if (IsAuthenticated(studentNumber, session))
            {
                var rvfood = db.ReservedFoods.Where(e => e.Id == reserveid).FirstOrDefault();
                try
                {
                    rvfood.Status = false;
                    db.SaveChanges();
                    result = "Success";
                }
                catch (Exception)
                {
                    result = "Unsuccess";
                }
            }
            return Json(result);
        }

        public bool IsAuthenticated(long studentNumber, string session)
        {
            bool result;
            string urlreq = "http://auth_service:8080/authenticate?studentNumber=" + studentNumber + "&session=" + session + "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlreq);
                request.Method = "GET";
                request.KeepAlive = true;
                request.ContentType = "appication/json";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string myResponse = string.Empty;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    myResponse = sr.ReadToEnd();
                }
                Auth_Response auth_ = null;
                if (myResponse.Length > 0)
                    auth_ = myResponse.FromJson<Auth_Response>();
                if (auth_ != null && auth_.status == "200")
                    result = true;
                else result = false;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}