using Online_SHopping_Cart.Models;
using Online_SHopping_Cart.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Online_SHopping_Cart.Controllers
{
    public class BuyerController : Controller
    {
        //Model Object
        ShoppingCartDbEntities db = new ShoppingCartDbEntities();
        public List<int> avail_list = new List<int>();
        // GET: Buyer


        #region Home Page

        /// <summary>
        /// Home Page of Buyer
        /// </summary>
        /// <returns></returns>
        /// 

        public ActionResult Index()
        {
            try
            {
                check_stock();
                List<BaseCategory_Table> cato = new List<BaseCategory_Table>();
                cato = db.BaseCategory_Table.Where(x => x.BaseCatIsDeleted == false).ToList();

                Session["filter1"] = 0;
                Session["filter2"] = 0;

                List<Product_Table> NewProducts = db.Product_Table.OrderByDescending(x => x.ProductId).Take(3).ToList();
                List<Buyer_Product> ProductList = new List<Buyer_Product>();
                foreach (var item in NewProducts)
                {
                    Buyer_Product Product = new Buyer_Product();
                    Product.ProductName = item.ProductName;
                    Product.ProductId = item.ProductId;
                    Product.ProductPrice = item.ProductPrice;
                    Product.ProductDesc = item.ProductDesc;
                    Image_Table img = db.Image_Table.Where(x => x.Productid == item.ProductId).FirstOrDefault();
                    if (img != null)
                    {
                        Product.BinaryImage = img.BinaryImage;
                        ProductList.Add(Product);
                    }

                }
                ViewBag.newpro = ProductList;

                return View(cato);
            }
            catch
            {
                Response.Redirect("~/User/Error");
            }
            return View();
        }

        #endregion

        #region User Profile

        /// <summary>
        /// Users is able to View and update User Profile
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult profile()
        {
            try
            {
                ViewBag.EnterDetails = TempData["EnterDetails"];
                string name = Session["Buyer"].ToString();
                User_Table User = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
                return View(User);
            }
            catch
            {
                return View("Error");
            }
           
        }

        [HttpPost]
        public ActionResult profile(User_Table UserDetail)
        {
            try
            {
                if (UserDetail.FirstName != null && UserDetail.LastName != null && UserDetail.UserEmail != null && UserDetail.UserAddress != null && UserDetail.UserPhno != null)
                {
                    string name = Session["Buyer"].ToString();
                    User_Table user = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
                    user.FirstName = UserDetail.FirstName;
                    user.LastName = UserDetail.LastName;
                    user.UserEmail = UserDetail.UserEmail;
                    user.UserAddress = UserDetail.UserAddress;
                    user.UserPhno = UserDetail.UserPhno;
                    user.UserUpdatedDate = System.DateTime.Now;
                    db.SaveChanges();
                }
                else
                {
                    TempData["EnterDetails"] = ConstantFile.EnterDetails;
                    return RedirectToAction("profile");
                }
            }
            catch
            {
                return View("Error");
            }
            return View();
        }

        #endregion

        #region Product Category

        /// <summary>
        /// Shows Product based on Base Category 
        /// </summary>
        /// <returns></returns>


        public ActionResult Product_cat()
        {
            try
            {
                int BaseId = (int)Session["base_cat"];

                List<ProductCategory_Table> ProductCategory = new List<ProductCategory_Table>();
                ProductCategory = db.ProductCategory_Table.Where(x => x.BaseCatid == BaseId & x.ProductCatIsDeleted == false).ToList();
                return View(ProductCategory);
            }
            catch
            {
                return View("Error");
            }
            

        }

        /// <summary>
        /// Shows Product based on Product Category 
        /// </summary>
        /// <returns></returns>

        public ActionResult Product_page()
        {
            try
            {
                int ProductCategoryId = (int)Session["prod_cat"];
                int? filter1 = (int)Session["filter1"];
                int? filter2 = (int)Session["filter2"];
                List<Product_Table> Products = new List<Product_Table>();
                if (filter1 == 1)
                {
                    Products = db.Product_Table.Where(x => x.ProductCatid == ProductCategoryId & x.ProductIsDeleted == false).OrderBy(x => x.ProductPrice).ToList();
                }
                else if (filter2 == 1)
                {
                    Products = db.Product_Table.Where(x => x.ProductCatid == ProductCategoryId & x.ProductIsDeleted == false).OrderByDescending(x => x.ProductPrice).ToList();
                }
                else
                {
                    Products = db.Product_Table.Where(x => x.ProductCatid == ProductCategoryId & x.ProductIsDeleted == false).ToList();
                }
                List<int> not_avail_product = new List<int>();
                List<Buyer_Product> ProductDetailList = new List<Buyer_Product>();
                foreach (var item in Products)
                {
                    Buyer_Product ProductObject = new Buyer_Product();
                    ProductObject.ProductName = item.ProductName;
                    ProductObject.ProductId = item.ProductId;
                    ProductObject.ProductPrice = item.ProductPrice;
                    ProductObject.ProductDesc = item.ProductDesc;
                    if (item.ProductStock <= 0)
                    {
                        not_avail_product.Add(item.ProductId);
                    }
                    Image_Table img = db.Image_Table.Where(x => x.Productid == item.ProductId && x.ImageIsDeleted == false).FirstOrDefault();
                    if (img != null)
                    {
                        ProductObject.BinaryImage = img.BinaryImage;
                        ProductDetailList.Add(ProductObject);
                    }

                }
                ViewBag.no_stock = not_avail_product;
                Session["filter1"] = 0;
                Session["filter2"] = 0;
                return View(ProductDetailList);
            }
            catch
            {
                return View("Error");
            }
           
        }

        /// <summary>
        /// Shows Product based on Seller Name
        /// </summary>
        /// <returns></returns>


        public ActionResult Brand_page()
        {
            try
            {
                int SellerId = (int)Session["brand_id"];
                int? filter1 = (int)Session["filter1"];
                int? filter2 = (int)Session["filter2"];
                List<Product_Table> Products = new List<Product_Table>();
                if (filter1 == 1)
                {
                    Products = db.Product_Table.Where(x => x.SellerId == SellerId & x.ProductIsDeleted == false).OrderBy(x => x.ProductPrice).ToList();
                }
                else if (filter2 == 1)
                {
                    Products = db.Product_Table.Where(x => x.SellerId == SellerId & x.ProductIsDeleted == false).OrderByDescending(x => x.ProductPrice).ToList();
                }
                else
                {
                    Products = db.Product_Table.Where(x => x.SellerId == SellerId & x.ProductIsDeleted == false).ToList();
                }
                List<int> not_avail_product = new List<int>();
                List<Buyer_Product> ProductDetailList = new List<Buyer_Product>();
                foreach (var item in Products)
                {
                    Buyer_Product ProductObject = new Buyer_Product();
                    ProductObject.ProductName = item.ProductName;
                    ProductObject.ProductId = item.ProductId;
                    ProductObject.ProductPrice = item.ProductPrice;
                    ProductObject.ProductDesc = item.ProductDesc;
                    if (item.ProductStock <= 0)
                    {
                        not_avail_product.Add(item.ProductId);
                    }
                    Image_Table img = db.Image_Table.Where(x => x.Productid == item.ProductId && x.ImageIsDeleted == false).FirstOrDefault();
                    ProductObject.BinaryImage = img.BinaryImage;
                    ProductDetailList.Add(ProductObject);
                }
                ViewBag.no_stock = not_avail_product;
                Session["filter1"] = 0;
                Session["filter2"] = 0;
                return View(ProductDetailList);
            }
            catch
            {
                return View("Error");
            }
            
        }

        #endregion

        #region Product Purchase

        /// <summary>
        /// Order methos allow user to Purchase Products
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult order(int id)
        {
            try
            {
                Session["pro_id"] = id;
                List<Location_Table> location = new List<Location_Table>();
                location = db.Location_Table.ToList();
                var LocationList = new List<SelectListItem>();
                foreach (var item in location)
                {
                    LocationList.Add(new SelectListItem
                    {
                        Text = item.LocationName.ToString(),
                        Value = item.LocationId.ToString(),

                    });

                    ViewBag.location_list = LocationList;

                }

                Book BookingProduct = new Book();
                Product_Table Product = db.Product_Table.Where(x => x.ProductId == id).FirstOrDefault();
                BookingProduct.ProductName = Product.ProductName;
                BookingProduct.ProductDesc = Product.ProductDesc;
                BookingProduct.ProductPrice = Product.ProductPrice;
                BookingProduct.ProductStock = Product.ProductStock;
                ViewBag.image_list = db.Image_Table.Where(x => x.Productid == id && x.ImageIsDeleted == false).ToList();
                return View(BookingProduct);
            }
            catch
            {
                return View("Error");
            }
           

        }

        [HttpPost]
        public ActionResult order(Book BookingObject, string amt)
        {
            try
            {
                string name = Session["Buyer"].ToString();
                int ProductId = Convert.ToInt32(Session["pro_id"].ToString());
                int stock = db.Product_Table.Where(x => x.ProductId == ProductId).Select(x => x.ProductStock).FirstOrDefault();
                if (BookingObject.Quantity <= stock)
                {
                    Order_Table order = new Order_Table();
                    OrderDetail_Table order_detail = new OrderDetail_Table();
                    order.TotalAmount = Convert.ToInt32(amt);
                    order.OrderDeliveryAddress = BookingObject.OrderDelivryAddress;
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
                    int orderid = order.OrderId;
                    order_detail.Orderid = orderid;
                    order_detail.Productid = ProductId;
                    int locationid = Convert.ToInt32(Session["location"]);
                    Service_Table serboj = db.Service_Table.Where(x => x.ServiceProviderid == BookingObject.UserId && x.Locationid == locationid).FirstOrDefault();
                    order_detail.Serviceid = serboj.ServiceId;
                    order_detail.Quantity = BookingObject.Quantity;
                    order_detail.Amount = Convert.ToInt32(amt);
                    db.OrderDetail_Table.Add(order_detail);
                    db.SaveChanges();
                    Product_Table pobj = db.Product_Table.Where(x => x.ProductId == ProductId).FirstOrDefault();
                    pobj.ProductStock = stock - BookingObject.Quantity;
                    db.SaveChanges();
                }
                Session["location"] = null;
                return RedirectToAction("notification");
            }
            catch
            {
                return View("Error");
            }
           
        }

        #endregion

        #region Cart Purchase

        /// <summary>
        /// purchase_all allows user to Purchase Products from cart
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult purchase_all()
        {
            try
            {
                List<Location_Table> location = new List<Location_Table>();
                location = db.Location_Table.ToList();
                var LocationList = new List<SelectListItem>();
                foreach (var item in location)
                {
                    LocationList.Add(new SelectListItem
                    {
                        Text = item.LocationName.ToString(),
                        Value = item.LocationId.ToString(),

                    });

                    ViewBag.location_list = LocationList;

                }
            }
            catch
            {
                return View("Error");
            }
            return View();
        }

        [HttpPost]
        public ActionResult purchase_all(Book BookingObject)
        {
            try
            {
                string name = Session["Buyer"].ToString();
                int userid = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();

                Order_Table order = db.Order_Table.Where(x => x.OrderStatus == 0 & x.OrderIsDeleted == false & x.Userid == userid).FirstOrDefault();
                order.OrderDeliveryAddress = BookingObject.OrderDelivryAddress;
                order.TotalAmount = Convert.ToInt32(TempData["tcart_amt"]);
                order.OrderNotification = "00";
                order.OrderDeliveryDate = System.DateTime.Now.AddDays(5);
                order.OrderStatus = 1;
                db.SaveChanges();
                var order_detail = db.OrderDetail_Table.Where(x => x.Orderid == order.OrderId).Select(x => x.OrderDetailId).ToList();
                foreach (var item in order_detail)
                {
                    int flag = 0;
                    List<int> availid1 = TempData["avail1"] as List<int>;
                    OrderDetail_Table OrderDetail = db.OrderDetail_Table.Where(x => x.OrderDetailId == item).FirstOrDefault();
                    foreach (var productid in availid1)
                    {
                        if (OrderDetail.Productid == productid)
                        {
                            Product_Table Product = db.Product_Table.Where(x => x.ProductId == productid).FirstOrDefault();
                            int stock = Convert.ToInt32(Product.ProductStock);
                            int qty = Convert.ToInt32(OrderDetail.Quantity);
                            Product.ProductStock = stock - qty;
                            db.SaveChanges();
                            flag = 1;
                        }
                    }
                    if (flag == 1)
                    {
                        int locationid = Convert.ToInt32(Session["location"]);
                        Service_Table Service = db.Service_Table.Where(x => x.ServiceProviderid == BookingObject.UserId && x.Locationid == locationid).FirstOrDefault();
                        OrderDetail.Serviceid = Service.ServiceId;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.OrderDetail_Table.Remove(OrderDetail);
                        db.SaveChanges();
                    }
                }
                count_cart();
                Session["location"] = null;
                return RedirectToAction("notification");
            }
            catch
            {
                return View("Error");
            }
           
        }

        #endregion

        #region Find Service Provise

        /// <summary>
        /// service_name and service_name_all find service providers for products based on location
        /// </summary>
        /// <returns></returns>

        public ActionResult service_name(string id)
        {
            try
            {
                Session["location"] = id;
                int productid = Convert.ToInt32(Session["pro_id"].ToString());
                int LocationId;
                List<SelectListItem> ServiceList = new List<SelectListItem>();
                if (!string.IsNullOrEmpty(id))
                {
                    LocationId = Convert.ToInt32(id);
                    List<Service_Table> Service = db.Service_Table.Where(x => x.Locationid == LocationId & x.Productid == productid).ToList();
                    foreach (var item in Service)
                    {
                        User_Table User = db.User_Table.Where(x => x.UserId == item.ServiceProviderid).FirstOrDefault();
                        ServiceList.Add(new SelectListItem
                        {
                            Text = User.UserName.ToString(),
                            Value = User.UserId.ToString(),

                        });
                    }

                    ViewBag.procat = ServiceList;
                }
                return Json(ServiceList, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View("Error");
            }
            
        }

        public ActionResult service_name_all(string id)
        {
            try
            {
                Session["location"] = id;
                List<SelectListItem> ServiceList = new List<SelectListItem>();
                List<Service_Table> service_list = new List<Service_Table>();

                int LocationId = Convert.ToInt32(id);
                // var ser = db.Service_Table.Where(x => x.Locationid == locId).Select(x => x.ServiceProviderid).ToList();
                var service = db.Service_Table.Where(x => x.Locationid == LocationId).ToList();
                int previd = 0;
                foreach (var items in service)
                {
                    if (previd != items.ServiceProviderid)
                    {
                        Service_Table ServiceObject = db.Service_Table.Where(x => x.ServiceProviderid == items.ServiceProviderid).FirstOrDefault();
                        previd = ServiceObject.ServiceProviderid;
                        service_list.Add(ServiceObject);
                    }

                }
                int total_pro = Convert.ToInt32(TempData["count"]);
                List<int> availid = TempData["avail"] as List<int>;


                foreach (var SelectedService in service_list)
                {
                    int count = 0;
                    foreach (var AvailableProducts in availid)
                    {
                        int ProductId = Convert.ToInt32(AvailableProducts);
                        Service_Table ServiceObject = db.Service_Table.Where(x => x.Productid == ProductId & x.ServiceProviderid == SelectedService.ServiceProviderid).FirstOrDefault();
                        if (ServiceObject != null)
                        {
                            count++;
                        }
                    }
                    if (count == total_pro)
                    {
                        string sname = db.Service_Table.Where(x => x.ServiceProviderid == SelectedService.ServiceProviderid).Select(x => x.ServiceName).FirstOrDefault();
                        User_Table UserObject = db.User_Table.Where(x => x.UserId == SelectedService.ServiceProviderid).FirstOrDefault();
                        ServiceList.Add(new SelectListItem
                        {
                            Text = UserObject.UserName,
                            Value = UserObject.UserId.ToString(),

                        });
                    }
                }


                return Json(ServiceList, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View("Error");
            }
            

        }

        #endregion

        #region Calculate Price

        /// <summary>
        /// Calculates the Booking Price based on Quantity , price , Delivery Cahrge
        /// </summary>
        /// <returns></returns>

        public JsonResult calculate_amt(int qty, int service_id)
        {

                int ProductId = Convert.ToInt32(Session["pro_id"].ToString());
                int stock = db.Product_Table.Where(x => x.ProductId == ProductId).Select(x => x.ProductStock).FirstOrDefault();
                int quantity = Convert.ToInt32(qty);
                decimal total = 0;
                if (quantity <= stock)
                {
                    int ServiceId = Convert.ToInt32(service_id);
                    decimal del_charge = db.Service_Table.Where(x => x.ServiceProviderid == ServiceId & x.Productid == ProductId).Select(x => x.DeliveryCharge).FirstOrDefault();
                    decimal price = db.Product_Table.Where(x => x.ProductId == ProductId).Select(x => x.ProductPrice).FirstOrDefault();
                    total = (quantity * price) + del_charge;
                    return Json(new { total, del_charge }, JsonRequestBehavior.AllowGet);
                }
                return Json(total, JsonRequestBehavior.AllowGet);  
        }
        #endregion

        #region Cart

        /// <summary>
        /// Cart Page
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult cart()
        {
            try
            {
                float total = 0;
                int count = 0;
                string name = Session["Buyer"].ToString();
                List<Buyer_Product> ProductList = new List<Buyer_Product>();
                int UserId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
                int OrderId = db.Order_Table.Where(x => x.Userid == UserId & x.OrderStatus == 0 & x.OrderIsDeleted == false).Select(x => x.OrderId).FirstOrDefault();
                var ProductId = db.OrderDetail_Table.Where(x => x.Orderid == OrderId).Select(x => x.Productid).ToList();
                List<int> avail_product = new List<int>();
                List<int> not_avail_product = new List<int>();
                foreach (var product_id in ProductId)
                {
                    Product_Table Product = db.Product_Table.Where(x => x.ProductId == product_id).FirstOrDefault();
                    Buyer_Product CartObject = new Buyer_Product();
                    CartObject.ProductName = Product.ProductName;
                    CartObject.ProductId = Product.ProductId;
                    CartObject.ProductPrice = Product.ProductPrice;
                    CartObject.ProductDesc = Product.ProductDesc;
                    if (Product.ProductStock > 0)
                    {
                        total += (float)Product.ProductPrice;
                        avail_list.Add(Product.ProductId);
                        avail_product.Add(Product.ProductId);
                        count++;
                    }
                    else
                    {
                        not_avail_product.Add(Product.ProductId);
                    }
                    Image_Table img = db.Image_Table.Where(x => x.Productid == product_id && x.ImageIsDeleted == false).FirstOrDefault();
                    if (img != null)
                    {
                        CartObject.BinaryImage = img.BinaryImage;
                        ProductList.Add(CartObject);
                    }

                }
                TempData["count"] = count;
                TempData["tcart_amt"] = total;
                ViewBag.avail = avail_product;
                ViewBag.tcart_amt = total;
                TempData["avail"] = avail_product;
                TempData.Keep("avail");
                TempData["avail1"] = avail_product;
                ViewBag.not_avail = not_avail_product;
                return View(ProductList);
            }
            catch
            {
                return View("Error");
            }
            
        }

        /// <summary>
        /// Products are added to cart
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public JsonResult add_cart(int id)
        {

                string user = Session["Buyer"].ToString();
                int UserId = db.User_Table.Where(x => x.UserName == user).Select(x => x.UserId).FirstOrDefault();
                Order_Table OrderObject = db.Order_Table.Where(x => x.OrderStatus == 0 & x.OrderIsDeleted == false & x.Userid == UserId).FirstOrDefault();
                if (OrderObject == null)
                {
                    Order_Table Order = new Order_Table();
                    Order.OrderStatus = 0;
                    Order.OrderIsDeleted = false;
                    string name = Session["Buyer"].ToString();
                    User_Table uid = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
                    Order.Userid = uid.UserId;
                    Order.OrderCreatedBy = name;
                    Order.OrderUpdatedBy = name;
                    Order.OrderCreatedDate = System.DateTime.Now;
                    Order.OrderUpdatedDate = System.DateTime.Now;
                    db.Order_Table.Add(Order);
                    db.SaveChanges();
                    OrderDetail_Table detail_obj = new OrderDetail_Table();
                    detail_obj.Orderid = Order.OrderId;
                    detail_obj.Productid = id;
                    detail_obj.Quantity = 1;
                    detail_obj.Amount = db.Product_Table.Where(x => x.ProductId == id).Select(x => x.ProductPrice).FirstOrDefault();
                    db.OrderDetail_Table.Add(detail_obj);
                    db.SaveChanges();
                }
                else
                {
                    bool flag = false;
                    var check = db.OrderDetail_Table.Where(x => x.Orderid == OrderObject.OrderId).Select(x => x.Productid).ToList();
                    foreach (var item in check)
                    {
                        if (item == id)
                        {
                            flag = true;
                        }
                    }
                    if (flag == false)
                    {
                        OrderDetail_Table detail_obj = new OrderDetail_Table();
                        detail_obj.Orderid = OrderObject.OrderId;
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

        /// <summary>
        /// Products are removed from cart
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public JsonResult remove_cart(int id)
        {
           
                Order_Table Order = db.Order_Table.Where(x => x.OrderStatus == 0 & x.OrderIsDeleted).FirstOrDefault();
                OrderDetail_Table DetailObject = db.OrderDetail_Table.Where(x => x.Productid == id && x.Orderid == Order.OrderId).FirstOrDefault();
                db.OrderDetail_Table.Remove(DetailObject);
                db.SaveChanges();
                OrderDetail_Table Detail = db.OrderDetail_Table.Where(x => x.Orderid == DetailObject.Orderid).FirstOrDefault();
                if (Detail == null)
                {

                    Order.OrderIsDeleted = true;
                    db.SaveChanges();
                }
                count_cart();
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("cart", "Buyer");
                return Json(new { Url = redirectUrl }, JsonRequestBehavior.AllowGet);
 
        }

        /// <summary>
        /// Finds number of products in the cart
        /// </summary>
        /// <returns></returns>

        public void count_cart()
        {
            string name = Session["Buyer"].ToString();
            int UserId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            int OrderId = db.Order_Table.Where(x => x.Userid == UserId & x.OrderStatus == 0 & x.OrderIsDeleted == false).Select(x => x.OrderId).FirstOrDefault();
            int count = db.OrderDetail_Table.Where(x => x.Orderid == OrderId).Count();
            Session["count"] = count;
        }

        #endregion

        #region Reminder

        /// <summary>
        /// Sets reminder for a selected Product
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public JsonResult remind(int id)
        {
                string name = Session["Buyer"].ToString();
                User_Table User = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
                Notify_table NotifyCheck = db.Notify_table.Where(x => x.Userid == User.UserId && x.Productid == id && x.flag == 0).FirstOrDefault();
                if (NotifyCheck == null)
                {
                    Notify_table Notify = new Notify_table();
                    Notify.Userid = User.UserId;
                    Notify.Productid = id;
                    Notify.flag = 0;
                    db.Notify_table.Add(Notify);
                    db.SaveChanges();
                }
                return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Checks if products stock is available
        /// </summary>

        public void check_stock()
        {

                int flag = 0;
                List<string> ProductName = new List<string>();
                string name = Session["Buyer"].ToString();
                int UserId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
                List<int> NotifyList = db.Notify_table.Where(x => x.Userid == UserId && x.flag == 0).Select(x => x.Productid).ToList();
                foreach (int item in NotifyList)
                {
                    Product_Table Product = db.Product_Table.Where(x => x.ProductId == item).FirstOrDefault();
                    if (Product.ProductStock > 0)
                    {
                        flag = 1;
                        ProductName.Add(Product.ProductName);

                        db.SaveChanges();
                    }
                }
                if (flag == 1)
                {
                    ViewBag.stock_list = ProductName;
                }
                else
                {
                    ViewBag.stock_list = null;
                }

        }

        /// <summary>
        /// Removes reminder after user sees it
        /// </summary>

        public void del_check_stock()
        {

            string name = Session["Buyer"].ToString();
            int UserId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            List<int> NotifyList = db.Notify_table.Where(x => x.Userid == UserId && x.flag == 0).Select(x => x.Productid).ToList();
            foreach (int item in NotifyList)
            {
                Notify_table Notify = db.Notify_table.Where(x => x.Userid == UserId && x.Productid == item).FirstOrDefault();
                Product_Table Product = db.Product_Table.Where(x => x.ProductId == item).FirstOrDefault();
                if (Product.ProductStock > 0)
                {

                    Notify.flag = 1;
                    db.SaveChanges();
                }
            }

        }

        #endregion

        #region Order History and Notifivcation

        /// <summary>
        /// Displays all the Orders 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult OrderHistoryList()
        {
            try
            {
                string name = Session["Buyer"].ToString();
                int id = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
                var orderHistory = db.Order_Table.Where(x => x.Userid == id & x.OrderStatus == 1).ToList();
                return View(orderHistory.ToList());
            }
            catch
            {
                return View("Error");
            }
           
        }

        /// <summary>
        /// Shows the Order Details 
        /// </summary>
        /// <returns></returns>

        public ActionResult OrderHistoryDetails(int id)
        {

            try
            {
                List<OrderHistory_ViewModel> ohvmlist = new List<OrderHistory_ViewModel>();
                var obj = db.OrderDetail_Table.Where(x => x.Orderid == id).ToList();
                Service_Table s = new Service_Table();
                User_Table uobj = new User_Table();
                foreach (var item in obj)
                {

                    var service = db.OrderDetail_Table.Where(x => x.Orderid == id).Select(x => x.Serviceid).FirstOrDefault();
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
            catch
            {
                return View("Error");
            }
          
        }

        /// <summary>
        /// Displays notification with the delivery date when a booking is made  
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult notification()
        {
            try
            {
                string name = Session["Buyer"].ToString();
                int UderId = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
                DateTime today = System.DateTime.Now;
                var delivery_list = db.Order_Table.Where(x => x.Userid == UderId & x.OrderStatus == 1 & x.OrderDeliveryDate > today).ToList();
                return View(delivery_list.ToList());
            }
            catch
            {
                return View("Error");
            }
            
        }

        #endregion

        #region Search

        /// <summary>
        /// search bar operation
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public JsonResult search(string name)
        {          
                int res = 0;
                BaseCategory_Table Base = db.BaseCategory_Table.Where(x => x.BaseCatName == name && x.BaseCatIsDeleted == false).FirstOrDefault();
                ProductCategory_Table ProductCategory = db.ProductCategory_Table.Where(x => x.ProductCatName == name && x.ProductCatIsDeleted == false).FirstOrDefault();
                User_Table User = db.User_Table.Where(x => x.UserName == name && x.UserIsDeleted == false).FirstOrDefault();
                if (Base != null)
                {
                    res = 1;
                    Session["base_cat"] = Base.BaseCatId;
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("Product_cat", "Buyer");
                    return Json(new { res, Url = redirectUrl }, JsonRequestBehavior.AllowGet);

                }
                else if (ProductCategory != null)
                {
                    res = 1;
                    Session["prod_cat"] = ProductCategory.ProductCatId;
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("Product_page", "Buyer");
                    return Json(new { res, Url = redirectUrl }, JsonRequestBehavior.AllowGet);

                }
                else if (User != null)
                {
                    res = 1;
                    Session["brand_id"] = User.UserId;
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("Brand_page", "Buyer");
                    return Json(new { res, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    return Json(new { res }, JsonRequestBehavior.AllowGet);
                }
 
        }

        /// <summary>
        /// Find the selected Product Category Id
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public JsonResult find_pro_cat_id(int id)
        {

                Session["prod_cat"] = id;
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Product_page", "Buyer");
                return Json(new { Url = redirectUrl }, JsonRequestBehavior.AllowGet);
        
        }

        /// <summary>
        /// Find the selected Base Category Id
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public JsonResult find_base_cat_id(int id)
        {

                Session["base_cat"] = id;
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Product_cat", "Buyer");
                return Json(new { Url = redirectUrl }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Filters

        /// <summary>
        /// Filtering of Products from Price Low - High
        /// </summary>
        /// <returns></returns>

        public JsonResult fill1()
        {
                Session["filter1"] = 1;
                Session["filter2"] = 0;
                return Json(new { }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Filtering of Products from Price High - Low
        /// </summary>
        /// <returns></returns>

        public JsonResult fill2()
        {
                Session["filter2"] = 1;
                Session["filter1"] = 0;
                return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Change Password

        /// <summary>
        /// Allows user to Cahnge Password
        /// </summary>
        /// <returns></returns>

        public ActionResult ChangePassword()
        {
            ViewBag.ChangePassword = TempData["ChangePassword"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                string name = Session["Buyer"].ToString();
                User_Table details = (from a in db.User_Table where a.UserName == name select a).FirstOrDefault();
                if (details.Password == model.OldPassword)
                {
                    if (details.Password == model.NewPassword)
                    {
                        TempData["ChangePassword"] = ConstantFile.PasswordConflict;
                    }
                    else if (model.NewPassword == model.ConfirmPassword)
                    {
                        details.Password = model.NewPassword;
                        db.SaveChanges();
                        TempData["ChangePassword"] = ConstantFile.PasswordChangeSuccess;
                    }
                    else
                    {
                        TempData["ChangePassword"] = ConstantFile.ConfirmPasswordConflict;
                    }
                }
                else
                {
                    TempData["ChangePassword"] = ConstantFile.OldPassworWrong;
                }
                return RedirectToAction("ChangePassword");
            }
            catch
            {
                return View("Error");
            }
           
        }

        #endregion

        #region Logout

        /// <summary>
        /// Logout's back to Login page
        /// </summary>

        public void Logout()
        {
            Session["Buyer"] = null;
            Session.Abandon();
            Response.Redirect("~/User/login");
        }

        #endregion

        #region Error

        /// <summary>
        /// Custom Error Page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }
        #endregion


    }
}