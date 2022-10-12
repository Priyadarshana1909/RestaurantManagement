USE [DBRestaurant]
GO
/****** Object:  UserDefinedFunction [dbo].[UDF_GetItemPrice]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------This Function holds type the Select query of RestaurantMenuItem where it returns the ItemPrice of respective MenuItemID
--SELECT  [dbo].[UDF_GetItemPrice](0) -- Invalid Menu Item Id - will return 0
--SELECT  [dbo].[UDF_GetItemPrice](1) -- Valid Menu Item Id - will return Item price

CREATE OR ALTER FUNCTION [dbo].[UDF_GetItemPrice] (
    @MenuItemID INT
)
RETURNS FLOAT AS
BEGIN
    Declare @ItemPrice FLOAT = 0;
	SELECT @ItemPrice = ISNULL(ItemPrice,0) FROM  RestaurantMenuItem WHERE MenuItemId = @MenuItemID
	
    RETURN @ItemPrice
END
GO
/****** Object:  UserDefinedFunction [dbo].[UDP_GetOrderDetails]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------THIS TABLE FUNCTION HOLDS THE TYPE OF SELECT QUERY WHERE IT RETURNS THE TABLE RECORD OF ORDER OF RESPECTIVE ORDERID

CREATE OR ALTER FUNCTION [dbo].[UDP_GetOrderDetails]
(
  @OrderID INT
)
RETURNS TABLE
AS
RETURN
(
SELECT OrderDate,
	   RestaurantID,
       MenuItemID,
       ItemQuantity,
       OrderAmount,
       DiningTableID
       FROM dbo.[Order]
       WHERE OrderID = @OrderID);
GO
/****** Object:  View [dbo].[VCuisineList]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER  VIEW [dbo].[VCuisineList]
As
	SELECT		C.CuisineID,C.CuisineName,I.ItemName,I.ItemPrice 
	FROM		Cuisine C
	INNER JOIN
				RestaurantMenuItem I
	ON 
				C.CuisineID = I.CuisineID
	Order By	C.CuisineID
	OFFSET 0 ROWS;
GO
/****** Object:  StoredProcedure [dbo].[USP_Billis]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


------(7) This sp holds type the Insert, Update, Delete of table BILLS
--SELECT * FROM Bills
--Select * from [Order]
--Exec [dbo].[USP_Bills]  @BillsID = 0,@RestaurantID = 4, @OrderID = 43, @BillAmount = 10, @CustomerID =14,@IsDelete = 0, @OutputID = 0
CREATE OR ALTER  PROCEDURE [dbo].[USP_Bills] (
								@BillsID INT,
                                @OrderID INT,
								@RestaurantID INT,
								@BillAmount FLOAT,
								@CustomerID INT,
                               	@IsDelete BIT,
								@OutputID INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION	
	        --to raise exception un comment below 2 lines

			 --   Declare @temp1 Int  = 0
		   --Set @temp1 = 1 / 0;

			
			IF(@IsDelete = 1 AND ((@BillsID <= 0) OR (NOT EXISTS (SELECT 1 FROM Bills WHERE BillsID = @BillsID))))
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid Bills Id',11,1); 
			END
			IF(@IsDelete = 0 AND ((@OrderID <= 0) OR (NOT EXISTS (SELECT 1 FROM [Order] WHERE OrderID  = @OrderID))))
			BEGIN
				-- in case of add / edit
				RAISERROR ('Invalid Order Id',11,1); 
			END

			DECLARE @DiningTableID INT
			IF (@IsDelete = 0)
			BEGIN
			    SELECT @BillAmount = OrderAmount,
				@RestaurantID = RestaurantID,
				@DiningTableID = DiningTableID
				FROM [dbo].[UDP_GetOrderDetails](@OrderID);

			END
			IF (@IsDelete = 0 AND @BillsID = 0)
			 BEGIN
			   -- Add Operation
				INSERT INTO Bills (OrderID,RestaurantID, BillAmount, CustomerID)
					VALUES ( @OrderID,@RestaurantID, @BillAmount, @CustomerID);
				
				Update DiningTableTrack Set TableStatus = 'Occupied'
				 WHERE DiningTableID = @DiningTableID;

				 SELECT @OutputID = @@ROWCOUNT;
					
			END
			ELSE IF (@IsDelete = 0 AND @BillsID > 0)
			BEGIN
			-- Update Operation
					UPDATE Bills
						SET OrderID = @OrderID,
							RestaurantID = @RestaurantID,
							BillAmount = @BillAmount,
							CustomerID = @CustomerID
						WHERE BillsID  = @BillsID;

						 SELECT @OutputID = @@ROWCOUNT;
			END
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM Bills WHERE BillsID  = @BillsID;

				 SELECT @OutputID = @@ROWCOUNT;
			END
	IF @@TRANCOUNT > 0
	BEGIN
	  COMMIT TRANSACTION
	END
END TRY
BEGIN CATCH
   --Print 'in catch'
		-- return the error inside the CATCH block
		-- Transaction uncommittable
		IF @@TRANCOUNT > 0
		BEGIN
		-- Print 'Roll back tranaction'
		  ROLLBACK TRANSACTION
        END
		DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USP_Cuisine]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Select * from Cuisine
--Insert Cuisine
--Exec [USP_Cuisine]  @CuisineID = 0, @RestaurantID = 4, @CuisineName = 'East Mahi', @IsDelete = 0, @OutputCuisineId = 0
--Update Cuisine
--Exec [USP_Cuisine]  @CuisineID = 21, @RestaurantID = 4, @CuisineName = 'West Mahi', @IsDelete = 0, @OutputCuisineId = 0
--Delete Cuisine Transaction which is used in RestaurantMenuItem will Rollback
--Exec [USP_Cuisine]  @CuisineID = 18, @RestaurantID = 4, @CuisineName = 'North Mahi', @IsDelete = 1, @OutputCuisineId = 0

CREATE OR ALTER PROCEDURE [dbo].[USP_Cuisine] (
								@CuisineID INT,
                                @RestaurantID INT,
                                @CuisineName NVARCHAR(50),								
								@IsDelete BIT,
								@OutputCuisineId INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION

	        IF(@IsDelete = 1 AND ((@CuisineID <= 0) OR (NOT EXISTS (SELECT 1 FROM Cuisine WHERE CuisineID = @CuisineID))))
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid Cuisine id',11,1); 
			END
			ELSE IF (@IsDelete = 0 AND ISNULL(@CuisineName,'') = '')
			BEGIN
				-- in case of add / delete				
			   RAISERROR ('Cuisine Name should not be null.',11,1,@CuisineName); 
			END
			IF (@IsDelete = 0 AND @CuisineID = 0)
			 BEGIN
			   -- Add Operation
				INSERT INTO Cuisine
								(RestaurantID,
									CuisineName)
					VALUES     ( @RestaurantID,
									@CuisineName)

			SELECT @OutputCuisineId = @@ROWCOUNT
			END
			ELSE IF (@IsDelete = 0 AND @CuisineID > 0)
			BEGIN
			-- Update Operation
					UPDATE Cuisine
						SET RestaurantID = @RestaurantID,
							CuisineName = @CuisineName            
						WHERE CuisineID  = @CuisineID;

						SELECT @OutputCuisineId = @@ROWCOUNT
			END
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM Cuisine WHERE CuisineID  = @CuisineID;

				SELECT @OutputCuisineId = @@ROWCOUNT
			END
			
	IF @@TRANCOUNT > 0
	BEGIN
	  COMMIT TRANSACTION
	END
END TRY
BEGIN CATCH
	
		
		-- Transaction uncommittable
		IF @@TRANCOUNT > 0
		BEGIN
		-- Print 'Roll back tranaction'
		  ROLLBACK TRANSACTION
        END
 
	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USP_Customer]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
------(6) This sp holds type the Insert, Update, Delete of table Customer 

--Select * from Customer
--Insert 
--Exec [dbo].[USP_Customer] @CustomerID = 0,@RestaurantID = 6, @CustomerName = 'Saurav Solanki', @MobileNo = '7865665878', @IsDelete = 0, @OutputID = 0
--Update
--Exec [dbo].[USP_Customer] @CustomerID = 14,@RestaurantID = 4, @CustomerName = 'Saurav Solanki', @MobileNo = '7865665800', @IsDelete = 0, @OutputID = 0
--Delete
--Exec [dbo].[USP_Customer] @CustomerID = 15,@RestaurantID = 4, @CustomerName = 'Nehal Solanki', @MobileNo = '7865665889', @IsDelete = 1, @OutputID = 0

CREATE OR ALTER  PROCEDURE [dbo].[USP_Customer] (
								@CustomerID INT,
                                @RestaurantID INT,
                                @CustomerName NVARCHAR(100),	
								@MobileNo NVARCHAR(10),	
								@IsDelete BIT,								
								@OutputID INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION
	       
		   --    Declare @temp1 Int  = 0
		   --Set @temp1 = 1 / 0;

		    IF(@IsDelete = 1 AND ((@CustomerID <= 0) OR (NOT EXISTS (SELECT 1 FROM Customer WHERE CustomerID = @CustomerID))))
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid CustomerID id',11,1); 
			END;
			ELSE IF (@IsDelete = 0)
			BEGIN
				-- in case of add / delete				
				IF(Len(ISNULL(@CustomerName,'')) < 10)
				BEGIN
				   RAISERROR ('CustomerName should have atleast 10 characters.',11,1,@CustomerName); 
				END

				If((@CustomerName LIKE '%[!@#$%^*()_-+=:?<>]%') OR (@CustomerName LIKE '%[0-9]%'))
				BEGIN
				   RAISERROR ('No special character, no integer is used for CustomerName..',11,1,@CustomerName); 
				END

				IF (LEN(ISNULL(@MobileNo,'')) <> 10)
				BEGIN
					RAISERROR ('Mobile number should be exactly 10 digits.',11,1,@MobileNo); 
				END

			END
			IF (@IsDelete = 0 AND @CustomerID = 0)
			 BEGIN
			   -- Add Operation
				INSERT INTO Customer
								(RestaurantID,
									CustomerName,
									MobileNo)
					VALUES     ( @RestaurantID,
									@CustomerName,
									@MobileNo);

			   SELECT @OutputID = @@ROWCOUNT
			END
			ELSE IF (@IsDelete = 0 AND @CustomerID > 0)
			BEGIN
			-- Update Operation
					UPDATE Customer
						SET RestaurantID = @RestaurantID,
							CustomerName = @CustomerName,
							MobileNo = @MobileNo
						WHERE CustomerID  = @CustomerID
						AND CustomerID NOT IN (Select CustomerID from Bills);
					SELECT @OutputId = @@ROWCOUNT
			END
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM Customer WHERE CustomerID  = @CustomerID
									 AND CustomerID NOT IN (Select CustomerID from Bills);
				SELECT @OutputId = @@ROWCOUNT
			END
			IF(@OutputID = 0)
			BEGIN
				RAISERROR('Data cannot be allowed to be updated / deleted AS Customer is already used in billing.',11,1);
			END;
			
	 IF @@TRANCOUNT > 0
	BEGIN
	  COMMIT TRANSACTION
	END
END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0
		BEGIN
		 --Print 'Roll back tranaction'
		  ROLLBACK TRANSACTION
        END

	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

END CATCH;

END
GO
/****** Object:  StoredProcedure [dbo].[USP_DiningTable]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------(4) This sp holds type the Insert, Update, Delete of table DiningTable 


--select * from DiningTable
--Select * from DiningTableTrack
--Select * from Restaurant
--update DiningTable set DiningTableID= 3 where CuisineID=12
--Insert into DiningTable Values (2,'Kadi')

--Insert Transaction
--Exec [dbo].[USP_DiningTable] @DiningTableID = 1,@RestaurantID = 0, @Location = '',  @IsDelete = 0, @OutputDiningTableID = 0
--Exec [dbo].[USP_DiningTable] @DiningTableID = 0,@RestaurantID = 4, @Location = 'Shahibaug',  @IsDelete = 0, @OutputDiningTableID = 0
--Delete Transaction
--Exec [dbo].[USP_DiningTable] @DiningTableID = 13,@RestaurantID = 4, @Location = 'Shahibaug',  @IsDelete = 1, @OutputDiningTableID = 0

ALTER   PROCEDURE [dbo].[USP_DiningTable] (
								@DiningTableID INT,
                                @RestaurantID INT,
                                @Location NVARCHAR(100),									
								@IsDelete BIT,
								@OutputDiningTableID INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION

	    -- Declare @temp1 Int  = 0
		   --Set @temp1 = 1 / 0;

			IF(@IsDelete = 1 AND ((@DiningTableID <= 0) OR (NOT EXISTS (SELECT 1 FROM DiningTable WHERE DiningTableID = @DiningTableID))))
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid DiningTable id',11,1); 
			END

			IF (@IsDelete = 0 AND @DiningTableID = 0)
			 BEGIN
			   -- Add Operation
				INSERT INTO DiningTable
								(RestaurantID,
									[Location])
					VALUES     ( @RestaurantID,
									@Location);
				Declare @NewDiningTableID INT= @@Identity
				INSERT INTO DiningTableTrack (DiningTableID, TableStatus)
				VALUES (@NewDiningTableID, 'Vacant');

				SELECT @OutputDiningTableID =  @@ROWCOUNT

			END
			ELSE IF (@IsDelete = 0 AND @DiningTableID > 0)
			BEGIN
			-- Update Operation
					UPDATE DiningTable
						SET RestaurantID = @RestaurantID,
							[Location] = @Location
						WHERE DiningTableID  = @DiningTableID;

						SELECT @OutputDiningTableID =  @@ROWCOUNT
			END
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM DiningTableTrack WHERE DiningTableID = @DiningTableID;
				DELETE FROM DiningTable WHERE DiningTableID  = @DiningTableID;
				
				SELECT @OutputDiningTableID =  @@ROWCOUNT
			END
	
			
	IF @@TRANCOUNT > 0
	BEGIN
	  COMMIT TRANSACTION
	END
END TRY
BEGIN CATCH

     IF @@TRANCOUNT > 0
		BEGIN
		 --Print 'Roll back tranaction'
		  ROLLBACK TRANSACTION
        END

	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

END CATCH;

END
/****** Object:  StoredProcedure [dbo].[USP_GetAmountByDayAndTable]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--4) Create procedure to list daywise, tablewise total order amount.

--Exec USP_GetAmountByDayAndTable 'Table'

CREATE OR ALTER PROCEDURE  [dbo].[USP_GetAmountByDayAndTable] (
											 @ListBy nVarchar(10))
AS
BEGIN
	IF(@ListBy = 'Table')
	BEGIN
			SELECT		CAST(OrderDate AS DATE) AS OrdersDate,YEAR(CAST(OrderDate AS DATE)) [year],
						Sum(OrderAmount) as TotalAmount 
			FROM		[Order]
			GROUP BY	CAST(OrderDate AS DATE)
	END
	IF(@ListBy = 'OrderDate' )
	BEGIN
			SELECT		CAST(OrderDate AS DATE) AS OrdersDate,DAY(CAST(OrderDate AS DATE)) [DAY],
						Sum(OrderAmount) as TotalAmount 
			FROM		[Order]
			GROUP BY	CAST(OrderDate AS DATE)
			ORDER BY	[DAY]
	END
    
END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetBill]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER  PROCEDURE [dbo].[USP_GetBill] (
                                @BillID INT = NULL
                              )								
AS
BEGIN
   SELECT Bills.BillsID, Bills.OrderID, Bills.RestaurantID,
   Bills.BillAmount, Bills.CustomerID, Customer.CustomerName, 
   Restaurant.RestaurantName
	FROM [Bills] 
	INNER JOIN Restaurant on [Bills].RestaurantID = Restaurant.RestaurantID
	INNER JOIN Customer on [Customer].CustomerID = [Bills].CustomerID
	WHERE @BillID IS NULL OR [Bills].BillsID = @BillID

END
GO

/****** Object:  StoredProcedure [dbo].[USP_GetCuisine]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[USP_GetCuisine] (
                                @CuisineID INT = NULL
                              )								
AS
BEGIN
   SELECT C.CuisineID,
	C.RestaurantID,
	C.CuisineName,
	R.RestaurantName
	FROM Cuisine as C
	INNER JOIN Restaurant R
			ON R.RestaurantID = C.RestaurantID
				WHERE @CuisineID IS NULL OR C.CuisineID = @CuisineID

END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetCustomer]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER  PROCEDURE [dbo].[USP_GetCustomer] (
                                @CustomerID INT = NULL
                              )								
AS
BEGIN
   SELECT C.CustomerID,
   R.RestaurantID,
	C.CustomerName,
	C.MobileNo,
	R.RestaurantName
	FROM Customer as C
	INNER JOIN Restaurant R
			ON R.RestaurantID = C.RestaurantID
				WHERE @CustomerID IS NULL OR C.CustomerID = @CustomerID

END
GO

/****** Object:  StoredProcedure [dbo].[USP_GetCustomerDynamicallyNew]    Script Date: 08/10/2022 23:16:33 ******/
USE [DBRestaurant]
GO
/****** Object:  StoredProcedure [dbo].[USP_GetCustomerDynamically]    Script Date: 12/10/2022 13:15:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Select * from Customer
--Select * from [Order]
--Select * from Bills
--EXEC USP_GetCustomerDynamically  @SortBy = 'Location', @SortDirection = 'DESC'
--EXEC USP_GetCustomerDynamically @OrderAmount1 = 234, @OrderAmount2 = 700, @SortBy = 'OrderDate', @SortDirection = 'DESC'
--EXEC USP_GetCustomerDynamically @OrderAmount1 = 0, @OrderAmount2 = 700, @SortBy = 'OrderDate', @SortDirection = 'ASC'
--EXEC USP_GetCustomerDynamically @ItemQuantity1 = 5, @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamically @ItemQuantity2 = 8, @SortBy = 'OrderDate', @SortDirection = 'ASC'
--EXEC USP_GetCustomerDynamically @ItemQuantity1 = 5,  @ItemQuantity2 = 10,  @SortBy = 'OrderDate', @SortDirection = 'ASC'
--EXEC USP_GetCustomerDynamically @ItemQuantity1 = 5, @Location = 'SURAT' , @SortBy = 'OrderDate', @SortDirection = 'ASC'
--EXEC USP_GetCustomerDynamically @ItemQuantity1 = 5, @Location = 'Baroda' , @SortBy = 'OrderDate', @SortDirection = 'ASC'
--EXEC USP_GetCustomerDynamically @ItemQuantity1 = 5, @RestaurantID = 1,  @Location = 'Surat' , @SortBy = 'OrderDate', @SortDirection = 'ASC'
--EXEC USP_GetCustomerDynamically @OrderDateTo = '2022-08-21',  @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamically @OrderDateFrom = '2021-08-11', @OrderDateTo = '2022-08-26',  @SortBy = 'OrderDate', @SortDirection = 'ASC'
--EXEC USP_GetCustomerDynamically @CustomerName = 'R'

ALTER   PROCEDURE [dbo].[USP_GetCustomerDynamically] (

        @CustomerName NVARCHAR(100) = NULL,
		@OrderID INT = NULL,
		@RestaurantName NVARCHAR(100) = NULL,
		@Location NVARCHAR(100) = NULL,
		@ItemQuantity1 INT = NULL,
		@ItemQuantity2 INT = NULL,
		@OrderAmount1 FLOAT = NULL,
		@OrderAmount2 FLOAT = NULL,
		@OrderDateFrom DATE = NULL,
		@OrderDateTo DATE = NULL,
		@SortBy NVARCHAR(50) = 'CustomerName',
		@SortDirection NVARCHAR(10) = ' ASC'
)
AS
BEGIN
	
	SET @CustomerName = LTRIM(RTRIM(@CustomerName))  
	SET @Location = LTRIM(RTRIM(@Location))  

	IF(@ItemQuantity1 IS NOT NULL AND @ItemQuantity2 IS NULL)
	BEGIN
	SET @ItemQuantity2 = @ItemQuantity1
	END


	IF(@OrderAmount1 IS NOT NULL AND @OrderAmount2 IS NULL)
	BEGIN
	SET @OrderAmount2 = @OrderAmount1
	END

	IF(@OrderDateFrom IS NOT NULL AND @OrderDateTo IS NULL)
	BEGIN
	SET @OrderDateTo = @OrderDateFrom
	END
	ELSE IF(@OrderDateTo IS NOT NULL AND @OrderDateFrom IS NULL)
	BEGIN
	SET @OrderDateFrom = @OrderDateTo
	END

	
	SELECT C.CustomerID,C.CustomerName, R.RestaurantName,  D.RestaurantID,D.DiningTableID,D.[Location],
	O.OrderID,O.ItemQuantity,O.OrderAmount,O.OrderDate FROM Bills B
	INNER JOIN Customer C
		ON B.CustomerID =  C.CustomerID
	INNER JOIN [Order] O
		ON O.OrderID = B.OrderID
	INNER JOIN DiningTable D
		ON O.DiningTableID = D.DiningTableID
	INNER JOIN Restaurant R
		ON O.RestaurantID = R.RestaurantID
	WHERE (@CustomerName IS NULL OR C.CustomerName LIKE + '%' + @CustomerName  + '%')
		AND (@OrderID IS NULL OR O.OrderID = @OrderID)
		AND (@RestaurantName IS NULL OR R.RestaurantName LIKE + '%' + @RestaurantName  + '%')

		AND (@Location IS NULL OR D.[Location] LIKE + '%' + @Location  + '%')
		AND (@ItemQuantity1 IS NULL OR @ItemQuantity2 IS NULL OR O.ItemQuantity BETWEEN @ItemQuantity1 AND @ItemQuantity2)
		AND (@OrderAmount1 IS NULL OR @OrderAmount2 IS NULL OR O.OrderAmount BETWEEN @OrderAmount1 AND @OrderAmount2)
		AND (@OrderDateFrom IS NULL OR @OrderDateTo IS NULL OR CAST(O.OrderDate as date)  BETWEEN @OrderDateFrom AND @OrderDateTo)

	ORDER BY  

		CASE WHEN (@SortBy = 'CustomerName' AND @SortDirection='ASC')  
							THEN C.CustomerName  
				END ASC,  
				CASE WHEN (@SortBy = 'CustomerName' AND @SortDirection='DESC')  
							THEN C.CustomerName  
				END DESC,
		CASE WHEN (@SortBy = 'RestaurantName' AND @SortDirection='ASC')  
							THEN R.RestaurantName
				END ASC,  
				CASE WHEN (@SortBy = 'RestaurantName' AND @SortDirection='DESC')  
							  THEN R.RestaurantName
				END DESC,
		CASE WHEN (@SortBy = 'OrderID' AND @SortDirection='ASC')  
							THEN O.OrderID
				END ASC,  
				CASE WHEN (@SortBy = 'OrderID' AND @SortDirection='DESC')  
							  THEN O.OrderID
				END DESC,
		CASE WHEN (@SortBy = 'Location' AND @SortDirection='ASC')  
							THEN D.[Location]
				END ASC,  
				CASE WHEN (@SortBy = 'Location' AND @SortDirection='DESC')  
							THEN D.[Location]
				END DESC,
		CASE WHEN (@SortBy = 'ItemQuantity' AND @SortDirection='ASC')  
							THEN O.ItemQuantity
				END ASC,  
				CASE WHEN (@SortBy = 'ItemQuantity' AND @SortDirection='DESC')  
							THEN O.ItemQuantity
				END DESC,
		CASE WHEN (@SortBy = 'OrderAmount' AND @SortDirection='ASC')  
							THEN O.OrderAmount
				END ASC,  
				CASE WHEN (@SortBy = 'OrderAmount' AND @SortDirection='DESC')  
							THEN O.OrderAmount
				END DESC,
		CASE WHEN (@SortBy = 'OrderDate' AND @SortDirection='ASC')  
							THEN O.OrderDate
				END ASC,
		CASE WHEN (@SortBy = 'OrderDate' AND @SortDirection='DESC')  
							THEN O.OrderDate
				END DESC
END

/****** Object:  StoredProcedure [dbo].[USP_GetDiningTableFromRestaurantId]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
  
--select * from Cuisine  
--update Cuisine set RestaurantID= 7 where CuisineID=12  
  
CREATE OR ALTER  PROCEDURE [dbo].[USP_GetDiningTableFromRestaurantId] (  
                                @RestaurantID INT)          
AS  
BEGIN  
     SELECT DiningTableID, RestaurantID,[Location] FROM DiningTable   
     WHERE RestaurantID = @RestaurantID
END  


GO
/****** Object:  StoredProcedure [dbo].[USP_GetMenuItemFromRestaurantId]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--select * from Cuisine
--update Cuisine set RestaurantID= 7 where CuisineID=12

CREATE OR ALTER   PROCEDURE [dbo].[USP_GetMenuItemFromRestaurantId] (
                                @RestaurantID INT)								
AS
BEGIN
     SELECT MenuItemID, CuisineID,ItemName, ItemPrice FROM RestaurantMenuItem 
	 WHERE CuisineID IN (SELECT RestaurantID FROM Cuisine WHERE RestaurantID = @RestaurantID)
END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetOrder]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[USP_GetOrder] (
                                @OrderID INT = NULL
                              )								
AS
BEGIN
   SELECT [Order].OrderID,[Order].OrderDate,[Order].RestaurantID,[Order].MenuItemID,
   ItemQuantity, OrderAmount, DiningTableID, Restaurant.RestaurantName,
    RestaurantMenuItem.ItemName, RestaurantMenuItem.ItemPrice
	FROM [Order] 
	Inner join Restaurant on [Order].RestaurantID = Restaurant.RestaurantID
	Inner join RestaurantMenuItem on [Order].MenuItemID = RestaurantMenuItem.MenuItemID
	WHERE @OrderID IS NULL OR [Order].OrderID = @OrderID

END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetRestaurant]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[USP_GetRestaurant] (
                                @RestaurantID INT = NULL
                              )								
AS
BEGIN
   SELECT RestaurantID,
	RestaurantName,
	[Address],
	MobileNo 
	FROM Restaurant WHERE @RestaurantID IS NULL OR Restaurant.RestaurantID = @RestaurantID

END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetRestaurantProfitByYear]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER  PROCEDURE  [dbo].[USP_GetRestaurantProfitByYear] (
											 @ListBy nVarchar(10))
AS
BEGIN
	IF(@ListBy = 'Restaurant')
	BEGIN
			SELECT		R.RestaurantID,R.RestaurantName,Sum(O.OrderAmount) AS TotalAmount 
			FROM		Restaurant R
			INNER JOIN
						[Order] O
			ON			R.RestaurantID = O.RestaurantID
			Group By 
						R.RestaurantID,R.RestaurantName
			Order BY
						R.RestaurantID;
	END
	ELSE IF(@ListBy = 'OrderDate' )
	BEGIN
			SELECT  R.RestaurantID,R.RestaurantName,
					CAST(O.OrderDate AS DATE) AS OrdersDate,
					YEAR(CAST(O.OrderDate AS DATE)) [year],
					Sum(O.OrderAmount) as TotalAmount 
			FROM 
					Restaurant R
			INNER JOIN 
					[Order] O
			ON		R.RestaurantID = O.RestaurantID
			Group By	
					CAST(O.OrderDate AS DATE),R.RestaurantID,R.RestaurantName
			Order BY
					CAST(O.OrderDate AS DATE);

		
	END
    
END
GO
/****** Object:  StoredProcedure [dbo].[USP_Order]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Insert
--Exec USP_Order @OrderID = 0,@RestaurantID = 4, @MenuItemID = 10,@ItemQuantity = 4,@DiningTableID = 12,@IsDelete = 0,@OutputId =0
--Update
--Exec USP_Order @OrderID = 43,@RestaurantID = 4, @MenuItemID = 11,@ItemQuantity = 5,@DiningTableID = 12,@IsDelete = 0,@OutputId =0
--Delete
--Exec USP_Order @OrderID = 44,@RestaurantID = 4, @MenuItemID = 10,@ItemQuantity = 4,@DiningTableID = 12,@IsDelete = 1,@OutputId =0

CREATE OR ALTER  PROCEDURE [dbo].[USP_Order] (
                                @OrderID INT,                                
								@RestaurantID INT,
								@MenuItemID INT,
								@ItemQuantity INT,								
								@DiningTableID INT,
								@IsDelete INT,
								@OrderDate DATETIME = NULL,								
								@OutputId INT OUTPUT
								)								
AS
BEGIN
BEGIN TRY

	BEGIN TRANSACTION
			
		    IF(@IsDelete = 1 AND ((@OrderID <= 0) OR (NOT EXISTS (SELECT 1 FROM [Order] WHERE OrderID = @OrderID))))
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid order id',11,1); 
			END
			ELSE IF (@IsDelete = 0)
			BEGIN
				-- in case of add / delete
				IF (@OrderDate IS NULL)
				BEGIN
					Set @OrderDate = GETDATE();
				END

				IF (@ItemQuantity <= 0)
				BEGIN
					RAISERROR ('ItemQuantity should be greater than 0',11,1,@ItemQuantity); 
				END		

			END

			IF (@IsDelete = 0 AND @OrderID = 0)
			BEGIN
			
				INSERT INTO [Order]
								(RestaurantID,
									OrderDate,
									MenuItemID,
									ItemQuantity,
									OrderAmount,
									DiningTableID)
					VALUES     ( @RestaurantID,
									GETDATE(),
									@MenuItemID,
									@ItemQuantity,
									[dbo].[UDF_GetItemPrice](@MenuItemID) * @ItemQuantity,
									@DiningTableID);
					
				Update DiningTableTrack Set TableStatus = 'Occupied'
				 WHERE DiningTableID = @DiningTableID;
				SElECT @OutputId = @@ROWCOUNT;
				Print @OutputId
			END	
			ELSE IF (@IsDelete = 0 AND @OrderID > 0)
			BEGIN
			-- Update Operation
					UPDATE [Order]
						SET RestaurantID = @RestaurantID,
							OrderDate = @OrderDate,
							MenuItemID = @MenuItemID,
							ItemQuantity = @ItemQuantity,
							OrderAmount = [dbo].[UDF_GetItemPrice](@MenuItemID) * @ItemQuantity,
							DiningTableID = @DiningTableID
						WHERE OrderID  = @OrderID 
						AND OrderID NOT IN (Select OrderID From Bills);	
						SELECT @OutputId = @@ROWCOUNT
						Print @OutputId
			END
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM [Order] WHERE OrderID = @OrderID 
									AND OrderID NOT IN (Select OrderID From Bills);
				Update DiningTableTrack Set TableStatus = 'Vacant'
									WHERE DiningTableID = @DiningTableID;
				SELECT @OutputId = @@ROWCOUNT
				Print @OutputId
			IF(@OutputId = 0)
			BEGIN
				RAISERROR ('Cannot Allow Update/Delete on this OrderId,Bills are already processed on these OrderID',11,1,@OrderID ); 
			END
			END

	IF @@TRANCOUNT > 0
	BEGIN
	  COMMIT TRANSACTION
	END
END TRY
BEGIN CATCH
	
	   IF @@TRANCOUNT > 0
		BEGIN
		 --Print 'Roll back tranaction'
		  ROLLBACK TRANSACTION
        END

	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		
		
END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USP_Restaurant]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


------(1) This sp holds type the Insert, Update, Delete of table Restaurant


--Select * from Restaurant
--Insert the Restaurant
--Exec USP_Restaurant @RestaurantID = 0 , @RestaurantName = 'Mahi', @Address = '45-New Vadilal Cross Road', @MobileNo=7865445344, @IsDelete = 0,@OutpurRestaurantId=0

--Update the Restaurant
--Exec USP_Restaurant @RestaurantID = 6 , @RestaurantName = 'Rahi', @Address = '45-New Swiss Road', @MobileNo=7865555344, @IsDelete = 0,@OutpurRestaurantId=0

--Delete Transaction made on the Used Restaurant Id in Cuisine which will rollback the transaction
--Exec USP_Restaurant @RestaurantID = 4 , @RestaurantName = 'Mahi', @Address = '45-New Vadilal Cross Road', @MobileNo=7865445344, @IsDelete = 1,@OutpurRestaurantId=0

--Select * From Restaurant
--sp_helptext USP_Restaurant
/****** Object:  StoredProcedure [dbo].[USP_Order]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER   PROCEDURE [dbo].[USP_Restaurant] (
                                @RestaurantID INT,
                                @RestaurantName NVARCHAR(200),
								@Address NVARCHAR(500),
								@MobileNo NVARCHAR(10),
								@IsDelete BIT,
								@OutpurRestaurantId INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION

	       IF(@IsDelete = 1 AND ((@RestaurantID <= 0) OR (NOT EXISTS (SELECT 1 FROM Restaurant WHERE RestaurantID = @RestaurantID))))
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid restaurant id',11,1); 
			END
			ELSE IF (@IsDelete = 0)
			BEGIN
				-- in case of add / delete
				IF (LEN(ISNULL(@MobileNo,'')) <> 10)
				BEGIN
					RAISERROR ('Mobile number should be exactly 10 digits.',11,1,@MobileNo); 
				END

				IF (LEN(ISNULL(@Address,'')) < 10)
				BEGIN
					RAISERROR ('Address should be atleast more than 10 characters.',11,1,@Address); 
				END

				IF (ISNUMERIC(LEFT(ISNULL(@Address,''),1)) = 0)
				BEGIN
				   RAISERROR ('1st letter of the address column should be numeric only.',11,1,@Address); 
				END

				IF(ISNULL(@RestaurantName,'') = '')
				BEGIN
				   RAISERROR ('Restaurant Name should not be null.',11,1,@RestaurantName); 
				END

			END

			IF (@IsDelete = 0 AND @RestaurantID = 0)
			BEGIN
			   -- Add Operation
				INSERT INTO Restaurant
								(RestaurantName,
									[Address],
									MobileNo)
					VALUES     ( @RestaurantName,
									@Address,
									@MobileNo);

									SElECT @OutpurRestaurantId = @@ROWCOUNT;
				Print @OutpurRestaurantId
			END
			ELSE IF (@IsDelete = 0 AND @RestaurantID > 0)
			BEGIN
			-- Update Operation
					UPDATE Restaurant
						SET RestaurantName = @RestaurantName,
							[Address] = @Address,
							MobileNo = @MobileNo                   
						WHERE RestaurantID  = @RestaurantID;

							SElECT @OutpurRestaurantId = @@ROWCOUNT;
			END
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM Restaurant WHERE RestaurantID = @RestaurantID;

					SElECT @OutpurRestaurantId = @@ROWCOUNT;
			END
	
			
	IF @@TRANCOUNT > 0
	BEGIN
	  COMMIT TRANSACTION
	END
END TRY
BEGIN CATCH
	
	 IF @@TRANCOUNT > 0
		BEGIN
		 --Print 'Roll back tranaction'
		  ROLLBACK TRANSACTION
        END

	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		
		

END CATCH

END
GO
GO
/****** Object:  StoredProcedure [dbo].[USP_RestaurantMenuItem]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
------(3) This sp holds type the Insert, Update, Delete of table RestaurantMenuItem

--Insert
--EXEC USP_RestaurantMenuItem @MenuItemID = 0, @CuisineID = 21,@ItemName = 'Pizza Triplet',@ItemPrice = 155,@IsDelete = 0, @OutputMenuItemID  = 0
--Update
--EXEC USP_RestaurantMenuItem @MenuItemID = 11, @CuisineID = 21,@ItemName = 'Pizza Sandwich',@ItemPrice = 155,@IsDelete = 0, @OutputMenuItemID  = 0
--Delete 
--EXEC USP_RestaurantMenuItem @MenuItemID = 12, @CuisineID = 21,@ItemName = 'Pizza Triplet',@ItemPrice = 155,@IsDelete = 1, @OutputMenuItemID  = 0
--select * from RestaurantMenuItem
--Select * from Cuisine
--update Cuisine set RestaurantID= 7 where CuisineID=12

CREATE OR ALTER  PROCEDURE [dbo].[USP_RestaurantMenuItem] (
								@MenuItemID INT,
                                @CuisineID INT,
                                @ItemName NVARCHAR(100),	
								@ItemPrice FLOAT,	
								@IsDelete BIT,
								@OutputMenuItemID INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION

			IF(@IsDelete = 1 AND @MenuItemID <= 0)
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid MenuItem id',11,1); 
			END
			ELSE IF (@IsDelete = 0)
			BEGIN
				-- in case of add / delete				
				IF(ISNULL(@ItemName, '') = '')
				BEGIN
				   RAISERROR ('Item Name should not be null.',11,1,@ItemName); 
				END

				IF(@ItemPrice <= 0)
				BEGIN
				   RAISERROR ('Item Price should be greater than 0.',11,1); 
				END

			END
			IF (@IsDelete = 0 AND @MenuItemID = 0)
			 BEGIN
			   -- Add Operation
				INSERT INTO RestaurantMenuItem
								(CuisineID,
									ItemName,
									ItemPrice)
					VALUES     ( @CuisineID,
									@ItemName,
									@ItemPrice);

				SElECT @OutputMenuItemID = @@ROWCOUNT;
			END
			ELSE IF (@IsDelete = 0 AND @MenuItemID > 0)
			BEGIN
			-- Update Operation
					UPDATE RestaurantMenuItem
						SET CuisineID = @CuisineID,
							ItemName = @ItemName,
							ItemPrice = @ItemPrice
						WHERE MenuItemID  = @MenuItemID;
					SElECT @OutputMenuItemID = @@ROWCOUNT;
			END
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM RestaurantMenuItem WHERE MenuItemID  = @MenuItemID;

				SElECT @OutputMenuItemID = @@ROWCOUNT;
			END
			
	IF @@TRANCOUNT > 0
	BEGIN
	  COMMIT TRANSACTION
	END
END TRY
BEGIN CATCH
	 IF @@TRANCOUNT > 0
		BEGIN
		 --Print 'Roll back tranaction'
		  ROLLBACK TRANSACTION
        END

	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

END CATCH;

END;
GO
/****** Object:  StoredProcedure [dbo].[USP_VacantTableInRestaurant]    Script Date: 08/10/2022 23:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER  PROCEDURE  [dbo].[USP_VacantTableInRestaurant] (
											 @RestaurantID INT)
AS
BEGIN
			SELECT		R.RestaurantID,R.RestaurantName,
						D.DiningTableID 
			FROM		Restaurant R
			INNER JOIN
						DiningTable D
			ON 
						R.RestaurantID = D.RestaurantID
			INNER JOIN
						DiningTableTrack DT
			ON 
						D.DiningTableID = DT.DiningTableID
			Where 
						DT.TableStatus = 'Vacant'
			AND			R.RestaurantID = @RestaurantID;
    
END
GO
