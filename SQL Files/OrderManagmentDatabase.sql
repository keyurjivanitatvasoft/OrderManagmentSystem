CREATE TABLE Customer (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    EmailId VARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(10) NOT NULL,
    Address VARCHAR(255) NOT NULL
);
CREATE TABLE [Order] (
    orderId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NOT NULL,
    productName VARCHAR(100) NOT NULL,
    amount DECIMAL(10, 2),
    quantity INT ,
    FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId)
);
ALTER TABLE Customer
ADD Photo VARCHAR ;
ALTER TABLE [Order]
DROP COLUMN  CreatedDate ;
ALTER TABLE Customer
ALTER COLUMN customerId  VARCHAR(50) NOT NULL ;

 select * from Customer;
select * From [Order];

select * From [Order] join Customer on [Order].customer_id=Customer.customer_id where [Order].Isdelete=0;
update Customer set Isdelete=0;
update [Order] set Isdelete=0;
--UPDATE customer SET phonenumber = '8545785869' WHERE customer_id = 5;

insert into Customer (firstName,lastName,emailId,phoneNumber,address) values ('keyur','jivani','keyurpatel@gmail.com','8141351199','morbi,india') ;
insert into [Order](customer_id,productName,amount,quantity) values (1,'laptop',60000,1) ;

delete from customer where customer_id>7;

--for search data of customer
CREATE OR ALTER PROCEDURE SearchCustomers 
    @FirstName  VARCHAR(50) = NULL,
    @LastName   VARCHAR(50) = NULL,
    @PhoneNumber VARCHAR(10) = NULL,
    @EmailId    VARCHAR(100) = NULL,
	@Address    VARCHAR(255) = NULL,
    @CustomerId INT = 0
AS
BEGIN

    SELECT Customer.CustomerId as CustomerId,Customer.FirstName as CustomerFirstName,
	Customer.LastName as CustomerLastName,Customer.EmailId as EmailId,
	Customer.PhoneNumber as PhoneNumber,Customer.Address as Address,PhotoPath as Photo
    FROM customer
    WHERE
		(IsDelete=0) AND
        (@FirstName IS NULL OR FirstName = @FirstName) AND
        (@LastName IS NULL OR LastName = @LastName) AND
        (@PhoneNumber IS NULL OR PhoneNumber = PhoneNumber) AND
        (@EmailId IS NULL OR EmailId = @EmailId) AND
		(@Address IS NULL OR Address = @Address) AND
        (@CustomerId = 0  OR CustomerId = @CustomerId)
		 ORDER BY Customer.CustomerId
END;

EXEC search_customers NULL, NULL, NULL, NULL,NULL, 3;


--for adding and updateing data of customer
CREATE OR ALTER PROCEDURE SaveCustomers 
    @FirstName  VARCHAR(50) ,
    @LastName   VARCHAR(50) ,
    @PhoneNumber VARCHAR(10) ,
    @EmailId    VARCHAR(100) ,
	@Address    VARCHAR(255)=null ,
	@IsDelete	BIT,
	@Photo TEXT =null,
    @CustomerId INT = 0
AS
BEGIN
   IF @CustomerId = 0
		BEGIN
			-- Insert new customer
			INSERT INTO Customer(FirstName,LastName,PhoneNumber,EmailId,Address,PhotoPath)
			VALUES (@FirstName, @LastName, @PhoneNumber, @EmailId, @Address,@Photo);
		END
    ELSE
		BEGIN
			-- Update existing customer
			UPDATE Customer
			SET 
				FirstName = @FirstName,
				LastName= @LastName,
				PhoneNumber = @PhoneNumber,
				EmailId = @EmailId,
				Address = @Address,
				PhotoPath=@Photo,
				IsDelete=@IsDelete
			WHERE CustomerId = @CustomerId;
		END
END;

CREATE TYPE IntListType AS TABLE (
    Value INT
);

CREATE OR ALTER PROCEDURE DeleteCustomers
    @CustomerIds IntListType READONLY 
AS
BEGIN
    Update Customer
	SET
		IsDelete=1
    WHERE CustomerId IN (SELECT Value FROM @CustomerIds);
END;

CREATE OR ALTER PROCEDURE CustomersExits
    @CustomerIds IntListType READONLY 
AS
BEGIN
    Select COUNT(*) 
	FROM Customer
    WHERE 
	IsDelete=0 AND 
	CustomerId IN (SELECT Value FROM @CustomerIds);
END;





EXEC search_order NULL,0,0, 0,0;
--for search data of order
CREATE OR ALTER PROCEDURE SearchOrder 
    @ProductName  VARCHAR(50) = NULL,
    @Amount   DECIMAL(10,2) = 0,
    @Quantity INT = 0,
	@CustomerId    INT = 0,
    @OrderId INT = 0
