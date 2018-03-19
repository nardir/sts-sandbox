using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }

        public ICollection<ProductPicture2> ProductPictures { get; set; }
    }

    public class Picture
    {
        public int PictureID { get; set; }
        public string Uri { get; set; }
    }

    public class EntityPicture
    {
        public int EntityID { get; set; }
        public int EntityInstanceID { get; set; }
        public int PictureID { get; set; }
        public int Rank { get; set; }
    }

    public class ProductPicture: EntityPicture
    {
        public int ProductID { get => EntityInstanceID; }
    }

    public class CustomerPicture: EntityPicture
    {
    }

    public class ProductPicture2
    {
        public int ProductID { get; set; }
        public int PictureID { get; set; }
        public int Rank { get; set; }

        public Product Product { get; set; }
        public Picture Picture { get; set; }
    }
}