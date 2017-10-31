﻿using Online_SHopping_Cart.Models;
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
        public ActionResult create(User_Table obj, string x)
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
            return View();
        }

        [HttpPost]
        public ActionResult login(string user, string password)
        {
            User_Table obj = db.User_Table.Where(x => x.UserName == user).FirstOrDefault();
            if (obj != null)
            {
                if (obj.Password == password)
                {
                    if (obj.Roleid == 1 && obj.UserIsDeleted == false)
                    {
                        Session["user"] = obj.UserName;
                        return RedirectToAction("Homepage", "Admin");
                    }
                    else if (obj.Roleid == 2 && obj.UserIsDeleted == false)
                    {
                        Session["user"] = obj.UserName;
                        return RedirectToAction("Index", "Seller");
                    }
                    else if (obj.Roleid == 3 && obj.UserIsDeleted == false)
                    {
                        Session["user"] = obj.UserName;
                        return RedirectToAction("Service_Home", "Service");
                    }
                    else if (obj.Roleid == 4)
                    {
                        Session["user"] = obj.UserName;
                        string name = obj.UserName;
                        int id = db.User_Table.Where(x => x.UserName == name).Select(x=>x.UserId).FirstOrDefault();
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
                    return View();


                }
            }
            else
            {
                return View();
            }
            return View();
        }

        [HttpGet]
        public ActionResult forget_pass()
        {
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
                return RedirectToAction("SendMail", new { email = email });
            }
            return View();
        }

        public ActionResult SendMail(string email)
        {
                   
                MailMessage mail = new MailMessage();

                var fromAddress = "arjunhariharasubramani27@gmail.com";

                var toAddress = email;

                const string fromPassword = "h7736509721";

                string mailSubject = " New Password ";
                string mailBody = "user123";
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

    }

  }

        


        

       


   