AS
BEGIN
	

    SELECT o.orderId as OrderId,o.ProductName as ProductName,o.Quantity as Quantity,o.Amount as Amount,o.CustomerId as CustomerId,c.FirstName as CustomerFirstName,c.LastName as CustomerLastName
    FROM [Order] as o
    INNER JOIN Customer as c 
	ON o.CustomerId = c.CustomerId
    WHERE
		(o.IsDelete=0) AND
        (@ProductName IS NULL OR o.ProductName = @ProductName) AND
        (@Amount=0 OR o.Amount = @Amount) AND
        (@Quantity=0 OR o.Quantity = @Quantity) AND
        (@CustomerId=0 OR o.CustomerId = @CustomerId) AND
        (@OrderId = 0 OR o.OrderId = @OrderId);
END;




--for adding and updateing data of Order
CREATE OR ALTER PROCEDURE SaveOrder 
    @ProductName  VARCHAR(50) ,
    @Amount   DECIMAL(10,2) ,
    @Quantity INT ,
	@CustomerId INT,
	@IsDelete BIT,
    @OrderId INT = 0
AS
BEGIN
   IF @OrderId = 0
		BEGIN
			-- Insert new Order
			INSERT INTO [Order](CustomerId,ProductName,Amount,Quantity)
			VALUES (@CustomerId,@ProductName,@Amount,@Quantity)
		END
    ELSE
		BEGIN
			-- Update existing Order
			UPDATE [Order] 
			SET
				CustomerId = @CustomerId,
				ProductName = @ProductName,
				Amount = @Amount,
				Quantity = @Quantity,
				IsDelete=@IsDelete
			WHERE OrderId = @OrderId;
		END
END;



CREATE OR ALTER PROCEDURE DeleteOrders
    @OrderIds IntListType READONLY 
AS
BEGIN
    Update [Order]
	SET
		IsDelete=1
    WHERE OrderId IN (SELECT Value FROM @OrderIds);
END;

CREATE OR ALTER PROCEDURE OrdersExits
    @OrderIds IntListType READONLY 
AS
BEGIN
    Select Count(*) 
	FROM [Order]
    WHERE 
	IsDelete=0 AND 
	OrderId IN (SELECT Value FROM @OrderIds);
END;



Select * From Customer where customer_id!=5 AND (emailId='parth@gamil.com' OR phoneNumber='8545785869' )

--for Customer orderdetails
CREATE OR ALTER PROCEDURE CustomerOrderDetails 
AS
BEGIN
	SELECT c.CustomerId as CustomerId, c.FirstName AS FirstName, c.LastName AS LastName, SUM(o.amount) AS TotalAmount
	FROM Customer c
	LEFT JOIN [Order] o ON c.CustomerId = o.CustomerId
	WHERE c.IsDelete=0
	GROUP BY c.CustomerId, c.FirstName, c.LastName

END;


CREATE TABLE ProductType(
	productTypeId INT PRIMARY KEY IDENTITY(1,1),
	productTypeName VARCHAR(50) NOT NULL,
	IsDelete BIT DEFAULT 0,
);
update ProductType set IsDelete=0;
select * from ProductType;
INSERT INTO ProductType (productTypeName) VALUES ('Type 1');
INSERT INTO ProductType (productTypeName) VALUES ('Type 2');
INSERT INTO ProductType (productTypeName) VALUES ('Type 3');
INSERT INTO ProductType (productTypeName) VALUES ('Type 4');
INSERT INTO ProductType (productTypeName) VALUES ('Type 5');
CREATE OR ALTER PROCEDURE ProductTypes 
    @productTypeName  VARCHAR(50)=NULL ,
    @productTypeId INT = 0
AS
BEGIN
	SELECT p.productTypeId as ProductTypeId,p.productTypeName as ProductTypeName
	FROM ProductType as p
    WHERE
		(p.Isdelete=0) AND
		(@productTypeId =0 OR p.productTypeId=@productTypeId) AND
        (@productTypeName IS NULL OR p.productTypeName= @productTypeName);
END;
CREATE OR ALTER PROCEDURE SaveProductType 
    @productTypeName  VARCHAR(50) ,
	@IsDelete BIT ,
    @productTypeId INT = 0
AS
BEGIN
   IF @productTypeId = 0
		BEGIN
			-- Insert new Order
			INSERT INTO ProductType (productTypeName)
			VALUES (@productTypeName)
		END
    ELSE
		BEGIN
			-- Update existing Order
			UPDATE ProductType 
			SET
				productTypeName=@productTypeName,
				Isdelete=@Isdelete
			WHERE productTypeId = @productTypeId;
		END
END;
CREATE TABLE Product (
    productId INT PRIMARY KEY IDENTITY(1,1),
	productTypeId INT NOT NULL,
    productName VARCHAR(50) NOT NULL,
    amount DECIMAL NOT NULL,
    quantity INT NOT NULL,
    FOREIGN KEY (productTypeId) REFERENCES ProductType(productTypeId)
);