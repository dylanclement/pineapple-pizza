using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model.DynamoDB
{
    public class IdCardDynamo : IDynamoObject<IdCard>
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public string PictureObjectId { get; set; }

        public void FromPoco(IdCard poco)
        {
            Name = poco.Name;
            Number = poco.Number;
            PictureObjectId = poco.PictureObjectId;
        }

        public IdCard ToPoco()
        {
            return new IdCard
            {
                Name = Name,
                Number = Number,
                PictureObjectId = PictureObjectId
            };
        }
    }
}
