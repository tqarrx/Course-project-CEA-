USE [master]
GO
/****** Object:  Database [CurrencyExchangeAccounting]    Script Date: 27.04.2025 23:03:28 ******/
CREATE DATABASE [CurrencyExchangeAccounting]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CurrencyExchangeAccounting_Data', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\CurrencyExchangeAccounting.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CurrencyExchangeAccounting_Log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\CurrencyExchangeAccounting.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CurrencyExchangeAccounting].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET ARITHABORT OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET  MULTI_USER 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET QUERY_STORE = OFF
GO
USE [CurrencyExchangeAccounting]
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 27.04.2025 23:03:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clients](
	[Client_ID] [int] IDENTITY(1,1) NOT NULL,
	[Client_Full_Name] [varchar](255) NOT NULL,
	[Passport_Series] [varchar](4) NOT NULL,
	[Passport_Number] [varchar](6) NOT NULL,
	[Department_Code] [varchar](7) NOT NULL,
	[Issued_By] [varchar](255) NOT NULL,
	[Issued_Date] [date] NOT NULL,
 CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[Client_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Currencies]    Script Date: 27.04.2025 23:03:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currencies](
	[Currency_ID] [int] IDENTITY(1,1) NOT NULL,
	[Rate_ID] [int] NOT NULL,
	[Currency_Name] [varchar](50) NOT NULL,
	[Remaining_Amount] [decimal](18, 2) NOT NULL,
	[Currency_Code] [char](3) NOT NULL,
	[Currency_Symbol] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Currencies] PRIMARY KEY CLUSTERED 
