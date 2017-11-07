using Online_SHopping_Cart.Models;
using Online_SHopping_Cart.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Online_SHopping_Cart.Controllers
{
    public class BuyerController : Controller
    {
         public List<int> avail_list = new List<int>();
        ShoppingCartDbEntities db = new ShoppingCartDbEntities();
        // GET: Buyer

        public ActionResult Index()
        {
            check_stock();
            List<BaseCategory_Table> cato = new List<BaseCategory_Table>();
            cato = db.BaseCategory_Table.Where(x => x.BaseCatIsDeleted == false).ToList();
            
            Session["filter1"] = 0;
            Session["filter2"] = 0;

            List<Product_Table> prods = db.Product_Table.OrderByDescending(x => x.ProductId).Take(3).ToList();
            List<Buyer_Product> plist = new List<Buyer_Product>();
            foreach (var item in prods)
            {
                Buyer_Product obj = new Buyer_Product();
                obj.ProductName = item.ProductName;
                obj.ProductId = item.ProductId;
                obj.ProductPrice = item.ProductPrice;
                obj.ProductDesc = item.ProductDesc;
                Image_Table img = db.Image_Table.Where(x => x.Productid == item.ProductId).FirstOrDefault();
                if (img != null)
                {
                    obj.BinaryImage = img.BinaryImage;
                    plist.Add(obj);
                }
                
            }
            ViewBag.newpro = plist;

            return View(cato);
        }

        [HttpGet]
        public ActionResult profile()
        {
            ViewBag.fill_msg = TempData["fill_msg"];
            string name = Session["user"].ToString();
            User_Table obj = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
            return View(obj);
        }

        [HttpPost]
        public ActionResult profile(User_Table obj)
        {
          
           if(obj.FirstName!=null && obj.LastName!=null && obj.UserEmail!=null && obj.UserAddress!=null && obj.UserPhno!=null )
            { 
            string name = Session["user"].ToString();
            User_Table user = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
            user.FirstName = obj.FirstName;
            user.LastName = obj.LastName;
            user.UserEmail = obj.UserEmail;
            user.UserAddress = obj.UserAddress;
            user.UserPhno = obj.UserPhno;
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

        public ActionResult Product_cat()
        {

            int id = (int)Session["base_cat"];

            List<ProductCategory_Table> pcato = new List<ProductCategory_Table>();
            pcato = db.ProductCategory_Table.Where(x => x.BaseCatid == id & x.ProductCatIsDeleted==false).ToList();
            return View(pcato);

        }

        public ActionResult Product_page()
        {
            int id = (int)Session["prod_cat"];
            int ? f1 = (int)Session["filter1"];
            int ? f2 = (int)Session["filter2"];
            List<Product_Table> prods = new List<Product_Table>();
            if(f1==1)
            {
                prods = db.Product_Table.Where(x => x.ProductCatid == id & x.ProductIsDeleted==false).OrderBy(x=>x.ProductPrice).ToList();
            }
            else if(f2==1)
            {
                prods = db.Product_Table.Where(x => x.ProductCatid == id & x.ProductIsDeleted == false).OrderByDescending(x => x.ProductPrice).ToList();
            }
            else
            {
                prods = db.Product_Table.Where(x => x.ProductCatid == id & x.ProductIsDeleted == false).ToList();
            }
            List<int> not_avail_product = new List<int>();
            List<Buyer_Product> plist = new List<Buyer_Product>();
            foreach(var item in prods)
            {
                Buyer_Product obj = new Buyer_Product();
                obj.ProductName = item.ProductName;
                obj.ProductId = item.ProductId;
                obj.ProductPrice = item.ProductPrice;
                obj.ProductDesc = item.ProductDesc;
                if(item.ProductStock<=0)
                {
                    not_avail_product.Add(item.ProductId);
                }
                Image_Table img = db.Image_Table.Where(x => x.Productid == item.ProductId && x.ImageIsDeleted==false).FirstOrDefault();
                if(img!=null)
                {
                    obj.BinaryImage = img.BinaryImage;
                    plist.Add(obj);
                }
               
            }
            ViewBag.no_stock = not_avail_product;
            Session["filter1"] = 0;
            Session["filter2"] = 0;
            return View(plist);
        }

        public ActionResult Brand_page()
        {
            int id = (int)Session["brand_id"];
            int? f1 = (int)Session["filter1"];
            int? f2 = (int)Session["filter2"];
            List<Product_Table> prods = new List<Product_Table>();
            if (f1 == 1)
            {
                prods = db.Product_Table.Where(x => x.SellerId == id & x.ProductIsDeleted == false).OrderBy(x => x.ProductPrice).ToList();
            }
            else if (f2 == 1)
            {
                prods = db.Product_Table.Where(x => x.SellerId == id & x.ProductIsDeleted == false).OrderByDescending(x => x.ProductPrice).ToList();
            }
            else
            {
                prods = db.Product_Table.Where(x => x.SellerId == id & x.ProductIsDeleted == false).ToList();
            }
            List<int> not_avail_product = new List<int>();
            List<Buyer_Product> plist = new List<Buyer_Product>();
            foreach (var item in prods)
            {
                Buyer_Product obj = new Buyer_Product();
                obj.ProductName = item.ProductName;
                obj.ProductId = item.ProductId;
                obj.ProductPrice = item.ProductPrice;
                obj.ProductDesc = item.ProductDesc;
                if (item.ProductStock <= 0)
                {
                    not_avail_product.Add(item.ProductId);
                }
                Image_Table img = db.Image_Table.Where(x => x.Productid == item.ProductId && x.ImageIsDeleted==false).FirstOrDefault();
                obj.BinaryImage = img.BinaryImage;
                plist.Add(obj);
            }
            ViewBag.no_stock = not_avail_product;
            Session["filter1"] = 0;
            Session["filter2"] = 0;
            return View(plist);
        }

        [HttpGet]
        public ActionResult order(int id)
        {
            Session["pro_id"] = id;
            List<Location_Table> location = new List<Location_Table>();
            location = db.Location_Table.ToList();
            var loclist = new List<SelectListItem>();
            foreach (var item in location)
            {
                loclist.Add(new SelectListItem
                {
                    Text = item.LocationName.ToString(),
                    Value = item.LocationId.ToString(),

                });

                ViewBag.location_list = loclist;

            }

            Book obj = new Book();
            Product_Table pro = db.Product_Table.Where(x => x.ProductId == id).FirstOrDefault();
            obj.ProductName = pro.ProductName;
            obj.ProductDesc = pro.ProductDesc;
            obj.ProductPrice = pro.ProductPrice;
            obj.ProductStock = pro.ProductStock;
            ViewBag.image_list = db.Image_Table.Where(x => x.Productid == id && x.ImageIsDeleted==false).ToList();
            return View(obj);
        }

        [HttpPost]
        public ActionResult order(Book obj,string amt)
        {

            string name = Session["user"].ToString();
            int pid = Convert.ToInt32(Session["pro_id"].ToString());
            int stock = db.Product_Table.Where(x => x.ProductId == pid).Select(x => x.ProductStock).FirstOrDefault();
            if(obj.Quantity<=stock)
            {
                Order_Table order = new Order_Table();
                OrderDetail_Table order_detail = new OrderDetail_Table();
                order.TotalAmount = Convert.ToInt32(amt);
                order.OrderDeliveryAddress = obj.OrderDelivryAddress;
                order.OrderDeliveryDate = System.DateTime.Now.AddDays(5);
                order.Userid = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
                order.OrderCreatedBy = name;
                order.OrderUpdatedBy = name;
                order.OrderCreatedDate = System.DateTime.Now;
                order.OrderUpdatedDate = System.DateTime.Now;
                order.OrderIsDeleted = false;
                order.OrderStatus = 1;
                order.OrderNotification = "00";
                db.Order_Table.Add(order);
                db.SaveChanges();
                int oid = order.OrderId;
                order_detail.Orderid = oid;
                order_detail.Productid = pid;
                int lid = Convert.ToInt32(Session["location"]);
                Service_Table serboj = db.Service_Table.Where(x => x.ServiceProviderid == obj.UserId && x.Locationid == lid).FirstOrDefault();
                order_detail.Serviceid = serboj.ServiceId;
                order_detail.Quantity = obj.Quantity;
                order_detail.Amount = Convert.ToInt32(amt);
                db.OrderDetail_Table.Add(order_detail);
                db.SaveChanges();
                Product_Table pobj = db.Product_Table.Where(x => x.ProductId == pid).FirstOrDefault();
                pobj.ProductStock = stock - obj.Quantity;
                db.SaveChanges();
            }
            Session["location"] = null;
            return RedirectToAction("notification");
        }

        [HttpGet]
        public ActionResult purchase_all()
        {
            List<Location_Table> location = new List<Location_Table>();
            location = db.Location_Table.ToList();
            var loclist = new List<SelectListItem>();
            foreach (var item in location)
            {
                loclist.Add(new SelectListItem
                {
                    Text = item.LocationName.ToString(),
                    Value = item.LocationId.ToString(),

                });

                ViewBag.location_list = loclist;

            }
            return View();
        }

        [HttpPost]
        public ActionResult purchase_all(Book obj)
        {
            string name = Session["user"].ToString();
            int usid = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();

            Order_Table order= db.Order_Table.Where(x => x.OrderStatus == 0 & x.OrderIsDeleted == false & x.Userid == usid).FirstOrDefault();
            order.OrderDeliveryAddress = obj.OrderDelivryAddress;
            order.TotalAmount = Convert.ToInt32(TempData["tcart_amt"]);
            order.OrderNotification = "00";
            order.OrderDeliveryDate = System.DateTime.Now.AddDays(5);
            order.OrderStatus = 1;
            db.SaveChanges();
            var odetail = db.OrderDetail_Table.Where(x => x.Orderid == order.OrderId).Select(x=>x.OrderDetailId).ToList();
            foreach(var item in odetail)
            {
                int flag = 0;
                List<int> availid1 = TempData["avail1"] as List<int>;
                OrderDetail_Table obj1 = db.OrderDetail_Table.Where(x => x.OrderDetailId == item).FirstOrDefault();
                foreach(var pro in availid1)
                {
                    if(obj1.Productid==pro)
                    {
                        Product_Table pobj = db.Product_Table.Where(x => x.ProductId == pro).FirstOrDefault();
                        int stock = Convert.ToInt32(pobj.ProductStock);
                        int qty = Convert.ToInt32(obj1.Quantity);
                        pobj.ProductStock = stock -qty;
                        db.SaveChanges();
                        flag = 1;
                    }
                }
                if(flag==1)
                {
                    int lid = Convert.ToInt32(Session["location"]);
                    Service_Table serboj = db.Service_Table.Where(x => x.ServiceProviderid == obj.UserId && x.Locationid == lid).FirstOrDefault();
                    obj1.Serviceid = serboj.ServiceId;
                    db.SaveChanges();
                }
                else
                {
                    db.OrderDetail_Table.Remove(obj1);
                    db.SaveChanges();
                }
            }
            count_cart();
            Session["location"] = null;
            return RedirectToAction("notification");
        }

        public ActionResult service_name(string id)
        {
            Session["location"] = id;
            int pid = Convert.ToInt32(Session["pro_id"].ToString());
            int locId;
            List<SelectListItem> ServiceList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(id))
            {
                locId = Convert.ToInt32(id);
                List<Service_Table> ser = db.Service_Table.Where(x => x.Locationid == locId & x.Productid == pid).ToList();
                foreach (var item in ser)
                {
                    User_Table obj = db.User_Table.Where(x => x.UserId == item.ServiceProviderid).FirstOrDefault();
                    ServiceList.Add(new SelectListItem
                    {
                        Text = obj.UserName.ToString(),
                        Value = obj.UserId.ToString(),

                    });
                }

                ViewBag.procat = ServiceList;
            }
            return Json(ServiceList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult service_name_all(string id)
        {
            Session["location"] = id;
            List<SelectListItem> ServiceList = new List<SelectListItem>();
            List<Service_Table> s = new List<Service_Table>();
            
            int locId= Convert.ToInt32(id);
            // var ser = db.Service_Table.Where(x => x.Locationid == locId).Select(x => x.ServiceProviderid).ToList();
            var ser = db.Service_Table.Where(x => x.Locationid == locId).ToList();
            int previd=0;
            foreach (var items in ser)
            {
                if (previd !=items.ServiceProviderid)
                {
                    Service_Table excuseme = db.Service_Table.Where(x => x.ServiceProviderid == items.ServiceProviderid).FirstOrDefault();
                    previd = excuseme.ServiceProviderid;
                    s.Add(excuseme);
                }
               
            }
            int total_pro = Convert.ToInt32(TempData["count"]);
            List<int> availid = TempData["avail"] as List<int>;
            

           foreach (var item1 in s)
           {
                int count = 0;
                foreach (var item in availid)
                {
                    int pid = Convert.ToInt32(item);
                    Service_Table obj = db.Service_Table.Where(x => x.Productid == pid & x.ServiceProviderid==item1.ServiceProviderid).FirstOrDefault();
                    if(obj!=null)
                    {
                        count++;
                    }
                }
                if(count== total_pro)
                {
                    string sname = db.Service_Table.Where(x => x.ServiceProviderid == item1.ServiceProviderid).Select(x => x.ServiceName).FirstOrDefault();
                    User_Table uobj = db.User_Table.Where(x => x.UserId == item1.ServiceProviderid).FirstOrDefault();
                    ServiceList.Add(new SelectListItem
                    {
                        Text = uobj.UserName,
                        Value = uobj.UserId.ToString(),

                    });
                }
            }
            

            return Json(ServiceList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult calculate_amt(int qty,int service_id)
        {
            int pid = Convert.ToInt32(Session["pro_id"].ToString());
            int stock = db.Product_Table.Where(x => x.ProductId == pid).Select(x => x.ProductStock).FirstOrDefault();
            int quantity = Convert.ToInt32(qty);
            decimal total = 0;
            if (quantity<=stock)
            { 
            int ser_id = Convert.ToInt32(service_id);
            decimal del_charge = db.Service_Table.Where(x => x.ServiceProviderid == ser_id & x.Productid == pid).Select(x => x.DeliveryCharge).FirstOrDefault();
            decimal price = db.Product_Table.Where(x => x.ProductId == pid).Select(x => x.ProductPrice).FirstOrDefault();
             total = (quantity * price) + del_charge;
            return Json(new { total, del_charge } , JsonRequestBehavior.AllowGet);
            }
            return Json(total, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult cart()
        {
            float total = 0;
            int c = 0;
            string name = Session["user"].ToString();
            List<Buyer_Product> plist = new List<Buyer_Product>();
            int uid = db.User_Table.Where(x => x.UserName == name).Select(x=>x.UserId).FirstOrDefault();
            int oid = db.Order_Table.Where(x => x.Userid == uid & x.OrderStatus == 0 & x.OrderIsDeleted == false).Select(x => x.OrderId).FirstOrDefault();
            var pro_id = db.OrderDetail_Table.Where(x => x.Orderid == oid).Select(x => x.Productid).ToList();
            List<int> avail_product = new List<int>();
            List<int> not_avail_product = new List<int>();
            foreach (var item1 in pro_id)
            {
                Product_Table pobj = db.Product_Table.Where(x => x.ProductId == item1).FirstOrDefault();
                Buyer_Product bobj = new Buyer_Product();
                bobj.ProductName = pobj.ProductName;
                bobj.ProductId = pobj.ProductId;
                bobj.ProductPrice = pobj.ProductPrice;
                bobj.ProductDesc = pobj.ProductDesc;
                if(pobj.ProductStock>0)
                {
                    total +=(float) pobj.ProductPrice;
                    avail_list.Add(pobj.ProductId);
                    avail_product.Add(pobj.ProductId);
                    c++;
                }
                else
                {
                    not_avail_product.Add(pobj.ProductId);
                }
                Image_Table img = db.Image_Table.Where(x => x.Productid == item1 && x.ImageIsDeleted==false).FirstOrDefault();
                if (img != null)
                {
                    bobj.BinaryImage = img.BinaryImage;
                    plist.Add(bobj);
                }
            
            }
            TempData["count"] = c;
            TempData["tcart_amt"] = total;      
            ViewBag.avail = avail_product;
            ViewBag.tcart_amt = total;
            TempData["avail"]= avail_product;
            TempData.Keep("avail");
            TempData["avail1"] = avail_product;
            ViewBag.not_avail = not_avail_product;
            return View(plist);
        }

        [HttpPost]
        public JsonResult add_cart(int id)
        {
            string user = Session["user"].ToString();
            int usid = db.User_Table.Where(x => x.UserName == user).Select(x => x.UserId).FirstOrDefault();
            Order_Table obj = db.Order_Table.Where(x => x.OrderStatus == 0 & x.OrderIsDeleted == false & x.Userid==usid).FirstOrDefault();
            if(obj==null)
            {
                Order_Table obj1 = new Order_Table();
                obj1.OrderStatus = 0;
                obj1.OrderIsDeleted = false;
                string name = Session["user"].ToString();
                User_Table uid = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
                obj1.Userid = uid.UserId;
                obj1.OrderCreatedBy = name;
                obj1.OrderUpdatedBy = name;
                obj1.OrderCreatedDate = System.DateTime.Now;
                obj1.OrderUpdatedDate = System.DateTime.Now;
                db.Order_Table.Add(obj1);
                db.SaveChanges();
                Order_Table obj2 = db.Order_Table.Where(x => x.OrderStatus == 0 & x.OrderIsDeleted == false & x.Userid==uid.UserId).FirstOrDefault();
                OrderDetail_Table detail_obj = new OrderDetail_Table();
                detail_obj.Orderid = obj2.OrderId;
                detail_obj.Productid = id;
                detail_obj.Quantity = 1;
                detail_obj.Amount = db.Product_Table.Where(x => x.ProductId == id).Select(x => x.ProductPrice).FirstOrDefault();
                db.OrderDetail_Table.Add(detail_obj);
                db.SaveChanges();
            }
            else
            {
                bool flag = false;
                var check = db.OrderDetail_Table.Where(x => x.Orderid == obj.OrderId).Select(x => x.Productid).ToList();
                foreach(var item in check)
                {
                    if(item==id)
                    {
                        flag = true;
                    }
                }
                if(flag==false)
                {
                    OrderDetail_Table detail_obj = new OrderDetail_Table();
                    detail_obj.Orderid = obj.OrderId;
                    detail_obj.Productid = id;
                    detail_obj.Quantity = 1;
                    detail_obj.Amount = db.Product_Table.Where(x => x.ProductId == id).Select(x => x.ProductPrice).FirstOrDefault();
                    db.OrderDetail_Table.Add(detail_obj);
                    db.SaveChanges();
                }
            
            }

    
            count_cart();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Product_page", "Buyer");
            return Json(new { Url = redirectUrl }, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public JsonResult remove_cart(int id)
        {
            OrderDetail_Table obj = db.OrderDetail_Table.Where(x => x.Productid == id ).FirstOrDefault();
            db.OrderDetail_Table.Remove(obj);
            db.SaveChanges();
            OrderDetail_Table obj1 = db.OrderDetail_Table.Where(x => x.Orderid == obj.Orderid).FirstOrDefault();
            if(obj1==null)
            {
                Order_Table obj2 = db.Order_Table.Where(x => x.OrderId == obj.Orderid).FirstOrDefault();
                obj2.OrderIsDeleted = true;
                db.SaveChanges();
            }
            count_cart();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("cart", "Buyer");
            return Json(new { Url = redirectUrl }, JsonRequestBehavior.AllowGet);
        }


        public void count_cart()
        {

            string name = Session["user"].ToString();
            int id = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            int oid = db.Order_Table.Where(x => x.Userid == id & x.OrderStatus == 0 & x.OrderIsDeleted == false).Select(x=>x.OrderId).FirstOrDefault();
            int count = db.OrderDetail_Table.Where(x => x.Orderid == oid).Count();
            Session["count"] = count;
        }

        public void check_stock()
        {
            try
            {
                int flag = 0;
                List<string> proname = new List<string>();
                string name = Session["user"].ToString();
                int id = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
                List<int> nlist = db.Notify_table.Where(x => x.Userid == id && x.flag == 0).Select(x => x.Productid).ToList();
                foreach (int item in nlist)
                {
                    Notify_table nobj = db.Notify_table.Where(x => x.Userid == id && x.Productid == item).FirstOrDefault();
                    Product_Table pobj = db.Product_Table.Where(x => x.ProductId == item).FirstOrDefault();
                    if (pobj.ProductStock > 0)
                    {
                        flag = 1;
                        proname.Add(pobj.ProductName);
                        //nobj.flag = 1;
                        db.SaveChanges();
                    }
                }
                if (flag == 1)
                {
                    ViewBag.stock_list = proname;
                }
                else
                {
                    ViewBag.stock_list = null;
                }
            }
            catch
            {
                Response.Redirect("~/User/login");
            }
        }

        public void del_check_stock()
        {
           
            string name = Session["user"].ToString();
            int id = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            List<int> nlist = db.Notify_table.Where(x => x.Userid == id && x.flag == 0).Select(x => x.Productid).ToList();
            foreach (int item in nlist)
            {
                Notify_table nobj = db.Notify_table.Where(x => x.Userid == id && x.Productid == item).FirstOrDefault();
                Product_Table pobj = db.Product_Table.Where(x => x.ProductId == item).FirstOrDefault();
                if (pobj.ProductStock > 0)
                {
                    
                    nobj.flag = 1;
                    db.SaveChanges();
                }
            }
            
        }

        [HttpGet]
        public ActionResult OrderHistoryList()
        {
            string name = Session["user"].ToString();
            int id = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            var orderHistory = db.Order_Table.Where(x => x.Userid == id & x.OrderStatus == 1).ToList();
            return View(orderHistory.ToList());
        }

        public ActionResult OrderHistoryDetails(int id)
        {


            List<OrderHistory_ViewModel> ohvmlist = new List<OrderHistory_ViewModel>();
            var obj = db.OrderDetail_Table.Where(x => x.Orderid == id).ToList();
            Service_Table s = new Service_Table();
            User_Table uobj = new User_Table();
            foreach (var item in obj)
            {

                var service = db.OrderDetail_Table.Where(x => x.Orderid == id).Select(x => x.Serviceid).FirstOrDefault();
                // var servicename = db.Service_Table.Where(x => x.ServiceId == service).Select(x => x.ServiceName).FirstOrDefault();
                s = db.Service_Table.Where(x => x.ServiceId == service).FirstOrDefault();
                uobj = db.User_Table.Where(x => x.UserId == s.ServiceProviderid).FirstOrDefault();
                var product_desc = db.Product_Table.Where(x => x.ProductId == item.Productid).Select(x => x.ProductDesc).FirstOrDefault();
                var product = db.Product_Table.Where(x => x.ProductId == item.Productid).Select(x => x.ProductName).FirstOrDefault();
                var deliveryadd = db.Order_Table.Where(x => x.OrderId == id).Select(x => x.OrderDeliveryAddress).FirstOrDefault();
                var deliverydate = db.Order_Table.Where(x => x.OrderId == id).Select(x => x.OrderDeliveryDate).FirstOrDefault();
                var image = db.Image_Table.Where(x => x.Productid == item.Productid).Select(x => x.BinaryImage).FirstOrDefault();
                OrderHistory_ViewModel obj1 = new OrderHistory_ViewModel();
                obj1.ProductName = product;
                obj1.ProductDesc = product_desc;
                obj1.OrderDelivryAddress = deliveryadd;
                obj1.Amount = (decimal)item.Amount;
                obj1.OrderDeliveryDate = (DateTime)deliverydate;
                obj1.ServiceName = uobj.UserName;
                obj1.BinaryImage = image;
                ohvmlist.Add(obj1);
            }


            return View(ohvmlist);
        }

        [HttpGet]
        public ActionResult notification()
        {
           
            string name = Session["user"].ToString();
            int id = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            DateTime today = System.DateTime.Now;
            var delivery_list = db.Order_Table.Where(x => x.Userid == id & x.OrderStatus == 1 & x.OrderDeliveryDate>today).ToList();
            return View(delivery_list.ToList());
        }

        [HttpPost]
        public JsonResult search(string name)
        {
            int res = 0;
            BaseCategory_Table obj = db.BaseCategory_Table.Where(x => x.BaseCatName == name && x.BaseCatIsDeleted==false).FirstOrDefault();
            ProductCategory_Table obj1 = db.ProductCategory_Table.Where(x => x.ProductCatName == name && x.ProductCatIsDeleted==false).FirstOrDefault();
            User_Table obj2 = db.User_Table.Where(x => x.UserName == name && x.UserIsDeleted == false).FirstOrDefault();
            if (obj != null)
            {
                res = 1;
                Session["base_cat"] = obj.BaseCatId;
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Product_cat", "Buyer");
                return Json(new {res, Url = redirectUrl }, JsonRequestBehavior.AllowGet);

            }
            else if (obj1 != null)
            {
                res = 1;
                Session["prod_cat"] = obj1.ProductCatId;
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Product_page", "Buyer");
                return Json(new {res, Url = redirectUrl }, JsonRequestBehavior.AllowGet);

            }
            else if(obj2!=null)
            {
                res = 1;
                Session["brand_id"] = obj2.UserId;
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Brand_page", "Buyer");
                return Json(new { res, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                
                return Json(new {res}, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public JsonResult find_pro_cat_id(int id)
        {
            Session["prod_cat"] = id;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Product_page", "Buyer");
            return Json(new { Url = redirectUrl }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult find_base_cat_id(int id)
        {
            Session["base_cat"] = id;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Product_cat", "Buyer");
            return Json(new { Url = redirectUrl }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult remind(int id)
        {
            string name = Session["user"].ToString();
            User_Table obj = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
            Notify_table nobj = db.Notify_table.Where(x => x.Userid == obj.UserId && x.Productid == id && x.flag == 0).FirstOrDefault();
            if(nobj!=null)
            { 
           
            }
            else
            {
                Notify_table nobj1 = new Notify_table();
                nobj1.Userid = obj.UserId;
                nobj1.Productid = id;
                nobj1.flag = 0;
                db.Notify_table.Add(nobj1);
                db.SaveChanges();
            }
            return Json(new {  }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult fill1()
        {
            Session["filter1"] = 1;
            Session["filter2"] = 0;
            return Json(new { }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult fill2()
        {
            Session["filter2"] = 1;
            Session["filter1"] = 0;
            return Json(new { }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ChangePassword()
        {
            ViewBag.message = TempData["message"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
           
            User_Table obj = new User_Table();
            string name = Session["user"].ToString();
            User_Table details = (from a in db.User_Table where a.UserName == name select a).FirstOrDefault();
            if (details.Password == model.OldPassword)
            {
                if(details.Password==model.NewPassword)
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
            del_check_stock();
            Session["user"] = null;
            Session["count"] = null;
            Session.Abandon();
            Response.Redirect("~/User/login");
        }

        

        
    }
}