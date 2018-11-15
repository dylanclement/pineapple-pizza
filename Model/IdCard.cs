using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model
{
    public interface IIdCard
    {
        string Name { get; }
        int Number { get; }
        string PictureObjectId { get; }
    }

    public class IdCard
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string PictureObjectId { get; set; }
    }
}