(
	[Currency_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExchangeRates]    Script Date: 27.04.2025 23:03:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExchangeRates](
	[Rate_ID] [int] IDENTITY(1,1) NOT NULL,
	[Currency_ID] [int] NOT NULL,
	[Buy_Rate] [decimal](18, 6) NOT NULL,
	[Sell_Rate] [decimal](18, 6) NOT NULL,
 CONSTRAINT [PK_Exchange rates] PRIMARY KEY CLUSTERED 
(
	[Rate_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Operations]    Script Date: 27.04.2025 23:03:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Operations](
	[Operation_ID] [int] IDENTITY(1,1) NOT NULL,
	[User_ID_Correct] [int] NULL,
	[User_ID] [int] NOT NULL,
	[Client_ID] [int] NOT NULL,
	[Currency_ID_From] [int] NOT NULL,
	[Currency_ID_To] [int] NOT NULL,
	[Amount_From] [decimal](18, 4) NOT NULL,
	[Currency_Name_From] [varchar](50) NOT NULL,
	[Amount_To] [decimal](18, 4) NOT NULL,
	[Currency_Name_To] [varchar](50) NOT NULL,
	[Operation_Date] [datetime] NOT NULL,
	[Last_Update_Date] [datetime] NULL,
 CONSTRAINT [PK_Operations] PRIMARY KEY CLUSTERED 
(
	[Operation_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 27.04.2025 23:03:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Role_ID] [int] IDENTITY(1,1) NOT NULL,
	[Role_Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Role_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 27.04.2025 23:03:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[User_ID] [int] IDENTITY(1,1) NOT NULL,
	[Login] [varchar](50) NOT NULL,
	[Password] [varchar](255) NOT NULL,
	[User_Full_Name] [varchar](100) NOT NULL,
	[Role_ID] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[User_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Clients] ON 

INSERT [dbo].[Clients] ([Client_ID], [Client_Full_Name], [Passport_Series], [Passport_Number], [Department_Code], [Issued_By], [Issued_Date]) VALUES (3, N'Чуркин Роман Владимирович', N'1234', N'567890', N'123', N'МВД', CAST(N'2025-04-27' AS Date))
SET IDENTITY_INSERT [dbo].[Clients] OFF
GO
SET IDENTITY_INSERT [dbo].[Currencies] ON 

INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (1, 1, N'Российский рубль', CAST(826.55 AS Decimal(18, 2)), N'RUB', N'₽')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (2, 2, N'Доллар США', CAST(0.00 AS Decimal(18, 2)), N'USD', N'$')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (3, 3, N'Евро', CAST(0.00 AS Decimal(18, 2)), N'EUR', N'€')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (4, 4, N'Фунт стерлингов', CAST(0.00 AS Decimal(18, 2)), N'GBP', N'£')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (5, 5, N'Китайский юань', CAST(0.00 AS Decimal(18, 2)), N'CNY', N'¥')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (6, 6, N'Японская иена', CAST(0.00 AS Decimal(18, 2)), N'JPY', N'¥')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (7, 7, N'Армянский драм', CAST(0.00 AS Decimal(18, 2)), N'AMD', N'֏')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (8, 8, N'Швейцарский франк', CAST(0.00 AS Decimal(18, 2)), N'CHF', N'₣')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (9, 9, N'Австралийский доллар', CAST(0.00 AS Decimal(18, 2)), N'AUD', N'A$')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (10, 10, N'Азербайджанский манат', CAST(0.00 AS Decimal(18, 2)), N'AZN', N'₼')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (11, 11, N'Белорусский рубль', CAST(0.00 AS Decimal(18, 2)), N'BYN', N'Br')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (12, 12, N'Болгарский лев', CAST(0.00 AS Decimal(18, 2)), N'BGN', N'лв')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (13, 13, N'Бразильский реал', CAST(0.00 AS Decimal(18, 2)), N'BRL', N'R$')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (14, 14, N'Венгерский форинт', CAST(0.00 AS Decimal(18, 2)), N'HUF', N'Ft')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (15, 15, N'Вона Республики Корея', CAST(0.00 AS Decimal(18, 2)), N'KRW', N'₩')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (16, 16, N'Дацкая крона', CAST(0.00 AS Decimal(18, 2)), N'DKK', N'kr')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (17, 17, N'Индийская рупия', CAST(0.00 AS Decimal(18, 2)), N'INR', N'₹')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (18, 18, N'Киргизский сом', CAST(0.00 AS Decimal(18, 2)), N'KGS', N'с')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (19, 19, N'Казахстанский тенге', CAST(0.00 AS Decimal(18, 2)), N'KZT', N'₸')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (20, 20, N'Канадский доллар', CAST(0.00 AS Decimal(18, 2)), N'CAD', N'C$')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (21, 21, N'Молдавский лей', CAST(0.00 AS Decimal(18, 2)), N'MDL', N'L')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (22, 22, N'Норвежская крона', CAST(0.00 AS Decimal(18, 2)), N'NOK', N'kr')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (23, 23, N'Польский злотый', CAST(0.00 AS Decimal(18, 2)), N'PLN', N'zl')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (24, 24, N'Румынский лей', CAST(0.00 AS Decimal(18, 2)), N'RON', N'lei')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (25, 25, N'СДР', CAST(0.00 AS Decimal(18, 2)), N'XDR', N'XDR')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (26, 26, N'Сингапурский доллар', CAST(0.00 AS Decimal(18, 2)), N'SGD', N'S$')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (27, 27, N'Таджикский сомони', CAST(0.00 AS Decimal(18, 2)), N'TJS', N'ЅМ')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (28, 28, N'Турецкая лира', CAST(0.00 AS Decimal(18, 2)), N'TRY', N'₺')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (29, 29, N'Новый туркменский манат', CAST(0.00 AS Decimal(18, 2)), N'TMT', N'm')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (30, 30, N'Узбекский сум', CAST(0.00 AS Decimal(18, 2)), N'UZS', N'сўм')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (31, 31, N'Украинская гривна', CAST(0.00 AS Decimal(18, 2)), N'UAH', N'₴')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (32, 32, N'Чешская крона', CAST(0.00 AS Decimal(18, 2)), N'CZK', N'Kc')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (33, 33, N'Шведская крона', CAST(0.00 AS Decimal(18, 2)), N'SEK', N'kr')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (34, 34, N'Южноафриканский рэнд', CAST(0.00 AS Decimal(18, 2)), N'ZAR', N'R')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (35, 35, N'Вьетнамский донг', CAST(0.00 AS Decimal(18, 2)), N'VND', N'₫')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (36, 36, N'Гонконгский доллар', CAST(0.00 AS Decimal(18, 2)), N'HKD', N'HK$')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (37, 37, N'Грузинский лари', CAST(0.00 AS Decimal(18, 2)), N'GEL', N'₾')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (38, 38, N'Дирхам ОАЭ', CAST(0.00 AS Decimal(18, 2)), N'AED', N'د.إ')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (39, 39, N'Динар Кувейта', CAST(0.00 AS Decimal(18, 2)), N'KWD', N'د.ك')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (40, 40, N'Египетский фунт', CAST(0.00 AS Decimal(18, 2)), N'EGP', N'ج.م')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (41, 41, N'Иракский динар', CAST(0.00 AS Decimal(18, 2)), N'IQD', N'ع.د')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (42, 42, N'Иранский риал', CAST(0.00 AS Decimal(18, 2)), N'IRR', N'﷼')
INSERT [dbo].[Currencies] ([Currency_ID], [Rate_ID], [Currency_Name], [Remaining_Amount], [Currency_Code], [Currency_Symbol]) VALUES (43, 43, N'Ливанский фунт', CAST(0.00 AS Decimal(18, 2)), N'LBP', N'ل.ل')
SET IDENTITY_INSERT [dbo].[Currencies] OFF
GO
SET IDENTITY_INSERT [dbo].[ExchangeRates] ON 

INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (1, 1, CAST(1.000000 AS Decimal(18, 6)), CAST(1.000000 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (2, 2, CAST(82.654900 AS Decimal(18, 6)), CAST(84.307998 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (3, 3, CAST(94.359300 AS Decimal(18, 6)), CAST(96.246486 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (4, 4, CAST(110.055000 AS Decimal(18, 6)), CAST(112.256100 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (5, 5, CAST(11.350600 AS Decimal(18, 6)), CAST(11.577612 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (6, 6, CAST(0.578006 AS Decimal(18, 6)), CAST(0.589566 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (7, 7, CAST(0.211756 AS Decimal(18, 6)), CAST(0.215991 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (8, 8, CAST(99.488300 AS Decimal(18, 6)), CAST(101.478066 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (9, 9, CAST(52.461100 AS Decimal(18, 6)), CAST(53.510322 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (10, 10, CAST(48.620500 AS Decimal(18, 6)), CAST(49.592910 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (11, 11, CAST(27.053000 AS Decimal(18, 6)), CAST(27.594060 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (12, 12, CAST(48.075900 AS Decimal(18, 6)), CAST(49.037418 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (13, 13, CAST(14.568600 AS Decimal(18, 6)), CAST(14.859972 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (14, 14, CAST(0.230397 AS Decimal(18, 6)), CAST(0.235004 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (15, 15, CAST(0.057792 AS Decimal(18, 6)), CAST(0.058948 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (16, 16, CAST(12.595000 AS Decimal(18, 6)), CAST(12.846900 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (17, 17, CAST(0.965834 AS Decimal(18, 6)), CAST(0.985150 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (18, 18, CAST(0.945168 AS Decimal(18, 6)), CAST(0.964071 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (19, 19, CAST(0.159825 AS Decimal(18, 6)), CAST(0.163021 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (20, 20, CAST(59.584000 AS Decimal(18, 6)), CAST(60.775680 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (21, 21, CAST(4.796450 AS Decimal(18, 6)), CAST(4.892379 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (22, 22, CAST(7.949270 AS Decimal(18, 6)), CAST(8.108255 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (23, 23, CAST(21.976300 AS Decimal(18, 6)), CAST(22.415826 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (24, 24, CAST(18.856800 AS Decimal(18, 6)), CAST(19.233936 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (25, 25, CAST(112.101500 AS Decimal(18, 6)), CAST(114.343530 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (26, 26, CAST(62.888900 AS Decimal(18, 6)), CAST(64.146678 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (27, 27, CAST(7.770510 AS Decimal(18, 6)), CAST(7.925920 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (28, 28, CAST(2.160160 AS Decimal(18, 6)), CAST(2.203363 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (29, 29, CAST(23.615700 AS Decimal(18, 6)), CAST(24.088014 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (30, 30, CAST(0.006393 AS Decimal(18, 6)), CAST(0.006521 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (31, 31, CAST(1.979700 AS Decimal(18, 6)), CAST(2.019294 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (32, 32, CAST(3.765600 AS Decimal(18, 6)), CAST(3.840912 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (33, 33, CAST(8.617750 AS Decimal(18, 6)), CAST(8.790105 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (34, 34, CAST(4.395790 AS Decimal(18, 6)), CAST(4.483705 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (35, 35, CAST(0.003313 AS Decimal(18, 6)), CAST(0.003379 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (36, 36, CAST(10.675500 AS Decimal(18, 6)), CAST(10.889010 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (37, 37, CAST(30.065100 AS Decimal(18, 6)), CAST(30.666402 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (38, 38, CAST(22.506400 AS Decimal(18, 6)), CAST(22.956528 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (39, 39, CAST(0.018000 AS Decimal(18, 6)), CAST(0.018400 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (40, 40, CAST(1.622350 AS Decimal(18, 6)), CAST(1.654797 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (41, 41, CAST(0.095000 AS Decimal(18, 6)), CAST(0.097000 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (42, 42, CAST(0.002500 AS Decimal(18, 6)), CAST(0.002600 AS Decimal(18, 6)))
INSERT [dbo].[ExchangeRates] ([Rate_ID], [Currency_ID], [Buy_Rate], [Sell_Rate]) VALUES (43, 43, CAST(0.650000 AS Decimal(18, 6)), CAST(0.663000 AS Decimal(18, 6)))
SET IDENTITY_INSERT [dbo].[ExchangeRates] OFF
GO
SET IDENTITY_INSERT [dbo].[Operations] ON 

INSERT [dbo].[Operations] ([Operation_ID], [User_ID_Correct], [User_ID], [Client_ID], [Currency_ID_From], [Currency_ID_To], [Amount_From], [Currency_Name_From], [Amount_To], [Currency_Name_To], [Operation_Date], [Last_Update_Date]) VALUES (5, NULL, 2, 3, 2, 1, CAST(10.0000 AS Decimal(18, 4)), N'Доллар США', CAST(826.5500 AS Decimal(18, 4)), N'Российский рубль', CAST(N'2025-04-27T22:08:24.007' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[Operations] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([Role_ID], [Role_Name]) VALUES (1, N'Кассир-оператор')
INSERT [dbo].[Roles] ([Role_ID], [Role_Name]) VALUES (2, N'Старший кассир')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([User_ID], [Login], [Password], [User_Full_Name], [Role_ID]) VALUES (1, N'operator', N'123123', N'Иванов Иван Иванович', 1)
INSERT [dbo].[Users] ([User_ID], [Login], [Password], [User_Full_Name], [Role_ID]) VALUES (2, N'senior', N'123123', N'Сергеев Сергей Сергеевич', 2)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
ALTER TABLE [dbo].[Currencies] ADD  CONSTRAINT [DF_Currencies_Remaining_Amount]  DEFAULT ((0)) FOR [Remaining_Amount]
GO
ALTER TABLE [dbo].[Currencies]  WITH CHECK ADD  CONSTRAINT [FK_Currencies_ExchangeRates] FOREIGN KEY([Rate_ID])
REFERENCES [dbo].[ExchangeRates] ([Rate_ID])
GO
ALTER TABLE [dbo].[Currencies] CHECK CONSTRAINT [FK_Currencies_ExchangeRates]
GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD  CONSTRAINT [FK_Operations_Clients] FOREIGN KEY([Client_ID])
REFERENCES [dbo].[Clients] ([Client_ID])
GO
ALTER TABLE [dbo].[Operations] CHECK CONSTRAINT [FK_Operations_Clients]
GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD  CONSTRAINT [FK_Operations_Currencies1] FOREIGN KEY([Currency_ID_From])
REFERENCES [dbo].[Currencies] ([Currency_ID])
GO
ALTER TABLE [dbo].[Operations] CHECK CONSTRAINT [FK_Operations_Currencies1]
GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD  CONSTRAINT [FK_Operations_Currencies2] FOREIGN KEY([Currency_ID_To])
REFERENCES [dbo].[Currencies] ([Currency_ID])
GO
ALTER TABLE [dbo].[Operations] CHECK CONSTRAINT [FK_Operations_Currencies2]
GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD  CONSTRAINT [FK_Operations_Users] FOREIGN KEY([User_ID])
REFERENCES [dbo].[Users] ([User_ID])
GO
ALTER TABLE [dbo].[Operations] CHECK CONSTRAINT [FK_Operations_Users]
GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD  CONSTRAINT [FK_Operations_Users1] FOREIGN KEY([User_ID_Correct])
REFERENCES [dbo].[Users] ([User_ID])
GO
ALTER TABLE [dbo].[Operations] CHECK CONSTRAINT [FK_Operations_Users1]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([Role_ID])
REFERENCES [dbo].[Roles] ([Role_ID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles]
GO
USE [master]
GO
ALTER DATABASE [CurrencyExchangeAccounting] SET  READ_WRITE 
GO
