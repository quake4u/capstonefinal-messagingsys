using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseraCapstone.Models
{
    public interface IMsgRepository
    {
        Msg GetMsg(int id);
        Msg Add(Msg msg);
        IEnumerable<Msg> GetAllMyMessages(string id);
    }
}
