﻿using Common;
using Data.Admin;
using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class LoginController : Controller
    {
        SysLogDA _sysLogDA = new SysLogDA();
        SysParameterDA _sysParameterDA = new SysParameterDA();
        PageMenuDA _pageMenuDA = new PageMenuDA();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var _userDA = new UserDA();
                var result = _userDA.Login(model.UserName, Encryptor.MD5Hash(model.Password));
                if (result == 1)
                {
                    var userSession = new UserLogin();
                    var quyenViewHom = _sysParameterDA.GetItemByCode("ViewHome") != null ? _sysParameterDA.GetItemByCode("ViewHome").ParamValue : null;
                    var user = _userDA.GetItemByUserName(model.UserName);
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.ID;
                    userSession.Name = user.Name;
                    userSession.IsAdmin = user.IsAdmin;
                    userSession.Avartar = user.Avartar;
                    userSession.GroupId = user.GroupID;
                    var viewHom = false;
                    if (!string.IsNullOrEmpty(quyenViewHom))
                    {
                        var xau = quyenViewHom.Split(',');
                        if (xau != null && xau.Length > 0)
                        {
                            viewHom = xau.Contains(user.GroupID);
                        }
                    }
                    userSession.ViewHome = viewHom;
                    //userSession.EmployeeId = user.EmployeeId != null ? (int)user.EmployeeId : 0;
                    //if (!string.IsNullOrEmpty(userSession.Avartar))
                    //{
                    //    userSession.Avartar = ConvertPathImageToBase64(userSession.Avartar);
                    //}
                    var listPermission = _userDA.GetListCredentials(model.UserName);

                    var menus = _pageMenuDA.GetMenuByUser(user.ID);
                    if ((menus == null || menus.Count == 0) && user.IsAdmin)
                    {
                        menus = _pageMenuDA.GetMenuAdmin();
                    }
                    Session.Add("CEDENTIALS_SESSION", listPermission);
                    Session.Add("Menus", menus);
                    Session.Add("USER_SESSION", userSession);
                    AddLog("Đăng nhập( UserName: " + user.UserName + ") thành công.");
                    //if (userSession.ViewHome)
                    //    return RedirectToAction("Index", "Home");
                    //else
                    //{
                    //    var action = "";
                    //    var controller = "";
                    //    var url = menus.Where(x => !string.IsNullOrEmpty(x.HREF_URL)).FirstOrDefault().HREF_URL;
                    //    if (!string.IsNullOrEmpty(url))
                    //    {
                    //        var xauChar = url.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToList();
                    //        if (xauChar != null)
                    //        {
                    //            action = xauChar[1];
                    //            controller = xauChar[0];
                    //        }
                    //    }
                    //    return RedirectToAction(action, controller);
                    //}
                    return RedirectToAction("Index", "Home");
                }
                else if (result == 0)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Tài khoản không tồn tại.");
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else if (result == -1)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Tài khoản đang bị khóa.");
                    ModelState.AddModelError("", "Tài khoản đang bị khóa.");
                }
                else if (result == -2)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Mật khẩu không đúng.");
                    ModelState.AddModelError("", "Mật khẩu không đúng.");
                }
                else
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Đăng nhập không thành công.");
                    ModelState.AddModelError("", "Đăng nhập không thành công.");
                }

            }
            return View("Index");
        }

        public string ConvertPathImageToBase64(string path)
        {
            string base64String = "";
            var pathRoot = Server.MapPath("~/FileUpload");

            var arrayPath = path.IndexOf("FileUpload");
            string pathGet = path.Substring(0, arrayPath) + "FileUpload";
            string patttt = path.Replace(pathGet, pathRoot);


            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    using (Image image = Image.FromFile(path))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();

                            // Convert byte[] to Base64 String
                            base64String = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return base64String;
        }

        private void AddLog(string content)
        {
            _sysLogDA.Add(
                    new SysLog
                    {
                        ControllerName = "Login",
                        UserName = "",
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

        public ActionResult Logout()
        {
            var user = Session["USER_SESSION"] as UserLogin;
            AddLog("Đăng xuất( UserName: " + user.UserName + ") thành công.");
            Session["USER_SESSION"] = null;
            return Redirect("/Login/Login");
        }
    }
}