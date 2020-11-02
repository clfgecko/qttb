using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class SysLogDA
    {
        Entities db = new Entities();

        public ObjectMessage Add(SysLog sysLog)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.SysLogs.Add(sysLog);
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
        public ObjectMessage Edit(SysLog sysLog)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.SysLogs.FirstOrDefault(x => x.ID == sysLog.ID);
                data.ControllerName = sysLog.ControllerName;
                data.UserName = sysLog.UserName;
                data.DateLog = sysLog.DateLog;
                data.Content = sysLog.Content;

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

    }
}
