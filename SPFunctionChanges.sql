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
--Exec [dbo].[USP_Billis] @IsDelete = 1,@RestaurantID = 0, @BillsID = 0, @OrderID = 0, @BillAmount = 0, @CustomerID =0, @OutputID = 0
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

--Exec [USP_Cuisine]  @CuisineID = 0, @RestaurantID = 0, @CuisineName = '', @IsDelete = 0, @OutputCuisineId = 0
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

--Declare @ID INT
--EXEC USP_Customer @CustomerID = 0, @RestaurantID ='3', @CustomerName = 'Priyanka Parmer ',@MobileNo = '8538978242',
--@IsDelete = 0
--SELECT @ID AS 'number of records affected'
--GO

--select * from RestaurantMenuItem
--update Cuisine set RestaurantID= 7 where CuisineID=12

--Select * from Customer


--Exec [dbo].[USP_Customer] @CustomerID = 1,@RestaurantID = 0, @CustomerName = '', @MobileNo = '', @IsDelete = 0, @OutputID = 0

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

--Declare @ID INT
--EXEC USP_DiningTable @DiningTableID = 4, @RestaurantID = 3,@Location = 'MG Palace',
--@IsDelete = 0, @OutputDiningTableID  = @ID OUTPUT
--SELECT @ID AS 'Record Number affected'
--GO

--select * from DiningTable
--update DiningTable set DiningTableID= 3 where CuisineID=12

--Insert into DiningTable Values (2,'Kadi')


--Exec [dbo].[USP_DiningTable] @DiningTableID = 1,@RestaurantID = 0, @Location = '',  @IsDelete = 0, @OutputDiningTableID = 0

