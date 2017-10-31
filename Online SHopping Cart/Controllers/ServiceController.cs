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
            return View();
        }
        [HttpGet]
        public ActionResult Add_Service()
        {
            ViewBag.message = TempData["message"];
            Service_ViewModel svm = new Service_ViewModel();
            List<BaseCategory_Table> category = new List<BaseCategory_Table>();
            category = db.BaseCategory_Table.ToList();
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
            svm.locationList = db.Location_Table.ToList();
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
                List<ProductCategory_Table> productCategory = db.ProductCategory_Table.Where(x => x.BaseCatid == baseCatId).ToList();
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

            List<Image_Table> imageList = new List<Image_Table>();
            var products = (from p in db.Product_Table
                            where p.ProductCatid == productCatId
                            select p).ToList();
            foreach (var item in products)
            {
                int productId = (from p in db.Product_Table where p.ProductId == item.ProductId select p.ProductId).FirstOrDefault();
                var imageId = (from i in db.Image_Table where i.Productid == productId select i.ImageId);
                foreach (var images in imageId)
                {

                    var imagelist = (from im in db.Image_Table where im.ImageId == images select im).FirstOrDefault();
                    imageList.Add(imagelist);

                }
            }
            return PartialView("Products_PartialView", imageList.DistinctBy(x => x.Productid).ToList());
        }
        [HttpPost]
        public ActionResult Add_Service(Service_ViewModel model, string[] ids)
        {
            try
            {
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

            string name = Session["user"].ToString();
            int serviceProviderId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            List<Order_Table> orderList = new List<Order_Table>();
            var services = (from o in db.OrderDetail_Table where o.Serviceid == serviceProviderId select o.Orderid).ToList();
            foreach (var item in services)
            {
                Order_Table order = (from o in db.Order_Table where o.OrderId == item select o).FirstOrDefault();
                orderList.Add(order);

            }
            ViewBag.OrderList = orderList.DistinctBy(x => x.OrderId).ToList();

            return View();

        }
        [HttpPost]
        public ActionResult OrderDetails(int OrderId, int UserId)
        {

            List<OrderHistory_ViewModel> ohvmlist = new List<OrderHistory_ViewModel>();
            string userName = db.User_Table.Where(x => x.UserId == UserId).Select(x => x.UserName).FirstOrDefault();
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

            }
            ViewBag.list = list.Distinct();
            return View(ohvmlist);
        }

        [HttpGet]
        public ActionResult profile()
        {
            string name = Session["user"].ToString();
            User_Table obj = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
            return View(obj);
        }

        [HttpPost]
        public ActionResult profile(User_Table obj)
        {

            string name = Session["user"].ToString();
            User_Table user = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
            user.FirstName = obj.FirstName;
            user.LastName = obj.LastName;
            user.UserEmail = obj.UserEmail;
            user.UserAddress = obj.UserAddress;
            user.UserUpdatedDate = System.DateTime.Now;
            db.SaveChanges();

            return View();
        }

        public void logout()
        {
            Session["user"] = null;
            Session["count"] = null;
            Session.Abandon();
            Response.Redirect("~/User/login");
        }
    }
}