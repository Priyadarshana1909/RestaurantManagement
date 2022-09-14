DROP Table IF EXISTS  [dbo].[Bills]
GO
DROP Table IF EXISTS  [dbo].[Customer]
GO
DROP Table IF EXISTS  [dbo].[Order]
GO
DROP Table IF EXISTS  [dbo].[DiningTableTrack]
GO
DROP Table IF EXISTS  [dbo].[DiningTable]
GO
DROP Table IF EXISTS  [dbo].[RestaurantMenuItem]
GO
DROP Table IF EXISTS  [dbo].[Cuisine]
GO
DROP Table IF EXISTS  [dbo].[Restaurant]
GO
DROP Table IF EXISTS  [dbo].[Bills]
GO

--This table hold Restaurant Details
CREATE TABLE [dbo].[Restaurant] (
  	RestaurantID INT IDENTITY(1,1),
    RestaurantName NVARCHAR(200) NOT NULL,
	[Address] NVARCHAR(500) NOT NULL,
    MobileNo NVARCHAR(10),
	CONSTRAINT  CL_Restaurant_RestaurantID PRIMARY KEY (RestaurantID),
	CONSTRAINT  NC_Restaurant_RestaurantName_Unique UNIQUE (RestaurantName),
	CONSTRAINT NC_Restaurant_MobileNo_Unique UNIQUE (MobileNo)
  
);
GO

--This table holds type of cuisine served by restaurant
CREATE TABLE [dbo].[Cuisine] (
  	CuisineID INT IDENTITY(1,1),
	RestaurantID INT NOT NULL,
    CuisineName NVARCHAR(50) NOT NULL,
	CONSTRAINT  CL_Cuisine_CuisineID PRIMARY KEY (CuisineID),
	CONSTRAINT FK_Cuisine_Restaurant FOREIGN KEY (RestaurantID) REFERENCES Restaurant(RestaurantID),
	CONSTRAINT  NC_Cuisine_CuisineName_Unique UNIQUE (CuisineName)
);
GO

--This table holds detail Item list served in restaurant
CREATE TABLE [dbo].[RestaurantMenuItem] (
  	MenuItemID INT IDENTITY(1,1),
	CuisineID INT NOT NULL,
    ItemName NVARCHAR(100) NOT NULL,
	ItemPrice FLOAT,
	CONSTRAINT  CL_RestaurantMenuItem_MenuItemID PRIMARY KEY (MenuItemID),
	CONSTRAINT FK_RestaurantMenuItem_Cuisine FOREIGN KEY (CuisineID) REFERENCES Cuisine(CuisineID),
	CONSTRAINT  NC_RestaurantMenuItem_ItemName_Unique UNIQUE (ItemName)
);
GO

--This table holds data related to total available dining tables in restaurant.

CREATE TABLE [dbo].[DiningTable] (
  	DiningTableID INT IDENTITY(1,1),
	RestaurantID INT NOT NULL,
    [Location] NVARCHAR(100) NOT NULL,
	CONSTRAINT  CL_DiningTable_DiningTableID PRIMARY KEY (DiningTableID),
	CONSTRAINT FK_DiningTable_Restaurant FOREIGN KEY (RestaurantID) REFERENCES Restaurant(RestaurantID),
	CONSTRAINT  NC_DiningTable_Location_Unique UNIQUE ([Location])
);
GO

--This table holds status of the table, whether available / occupied.CREATE TABLE [dbo].[DiningTableTrack] (
  	DiningTableTrackID INT IDENTITY(1,1),
	DiningTableID INT NOT NULL,
    TableStatus NVARCHAR(100),
	CONSTRAINT  CL_DiningTableTrack_DiningTableTrackID PRIMARY KEY (DiningTableTrackID),
	CONSTRAINT FK_DiningTableTrack_DiningTable FOREIGN KEY (DiningTableID) REFERENCES DiningTable(DiningTableID)
);
GO

--This table holds data related to orders given by Customer

CREATE TABLE [dbo].[Order] (
  	OrderID INT IDENTITY(1,1),
	OrderDate DateTime NOT NULL,
    RestaurantID INT NOT NULL,
	MenuItemID INT NOT NULL,
	ItemQuantity INT NOT NULL,
	OrderAmount FLOAT NOT NULL,
	DiningTableID INT NOT NULL,
	CONSTRAINT  CL_Order_OrderID PRIMARY KEY (OrderID),
	CONSTRAINT FK_Order_Restaurant FOREIGN KEY (RestaurantID) REFERENCES Restaurant(RestaurantID),
	CONSTRAINT FK_Order_RestaurantMenuItem FOREIGN KEY (MenuItemID) REFERENCES RestaurantMenuItem(MenuItemID),
	CONSTRAINT FK_Order_DiningTable FOREIGN KEY (DiningTableID) REFERENCES DiningTable(DiningTableID)
);
GO

--This table holds Customer Data
CREATE TABLE [dbo].[Customer] (
  	CustomerID INT IDENTITY(1,1),
    RestaurantID INT NOT NULL,
	CustomerName NVARCHAR(100) NOT NULL,
	MobileNo NVARCHAR(10) NOT NULL,
	CONSTRAINT  CL_Customer_CustomerID PRIMARY KEY (CustomerID),
	CONSTRAINT FK_Customer_Restaurant FOREIGN KEY (RestaurantID) REFERENCES [Restaurant](RestaurantID)
);

GO
--This table holds billing data
CREATE TABLE [dbo].[Bills] (
  	BillsID INT IDENTITY(1,1),
    OrderID INT NOT NULL,
	RestaurantID INT NOT NULL,
	BillAmount FLOAT NOT NULL,
	CustomerID INT NOT NULL,
	CONSTRAINT  CL_Bills_BillsID PRIMARY KEY (BillsID),
	CONSTRAINT FK_Bills_Order FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
	CONSTRAINT FK_Bills_Restaurant FOREIGN KEY (RestaurantID) REFERENCES Restaurant(RestaurantID),
	CONSTRAINT FK_Bills_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);
GO


