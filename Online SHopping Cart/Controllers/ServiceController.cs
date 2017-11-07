using Online_SHopping_Cart.Models;
using Online_SHopping_Cart.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using System.Web;
using System.Web.Mvc;

namespace Online_SHopping_Cart.Controllers
{
    public class ServiceController : Controller
    {
        ShoppingCartDbEntities db = new ShoppingCartDbEntities();
        // GET: Service
       
       
        public ActionResult Service_Home()
        {
            Notofication_Count();
            order_count();
            return View();
        }

        public void order_count()
        {
            string name = Session["user"].ToString();
            int orderCount = 0;
            int serviceProviderId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            List<Order_Table> orderList = new List<Order_Table>();
            var orders = (from o in db.Order_Table where o.OrderStatus == 1 && o.OrderIsDeleted == false select o);
            foreach (var item in orders)
            {
                var serviceids = (from or in db.OrderDetail_Table where item.OrderId == or.Orderid select or.Serviceid).ToList();
                foreach (var item1 in serviceids)
                {

                    var serid = (from a in db.Service_Table where a.ServiceId == item1 select a.ServiceProviderid).FirstOrDefault();
                    if (serid == serviceProviderId)
                    {
                        if (item.OrderDeliveryDate > System.DateTime.Now)
                        {
                            orderCount++;
                        }

                        orderList.Add(item);
                    }
                    Session["ordercount"] = orderCount;
                }
            }
        }
        public void Notofication_Count()
        {
            string name = Session["user"].ToString();

            int serviceProviderId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            List<Order_Table> orderlist = new List<Order_Table>();
            var orders = (from o in db.Order_Table where o.OrderStatus == 1 && o.OrderIsDeleted == false && o.OrderNotification == "00" || o.OrderNotification == "10" select o);
            foreach (var item in orders)
            {
                var serviceids = (from or in db.OrderDetail_Table where item.OrderId == or.Orderid select or.Serviceid).ToList();
                foreach (var item1 in serviceids)
                {

                    var serid = (from a in db.Service_Table where a.ServiceId == item1 select a.ServiceProviderid).FirstOrDefault();
                    if (serid == serviceProviderId)
                    {
                        orderlist.Add(item);
                    }
                }
            }
            var l = orderlist.DistinctBy(x => x.OrderId).ToList();
            // ViewBag.ordering = orderlist;
            ViewBag.ordering = l;
            //Session["notif-count"] = orderlist.Count();

            Session["notif-count"] = l.Count();
        }
        public void DisableNotification()
        {
            Order_Table obj = new Order_Table();
            var order = (from a in db.Order_Table select a).ToList();
            foreach (var item in order)
            {
                if (item.OrderNotification == "00")
                {
                    item.OrderNotification = "01";
                }
                else if (item.OrderNotification == "10")
                {
                    item.OrderNotification = "11";
                }

            }
            db.SaveChanges();
            Notofication_Count();
        }

