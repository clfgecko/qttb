using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Admin
{
    public class PageMenuDA
    {
        Entities db = new Entities();

        public PageMenu GetItemByName(string name)
        {
            return db.PageMenus.FirstOrDefault(x => x.NAME == name);
        }

        public List<PageMenu> GetAllByPage(int page)
        {
            var pageSize = 10;
            var param = db.SysParameters.FirstOrDefault(x => x.ParamCode == "PageSize");
            if (param != null)
                pageSize = Convert.ToInt32(param.ParamValue);
            return db.PageMenus.Where(x => x.IS_ACTIVE).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
        }

        public List<PageMenu> GetAll()
        {
            return db.PageMenus.Where(x => x.IS_ACTIVE).ToList();
        }

        public List<MenuModel> GetMenuAdmin()
        {
            var result = new List<MenuModel>();
            try
            {
                var pageMenus = db.PageMenus.Where(x => x.IS_ACTIVE && x.IS_SYSTEM_ROLE).ToList();
                if(pageMenus != null && pageMenus.Count > 0)
                {
                    for (int i = 0; i < pageMenus.Count; i++)
                    {
                        var model = new MenuModel
                        {
                            ID = pageMenus[i].ID,
                            NAME = pageMenus[i].NAME,
                            DESCRIPTION = pageMenus[i].DESCRIPTION,
                            IS_ACTIVE = pageMenus[i].IS_ACTIVE,
                            ORDER_BY = pageMenus[i].ORDER_BY,
                            CONTROLLER_NAME = pageMenus[i].CONTROLLER_NAME,
                            HREF_URL = pageMenus[i].HREF_URL,
                            PARENT_PAGE_ID = pageMenus[i].PARENT_PAGE_ID,
                            IS_SYSTEM_ROLE = pageMenus[i].IS_SYSTEM_ROLE,
                            Actions = GetAllByPageMenu(pageMenus[i].ID)
                        };
                        result.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                db.SysLogs.Add(
                   new SysLog
                   {
                       ControllerName = "PageMenuDA",
                       UserName = "",
                       DateLog = DateTime.Now,
                       Content = "Lấy danh sách page menu lỗi: " + ex.Message
                   }
                   );
                db.SaveChanges();
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách action trên 1 page menu
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public string GetAllByPageMenu(int page)
        {
            var result = "";
            try
            {
                var actions = db.PageActions.Where(x => x.PAGE_ID == page).Select(x => x.CONTROL_NAME).ToArray();
                if (actions != null && actions.Length > 0)
                {
                    result = string.Join("|", actions);
                }
            }
            catch (Exception ex)
            {
                db.SysLogs.Add(
                   new SysLog
                   {
                       ControllerName = "PageMenuDA",
                       UserName = "",
                       DateLog = DateTime.Now,
                       Content = "Lấy danh sách action của page menu lỗi: " + ex.Message
                   }
                   );
                db.SaveChanges();
                result = "";
            }
            return result;
        }
        /// <summary>
        /// Lấy danh sách menu theo user ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<MenuModel> GetMenuByUser(long userID)
        {
            var result = new List<MenuModel>();
            try
            {
                // lấy thông tin user
                var user = db.Users.FirstOrDefault(x => x.ID == userID);

                // Lấy danh sách menu
                result = (from rp in db.RolePages
                          join pm in db.PageMenus on rp.PageID equals pm.ID
                          where rp.RoleID == user.GroupID && pm.IS_ACTIVE == true && rp.IS_ACTIVE == true
                          select new MenuModel
                          {
                              ID = pm.ID,
                              NAME = pm.NAME,
                              DESCRIPTION = pm.DESCRIPTION,
                              IS_ACTIVE = pm.IS_ACTIVE,
                              ORDER_BY = pm.ORDER_BY,
                              CONTROLLER_NAME = pm.CONTROLLER_NAME,
                              HREF_URL = pm.HREF_URL,
                              PARENT_PAGE_ID = pm.PARENT_PAGE_ID,
                              IS_SYSTEM_ROLE = pm.IS_SYSTEM_ROLE,
                              Actions = rp.CONTROL_STRING
                          }).ToList();

            }
            catch (Exception ex)
            {
                db.SysLogs.Add(
                    new SysLog
                    {
                        ControllerName = "PageMenuDA",
                        UserName = "",
                        DateLog = DateTime.Now,
                        Content = "Lấy danh sách menu lỗi: " + ex.Message
                    }
                    );
                db.SaveChanges();
                result = new List<MenuModel>();
            }
            return result;
        }

        public ObjectMessage Add(PageMenu pageMenu)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.PageMenus.Add(pageMenu);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(PageMenu pageMenu)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.PageMenus.FirstOrDefault(x => x.ID == pageMenu.ID);
                data.NAME = pageMenu.NAME;
                data.DESCRIPTION = pageMenu.DESCRIPTION;
                data.ORDER_BY = pageMenu.ORDER_BY;
                data.CONTROLLER_NAME = pageMenu.CONTROLLER_NAME;
                data.HREF_URL = pageMenu.HREF_URL;
                data.PARENT_PAGE_ID = pageMenu.PARENT_PAGE_ID;
                data.IS_SYSTEM_ROLE = pageMenu.IS_SYSTEM_ROLE;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Delete(int Id)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var itemDelete = db.PageMenus.Find(Id);
                db.PageMenus.Remove(itemDelete);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
