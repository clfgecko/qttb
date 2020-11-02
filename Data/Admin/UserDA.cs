using Common;
using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class UserDA
    {
        Entities db = new Entities();
        public int Login(string userName, string password)
        {
            var result = db.Users.FirstOrDefault(x => x.UserName == userName);
            if (result == null)
                return 0;
            else
            {
                if (!result.Status)
                    return -1;
                else
                {
                    if (result.Password == password)
                        return 1;
                    else
                        return -2;
                }
            }
        }
        public User GetItemByUserName(string userName)
        {
            return db.Users.FirstOrDefault(x => x.UserName == userName);
        }
        public List<string> GetListCredentials(string userName)
        {
            var query = from pm in db.PageMenus
                        join rp in db.RolePages on pm.ID equals rp.PageID
                        join u in db.Users on rp.RoleID equals u.GroupID
                        where u.UserName == userName && !string.IsNullOrEmpty(pm.CONTROLLER_NAME)
                        select new
                        {
                            ID = pm.CONTROLLER_NAME
                        };

            return query.Select(x => x.ID).ToList();
        }

        public ObjectMessage Add(User user)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                user.Password = Encryptor.MD5Hash("123456789a@");
                db.Users.Add(user);
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
        public ObjectMessage Edit(User user)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.Users.FirstOrDefault(x => x.ID == user.ID);
                data.UserName = user.UserName;
                data.Address = user.Address;
                data.Name = user.Name;
                data.Email = user.Email;
                data.Phone = user.Phone;
                data.Avartar = user.Avartar;
                data.Status = user.Status;
                //data.EmployeeId = user.EmployeeId;
                data.GroupID = user.GroupID;
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

        public ObjectMessage ChangePassword(long userId, string passwordOd, string passwordNew)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.Users.FirstOrDefault(x => x.ID == userId);
                var passwordOldb = data.Password;
                string passwordOd1 = Encryptor.MD5Hash(passwordOd);
                if (passwordOldb != passwordOd1)
                {
                    obj.Error = true;
                    obj.Title = "Bạn nhập mật khẩu cũ không đúng.";
                }
                else
                {
                    data.Password = Encryptor.MD5Hash(passwordNew);
                    db.SaveChanges();
                    obj.Error = false;
                    obj.Title = "Thêm mới thành công!";
                }

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
                var itemDelete = db.Users.Find(Id);
                db.Users.Remove(itemDelete);
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
