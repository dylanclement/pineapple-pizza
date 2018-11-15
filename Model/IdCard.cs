using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model
{
    public class IdCard
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public Guid PictureObjectId { get; set; }
    }
}
