using Microsoft.AspNetCore.Mvc;
using OrderManagmentSytemBAL.ProductRepositry;
using OrderManagmentSytemDAL.ViewModels;

namespace OrderManagmentSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepositry productRepositry;
        public ProductController(IProductRepositry productRepositry)
        {
            this.productRepositry = productRepositry;
        }
        #region ProductType
        [HttpGet]
        [Route("/producttype")]
        public IActionResult ProductTypeList()
        {
            Response productTypeDetails = productRepositry.ProductTypes(new ProductType());
            return View(productTypeDetails);
        }
        [HttpGet]
        [Route("/producttype/edit")]
        public IActionResult EditProductType(int productTypeId)
        {
            ProductType productType = new ProductType
            {
                ProductTypeId = productTypeId,
            };
            Response productTypeResponse = productRepositry.ProductTypes(productType);
            if (productTypeResponse.IsSuccess && productTypeResponse.Result is IEnumerable<ProductType> producttype && producttype.Count() == 1)
            {
                return View("ProductTypeForm", producttype.First());
            }
            return RedirectToAction("ProductTypeList");
        }
        [HttpGet]
        [Route("/producttype/deleteconfirmation")]
        public IActionResult DeleteConfirmationProductType(int productTypeId)
        {
            ProductType productType = new ProductType
            {
                ProductTypeId = productTypeId,
            };
            Response searchCustomer = productRepositry.ProductTypes(productType);
            if (searchCustomer.IsSuccess && searchCustomer.Result is IEnumerable<ProductType> producttype && producttype.Count() == 1)
            {
                return PartialView("ConfirmationBox", productTypeId);
            }
            return Json(new { message = "Product Type Not Exits" });
        }

        [HttpGet]
        [Route("/producttype/delete")]
        public IActionResult DeleteProductType(int productTypeId)
        {
            ProductType productType = new ProductType
            {
                ProductTypeId = productTypeId,
            };
            Response searchCustomer = productRepositry.ProductTypes(productType);
            if (searchCustomer.IsSuccess && searchCustomer.Result is IEnumerable<ProductType> producttypes && producttypes.Count() == 1)
            {
                Response customerDetails = productRepositry.SaveProductType(producttypes.First(), true);
                if (customerDetails.IsSuccess)
                {
                    return RedirectToAction("ProductTypeList");
                }
            }
            return RedirectToAction("Privacy", "Home");
        }
        [HttpGet]
        [Route("/producttype/Addproducttype")]
        public IActionResult CreateProductType()
        {
            ProductType productType = new ProductType();
            return View("ProductTypeForm", productType);

        }
        [HttpPost]
        [Route("/producttype/Addproducttype")]
        public IActionResult CreateProductType(ProductType productType)
        {
            if (!ModelState.IsValid)
            {
                return View("ProductTypeForm", productType);
            }
            Response productTypeCheck = productRepositry.ProductTypes(new ProductType
            {
                ProductTypeName= productType.ProductTypeName,
            });
            if (productTypeCheck.IsSuccess && productTypeCheck.Result is  IEnumerable<ProductType> result && result.Count(p =>!p.ProductTypeId.Equals(productType.ProductTypeId)) > 0)
            {
                ModelState.AddModelError("ProductTypeId", "Product Type Name already Exits.");
                return View("ProductTypeForm", productType);
            }
            Response response = productRepositry.SaveProductType(productType, false);
            if (response.IsSuccess)
            {
                return RedirectToAction("ProductTypeList");
            }
            return View("ProductTypeForm", productType);
        }

        #endregion
    }
}
