using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemBAL.ProductRepositry
{
    public interface IProductRepositry
    {
        #region ProductType
        public Response ProductTypes(ProductType productType);
        public Response SaveProductType(ProductType productType,bool Isdelete);
        #endregion
    }
}