CREATE OR ALTER PROCEDURE [dbo].[USP_DiningTable] (
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
				
				INSERT INTO DiningTableTrack (DiningTableID, TableStatus)
				VALUES (@OutputDiningTableID, 'Vacant');

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
GO
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
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC USP_GetCustomerDynamicallyNew  @SortBy = 'Location', @SortDirection = 'DESC'
--EXEC USP_GetCustomerDynamicallyNew @OrderAmount1 = 234, @OrderAmount2 = 700, @SortBy = 'OrderDate', @SortDirection = ' DESC'
--EXEC USP_GetCustomerDynamicallyNew @OrderAmount1 = 0, @OrderAmount2 = 700, @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamicallyNew @ItemQuantity1 = 5, @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamicallyNew @ItemQuantity1 = 5,  @ItemQuantity2 = 10, @DiningTableID = 2, @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamicallyNew @ItemQuantity1 = 5,  @ItemQuantity2 = 10, @DiningTableID = 7, @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamicallyNew @ItemQuantity1 = 5, @Location = 'SURAT' , @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamicallyNew @ItemQuantity1 = 5, @Location = 'Baroda' , @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamicallyNew @ItemQuantity1 = 5, @RestaurantID = 1,  @Location = 'Surat' , @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamicallyNew @OrderDateFrom = '2022-08-26',  @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamicallyNew @OrderDateFrom = '2021-08-11', @OrderDateTo = '2022-08-26',  @SortBy = 'OrderDate', @SortDirection = ' ASC'
--EXEC USP_GetCustomerDynamicallyNew @CustomerName = 'R'

CREATE OR ALTER  PROCEDURE [dbo].[USP_GetCustomerDynamically] ( 

    	@CustomerID		INT = NULL,
		@CustomerName	NVARCHAR(100) = NULL,
		@RestaurantID	INT = NULL,
		@DiningTableID	INT = NULL,
		@OrderID		INT = NULL,
		@Location		NVARCHAR(100) = NULL,
		@ItemQuantity1	INT = NULL,
		@ItemQuantity2	INT = NULL,
		@OrderAmount1	FLOAT = NULL,
		@OrderAmount2	FLOAT = NULL,
		@OrderDateFrom	DATE = NULL,
		@OrderDateTo	DATE = NULL,
		@SortBy NVARCHAR(50) = NULL,
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

SELECT		C.CustomerID,C.CustomerName,D.RestaurantID,D.DiningTableID,D.[Location], 
								O.OrderID,O.ItemQuantity,O.OrderAmount,O.OrderDate  from Bills B 
					INNER JOIN	Customer C
					ON			B.CustomerID =  C.CustomerID
					INNER JOIN	[Order] O
					ON			O.OrderID = B.OrderID
					INNER JOIN	DiningTable D
					ON			O.DiningTableID = D.DiningTableID
WHERE 	(@CustomerID IS NULL OR C.CustomerID = 	@CustomerID)
AND (@CustomerName IS NULL OR C.CustomerName LIKE + '%' + @CustomerName  + '%')
AND (@RestaurantID IS NULL OR D.RestaurantID = @RestaurantID)
AND (@DiningTableID IS NULL OR D.DiningTableID = @DiningTableID)
AND (@OrderID IS NULL OR O.OrderID = @OrderID)
AND (@Location IS NULL OR D.[Location] LIKE + '%' + @Location  + '%')
AND (@ItemQuantity1 IS NULL OR @ItemQuantity2 IS NULL OR O.ItemQuantity BETWEEN @ItemQuantity1 AND @ItemQuantity2)
AND (@OrderAmount1 IS NULL OR @OrderAmount2 IS NULL OR O.OrderAmount BETWEEN @OrderAmount1 AND @OrderAmount2)
AND (@OrderDateFrom IS NULL OR @OrderDateTo IS NULL OR CAST(O.OrderDate as date)  BETWEEN @OrderDateFrom AND @OrderDateTo)

ORDER BY  
			CASE WHEN (@SortBy = 'CustomerID' AND @SortDirection='ASC')  
                        THEN C.CustomerID  
            END ASC,  
            CASE WHEN (@SortBy = 'CustomerID' AND @SortDirection='DESC')  
                        THEN C.CustomerID 
            END DESC,
            CASE WHEN (@SortBy = 'CustomerName' AND @SortDirection='ASC')  
                        THEN C.CustomerName  
            END ASC,  
            CASE WHEN (@SortBy = 'CustomerName' AND @SortDirection='DESC')  
                        THEN C.CustomerName  
            END DESC,
			CASE WHEN (@SortBy = 'RestaurantID' AND @SortDirection='ASC')  
                        THEN D.RestaurantID 
            END ASC,  
            CASE WHEN (@SortBy = 'RestaurantID' AND @SortDirection='DESC')  
                        THEN D.RestaurantID
            END DESC,
			CASE WHEN (@SortBy = 'DiningTableID' AND @SortDirection='ASC')  
                        THEN D.DiningTableID 
            END ASC,  
            CASE WHEN (@SortBy = 'DiningTableID' AND @SortDirection='DESC')  
                        THEN D.DiningTableID
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
				
GO
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


--Declare @ID INT
--EXEC USP_Restaurant @RestaurantID = 3, @RestaurantName = 'Chocalate House', @Address = '2 Maninagar Ahmedabad', @MobileNo = '5134567890', 
--@IsDelete = 0, @OutpurRestaurantId  = @ID OUTPUT
--SELECT @ID AS 'Record Number Affected'
--GO

--Select * from Restaurant

--EXEC sys.sp_cdc_enable_table  
--@source_schema = N'dbo',  
--@source_name   = N'Employee_Main',  
--@role_name     = NULL,  
--@filegroup_name = NULL,  
--@supports_net_changes = 0 
--GO

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

--Declare @ID INT
--EXEC USP_RestaurantMenuItem @MenuItemID = 0, @CuisineID = 4,@ItemName = 'Dessert',@ItemPrice = 117.9,
--@IsDelete = 0, @OutputMenuItemID  = @ID OUTPUT
--SELECT @ID AS 'Record Number affected'
--GO

--select * from RestaurantMenuItem
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
