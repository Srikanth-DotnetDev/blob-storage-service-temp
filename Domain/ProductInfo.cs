using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal.BlobStorage.Domain
{
    public class ProductInfo
    {
        public ProductInfo(string id, string productType)
        {
            this.id = id ?? throw new ArgumentNullException(nameof(id));
            this.productType = productType ?? throw new ArgumentNullException(nameof(productType));
        }

        public string id { get; set; }    
        public string productType { get; set; } 
    }
}
