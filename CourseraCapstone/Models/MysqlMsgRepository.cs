using CourseraCapstone.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseraCapstone.Models
{
    public class MysqlMsgRepository : IMsgRepository
    {
        private readonly ApplicationDbContext context;

        public MysqlMsgRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public Msg Add(Msg msg)
        {
            context.Msgs.Add(msg);
            context.SaveChanges();
            return msg;
        }

        public IEnumerable<Msg> GetAllMyMessages(string id)
        {
            return context.Msgs.Include(x => x.Sender).Where(m => m.RecieverId == id).OrderByDescending(o => o.Id).ToList();
        }

        public Msg GetMsg(int id)
        {
            return context.Msgs.Include(x => x.Sender).FirstOrDefault(m => m.Id == id);
        }
    }
}
