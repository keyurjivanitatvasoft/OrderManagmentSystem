using Dapper;
using Microsoft.Extensions.Options;
using OrderManagmentSytemDAL.DbContext;
using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemBAL.ProductRepositry
{
    public class ProductRepositry:IProductRepositry
    {
        private readonly OrderManagmentSystemDapperContext context;
        public ProductRepositry(IOptions<ConnectionStrings> connectionString)
        {
            context = new OrderManagmentSystemDapperContext(connectionString.Value.OrderManagmentSystem);
        }
        #region ProductType
        public Response ProductTypes(ProductType productType)
        {
            Response response = new Response();
            try
            {
                var parameters = new Dapper.DynamicParameters();
                parameters.Add("@productTypeName", productType.ProductTypeName);
                parameters.Add("@@productTypeId", productType.ProductTypeId);
                IEnumerable<ProductType> productTypes = context.Query<ProductType>("ProductTypes", parameters, true);

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = productTypes;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }
        public Response SaveProductType(ProductType productType, bool Isdelete)
        {
            Response response = new Response();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@productTypeName", productType.ProductTypeName);
                parameters.Add("@productTypeId", productType.ProductTypeId);
                parameters.Add("@Isdelete", Isdelete);

                int rowaffected = context.Execute("SaveProductType", parameters, true);
                if (rowaffected > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return response;
        }
        #endregion
    }
}