        [HttpGet]
        public ActionResult Add_Service()
        {
            Notofication_Count();
            ViewBag.message = TempData["message"];
            Service_ViewModel svm = new Service_ViewModel();
            List<BaseCategory_Table> category = new List<BaseCategory_Table>();
            category = db.BaseCategory_Table.Where(x => x.BaseCatIsDeleted==false).ToList();
            var productlist = new List<SelectListItem>();
            foreach (var item in category)
            {
                productlist.Add(new SelectListItem
                {
                    Text = item.BaseCatName.ToString(),
                    Value = item.BaseCatId.ToString(),

                });

                ViewBag.baseCategory = productlist;
            }
            TempData["categoryName"] = ViewBag.baseCategory;
            svm.locationList = db.Location_Table.Where(x=>x.LocationIsDeleted==false).ToList();
            svm.selectedLocation = 0;
            return View(svm);

        }
        public ActionResult GetProductCategory(string id)
        {
            int baseCatId;
            List<SelectListItem> productcategoryList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(id))
            {
                baseCatId = Convert.ToInt32(id);
                List<ProductCategory_Table> productCategory = db.ProductCategory_Table.Where(x => x.BaseCatid == baseCatId && x.ProductCatIsDeleted==false).ToList();
                foreach (var item in productCategory)
                {
                    productcategoryList.Add(new SelectListItem
                    {
                        Text = item.ProductCatName.ToString(),

                        Value = item.ProductCatId.ToString(),

                    });
                }
            }
            return Json(productcategoryList, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult Image_PartialView(int productCatId)
        {
            List<Buyer_Product> imageList = new List<Buyer_Product>();
            var products = (from p in db.Product_Table
                            where p.ProductCatid == productCatId && p.ProductIsDeleted == false
                            select p).ToList();
            foreach (var item in products)
            {

                Buyer_Product obj = new Buyer_Product();
                obj.ProductName = item.ProductName;
                obj.ProductId = item.ProductId;
                obj.ProductPrice = item.ProductPrice;
                obj.ProductDesc = item.ProductDesc;
                Image_Table img = db.Image_Table.Where(x => x.Productid == item.ProductId && x.ImageIsDeleted==false).FirstOrDefault();
                if(img!=null)
                {
                    obj.BinaryImage = img.BinaryImage;
                    imageList.Add(obj);
                }
                

            }
            ViewBag.imglist = imageList.DistinctBy(x => x.ProductId).ToList();
            return PartialView("Products_PartialView");
        }
        [HttpPost]
        public ActionResult imagedisplay(int id)
        {
            Product_Table product = db.Product_Table.Find(id);

            var image = (from a in db.Image_Table where a.Productid == product.ProductId && a.ImageIsDeleted == false select a).ToList();
            ViewBag.imlist = image.ToList();
            return PartialView("imagedisplay");
        }
        [HttpPost]
        public ActionResult Add_Service(Service_ViewModel model, string[] ids)
        {
            try
            {
                Notofication_Count();
                string name = Session["user"].ToString();
                int serviceProviderId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
                List<Order_Table> orderList = new List<Order_Table>();
                for (int j = 0; j < ids.Count(); j++)
                {
                    int currentProduct = Convert.ToInt32(ids[j]);
                    var product = (from s in db.Service_Table where s.Productid == currentProduct && s.Locationid == model.selectedLocation && s.ServiceProviderid == serviceProviderId && s.ServiceIsDeleted == false select s).FirstOrDefault();

                    if (product == null)
                    {
                        Service_Table service = new Service_Table();
                        service.Locationid = model.selectedLocation;
                        service.DeliveryCharge = model.deliveryCharge;
                        service.ServiceDesc = model.serviceDescription;
                        service.Productid = Convert.ToInt32(ids[j]);
                        service.ServiceName = model.serviceName;
                        service.ServiceProviderid = serviceProviderId;
                        service.ServiceCreatedDate = System.DateTime.Now;
                        service.ServiceUpdatedDate = System.DateTime.Now;
                        service.ServiceCreatedBy = Session["user"].ToString();
                        service.ServiceUpdatedBy = Session["user"].ToString();
                        service.ServiceIsDeleted = false;
                        db.Service_Table.Add(service);
                        db.SaveChanges();


                    }
                    else
                    {
                        var productName = (from p in db.Product_Table where p.ProductId == currentProduct select p.ProductName).FirstOrDefault();
                        string message = String.Format("Same location already added for the product " + productName);
                        TempData["message"] = message;
                        return RedirectToAction("Add_Service");
                    }
                }
                TempData["message"] = "Service Added";
                return RedirectToAction("Add_Service");
            }
            catch (Exception e)
            {
                TempData["message"] = "fill all fileds";
                return RedirectToAction("Add_Service");
            }

        }
        public ActionResult Manage_Service()
        {
            Notofication_Count();
            string name = Session["user"].ToString();
            int serviceProviderId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();

            var serviceDetails = (from s in db.Service_Table
                                  join p in db.Product_Table on s.Productid equals p.ProductId
                                  join l in db.Location_Table on s.Locationid equals l.LocationId
                                  where s.ServiceIsDeleted != true && s.ServiceProviderid == serviceProviderId
                                  select new
                                  {
                                      s.ServiceId,
                                      s.ServiceName,
                                      s.DeliveryCharge,
                                      s.ServiceDesc,
                                      l.LocationName,
                                      p.ProductName
                                  });

            ViewBag.Services = serviceDetails.ToList();



            return View();
        }
        [HttpPost]
        public JsonResult ServiceEdit(int ServiceId, string ServiceName, decimal DeliveryCharge, string ServiceDesc, string LocationName, string ProductName)
        {

            Product_Table product = new Product_Table();
            Location_Table location = new Location_Table();
            Service_Table service = db.Service_Table.Find(ServiceId);
            service.ServiceName = ServiceName;
            service.DeliveryCharge = DeliveryCharge;
            service.ServiceDesc = ServiceDesc;
            int locationId = (from l in db.Location_Table where l.LocationName == LocationName select l.LocationId).FirstOrDefault();

            service.Locationid = locationId;
            int productId = (from p in db.Product_Table where p.ProductName == ProductName select p.ProductId).FirstOrDefault();
            service.Productid = productId;
            service.ServiceUpdatedDate = System.DateTime.Now;
            db.SaveChanges();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Manage_Service", "Service");
            return Json(new { ServiceId = ServiceId, ServiceName = ServiceName, DeliveryCharge = DeliveryCharge, ServiceDesc = ServiceDesc, LocationName = LocationName, ProductName = ProductName, Url = redirectUrl }, JsonRequestBehavior.AllowGet);



        }

        [HttpPost]
        public ActionResult ServiceDelete(int ServiceId)
        {
            Service_Table service = db.Service_Table.Find(ServiceId);
            service.ServiceIsDeleted = true;
            db.SaveChanges();
            bool result = true;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Manage_Service", "Service");
            return Json(new { Url = redirectUrl, result });
        }
        public ActionResult View_Service()
        {
            DisableNotification();
            Notofication_Count();

            string name = Session["user"].ToString();
            int serviceProviderId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            List<Order_Table> orderList = new List<Order_Table>();
            var orders = (from o in db.Order_Table where o.OrderStatus == 1 && o.OrderIsDeleted == false select o);
            foreach (var item in orders)
            {
                var serviceids = (from or in db.OrderDetail_Table where item.OrderId == or.Orderid select or.Serviceid).ToList();
                foreach (var item1 in serviceids)
                {

                    var serid = (from a in db.Service_Table where a.ServiceId == item1 select a.ServiceProviderid).FirstOrDefault();
                    if (serid == serviceProviderId)
                    {


                        orderList.Add(item);
                    }

                }
            }

            ViewBag.OrderList = orderList.DistinctBy(x => x.OrderId).ToList();

            return View();

        }
        [HttpPost]
        public ActionResult OrderDetails(int OrderId, int UserId)
        {

            List<OrderHistory_ViewModel> ohvmlist = new List<OrderHistory_ViewModel>();
            string userName = db.User_Table.Where(x => x.UserId == UserId).Select(x => x.UserName).FirstOrDefault();
            string userPhone = db.User_Table.Where(x => x.UserId == UserId).Select(x => x.UserPhno).FirstOrDefault();

            var obj = db.OrderDetail_Table.Where(x => x.Orderid == OrderId).ToList();
            List<string> list = new List<string>();
            foreach (var item in obj)
            {
                var service = db.OrderDetail_Table.Where(x => x.Orderid == OrderId).Select(x => x.Serviceid).FirstOrDefault();
                var product_desc = db.Product_Table.Where(x => x.ProductId == item.Productid).Select(x => x.ProductDesc).FirstOrDefault();
                var product = db.Product_Table.Where(x => x.ProductId == item.Productid).Select(x => x.ProductName).FirstOrDefault();
                var deliveryadd = db.Order_Table.Where(x => x.OrderId == OrderId).Select(x => x.OrderDeliveryAddress).FirstOrDefault();
                var deliverydate = db.Order_Table.Where(x => x.OrderId == OrderId).Select(x => x.OrderDeliveryDate).FirstOrDefault();
                var image = db.Image_Table.Where(x => x.Productid == item.Productid).Select(x => x.BinaryImage).FirstOrDefault();
                OrderHistory_ViewModel obj1 = new OrderHistory_ViewModel();
                obj1.ProductName = product;
                obj1.ProductDesc = product_desc;
                obj1.OrderDelivryAddress = deliveryadd;
                obj1.Amount = (decimal)item.Amount;
                obj1.OrderDeliveryDate = (DateTime)deliverydate;
                obj1.CustomerName = userName;
                obj1.BinaryImage = image;
                ohvmlist.Add(obj1);
                list.Add(userName);
                list.Add(deliveryadd);
                list.Add(userPhone);

            }
            ViewBag.list = list.Distinct();
            return PartialView("OrderDetails", ohvmlist);
        }


        [HttpGet]
        public ActionResult profile()
        {
            Notofication_Count();
            ViewBag.fill_msg = TempData["fill_msg"];
            Notofication_Count();
            string name = Session["user"].ToString();
            User_Table obj = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
            return View(obj);
        }

        [HttpPost]
        public ActionResult profile(User_Table obj)
        {
            Notofication_Count();
            if (obj.FirstName != null && obj.LastName != null && obj.UserEmail != null && obj.UserAddress != null && obj.UserPhno != null)
            {
                string name = Session["user"].ToString();
                User_Table user = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
                user.FirstName = obj.FirstName;
                user.LastName = obj.LastName;
                user.UserEmail = obj.UserEmail;
                user.UserAddress = obj.UserAddress;
                user.UserPhno = obj.UserPhno;
                user.UserUpdateBy = name;
                user.UserUpdatedDate = System.DateTime.Now;
                db.SaveChanges();
            }
            else
            {
                TempData["fill_msg"] = "Please Enter The Details";
                return RedirectToAction("profile");
            }
            return View();
        }

        public ActionResult ChangePassword()
        {
            Notofication_Count();
            ViewBag.message = TempData["message"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            Notofication_Count();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User_Table obj = new User_Table();
            string name = Session["user"].ToString();
            User_Table details = (from a in db.User_Table where a.UserName == name select a).FirstOrDefault();
            if (details.Password == model.OldPassword)
            {
                if (details.Password == model.NewPassword)
                {
                    TempData["message"] = "your old password and new password are same!!!";
                }
                else if (model.NewPassword == model.ConfirmPassword)
                {
                    details.Password = model.NewPassword;
                    db.SaveChanges();
                    TempData["message"] = "password changes successfully!!";
                }
                else
                {
                    TempData["message"] = "confirm password an new password does not match";
                }
            }
            else
            {
                TempData["message"] = "your old password is incorrect ";
            }
            return RedirectToAction("ChangePassword");
        }

        public void logout()
        {
            Session["user"] = null;
            Session["count"] = null;
            Session.Abandon();
            Response.Redirect("/User/login");
        }
    }
}