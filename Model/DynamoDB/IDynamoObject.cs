using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model.DynamoDB
{
    interface IDynamoObject<T>
    {
        T ToPoco();
        void FromPoco(T poco);
    }
}
