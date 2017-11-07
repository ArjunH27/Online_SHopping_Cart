using Online_SHopping_Cart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Online_SHopping_Cart.Controllers
{
    public class UserController : Controller
    {
        ShoppingCartDbEntities db = new ShoppingCartDbEntities();
        // GET: User
        public ActionResult create()
        {
            List<Role_Table> role = new List<Role_Table>();
            role = db.Role_Table.Where(x => x.RoleIsDeleted == false).ToList();
            var rolelist = new List<SelectListItem>();
            foreach (var item in role)
            {
                rolelist.Add(new SelectListItem
                {
                    Text = item.RoleName.ToString(),
                    Value = item.RoleId.ToString()
                });
            }
            ViewBag.rolename = rolelist;
            return View();
        }
        [HttpPost]
        public ActionResult create(User_Table obj)
        {
            if (ModelState.IsValid)
            {
                obj.UserCreatedDate = System.DateTime.Now;
                obj.UserUpdatedDate = System.DateTime.Now;
                if (obj.Roleid == 1 || obj.Roleid == 2 || obj.Roleid == 3)
                {
                    obj.UserCreatedBy = obj.UserName;
                    obj.UserUpdateBy = obj.UserName;
                    obj.UserIsDeleted = true;
                    db.User_Table.Add(obj);
                    db.SaveChanges();
                    return RedirectToAction("login");
                }
                else
                {
                    obj.UserCreatedBy = obj.UserName;
                    obj.UserUpdateBy = obj.UserName;
                    obj.UserIsDeleted = false;
                    db.User_Table.Add(obj);
                    db.SaveChanges();
                    return RedirectToAction("login");
                }
            }
            else
            {
                return RedirectToAction("errror");
            }
        }

        [HttpGet]
        public ActionResult login()
        {
            ViewBag.cmsg = TempData["cmsg"];
            ViewBag.message1 = TempData["message1"];
            ViewBag.message2 = TempData["message2"];
            ViewBag.message3 = TempData["message3"];
            return View();
        }

        [HttpPost]
        public ActionResult login(string user, string password)
        {
            User_Table obj = db.User_Table.Where(x => x.UserName == user).FirstOrDefault();
          
            if (obj != null)
            {
                Role_Table robj = db.Role_Table.Where(x => x.RoleId == obj.Roleid).FirstOrDefault();
                if (obj.Password == password)
                {
                    if (obj.UserIsDeleted == false)
                    {

                        if (robj.RoleName == "Super_Admin")
                        {
                            Session["user"] = obj.UserName;
                            return RedirectToAction("Homepage", "Admin");
                        }
                        else if (robj.RoleName == "Seller")
                        {
                            Session["user"] = obj.UserName;
                            return RedirectToAction("Index", "Seller");
                        }
                        else if (robj.RoleName == "Courier_Service")
                        {
                            Session["user"] = obj.UserName;
                            Session["name"] = obj.FirstName;
                            return RedirectToAction("Service_Home", "Service");
                        }
                        else if (robj.RoleName == "Buyer")
                        {
                            Session["user"] = obj.UserName;
                            Session["name"] = obj.FirstName;
                           
                            string name = obj.UserName;
                            int id = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
                            var oder_id = db.Order_Table.Where(x => x.Userid == id & x.OrderStatus == 0 & x.OrderIsDeleted == false).Select(x => x.OrderId).FirstOrDefault();
                            int count = db.OrderDetail_Table.Where(x => x.Orderid == oder_id).Count();
                            Session["count"] = count;
                            return RedirectToAction("loader", "User");
                        }
                        else
                        {
                            return RedirectToAction("errror");
                        }
                    }
                    else
                    {
                        TempData["message3"] = "Not an Autharized User";
                        return RedirectToAction("login", "User");
                    }

                }
                else
                {
                    TempData["message2"] = "Password Dont Match";
                    return RedirectToAction("login", "User");


                }
            }
            else
            {
                TempData["message1"] = "User Does Not Exist";
                return RedirectToAction("login", "User");
            }
            return View();
        }

        [HttpGet]
        public ActionResult forget_pass()
        {
            ViewBag.for_valid = TempData["for_valid"];
            return View();
        }

        [HttpPost]
        public ActionResult forget_pass(string user,string email)
        {
            User_Table obj = db.User_Table.Where(x => x.UserName == user & x.UserEmail == email).FirstOrDefault();
            if(obj!=null)
            {
                obj.Password = "user123";
                db.SaveChanges();
                TempData["cmsg"] = "New Psssword is send to your Mail";
                return RedirectToAction("SendMail", new { email = email });
            }
            else
            {
                TempData["for_valid"] = "Enter the Correct Credentials";
                return RedirectToAction("forget_pass", "User");
            }
            return View();
        }

        public ActionResult SendMail(string email)
        {
                   
                MailMessage mail = new MailMessage();

                var fromAddress = "factoryforshop@gmail.com";

                var toAddress = email;

                const string fromPassword = "shopfactory123";

                string mailSubject = " New Password ";
                string mailBody = "New Password : user123";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(fromAddress, fromPassword); // admin name and password   
                smtp.EnableSsl = true;
                smtp.Send(fromAddress, toAddress, mailSubject, mailBody);


            return RedirectToAction("login");


        }

        [HttpGet]
        public ActionResult errror()
        {
            return View();
        }

        public ActionResult loader()
        {
            return View();
        }

        public JsonResult confirm_pass(string pass, string cp)
        {
            int res = 0;
            if(pass==cp)
            {
                res = 1;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsNameExist(string UserName)
        {
            var validateName = db.User_Table.Where(x => x.UserName == UserName && x.UserIsDeleted == false).FirstOrDefault();   //Details of base category that has same name of Entered rolename and which are active is taken

            if (validateName != null)                                                                                                          //If no such details exist false is returned 
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else                                                                                                                                //If details exist true is returned which dispalys error message given with validation of Base Category
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult IsmailExist(string UserEmail)
        {
            var validateName = db.User_Table.Where(x => x.UserEmail == UserEmail && x.UserIsDeleted == false).FirstOrDefault();   //Details of base category that has same name of Entered rolename and which are active is taken

            if (validateName != null)                                                                                                          //If no such details exist false is returned 
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else                                                                                                                                //If details exist true is returned which dispalys error message given with validation of Base Category
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }

  }

        


        

       


   