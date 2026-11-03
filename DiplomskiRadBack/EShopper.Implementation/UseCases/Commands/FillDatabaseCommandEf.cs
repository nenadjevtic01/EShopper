using Microsoft.EntityFrameworkCore;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class FillDatabaseCommandEf : UseCaseEf, IFillDatabaseCommand
    {
        public int UseCaseId => 18;

        public string UseCaseName => "Fill database";

        public string UseCaseDescription => "Initial fill database";

        public FillDatabaseCommandEf(EShopperDbContext context) : base(context)
        {
        }

        public void Execute(int request)
        {
            if (Context.Users.Any())
            {
                throw new Exception("Database already filled.");
            }

            Context.Database.ExecuteSqlRaw("ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = OFF");

            var carts = new List<Cart>
            {
                new Cart(),
                new Cart()
            };

            var infos = new List<Info>
            {
                new Info
                {
                    Address = "Adresa 1",
                    City = "Beograd",
                    PostalCode = "11000",
                    Country = "Srbija"
                },
                new Info
                {
                    Address = "Adresa 2",
                    City = "Novi Sad",
                    PostalCode = "12000",
                    Country = "Srbija"
                },
                new Info()
            };

            var users = new List<User>
            {
                new User
                {
                    FirstName = "Admin",
                    LastName = "Adminovic",
                    Email = "admin@gmail.com",
                    Username = "admin123",
                    Password = "$2a$11$Ahl8NDzdxDryw1hhJdck/OSJk8W6fZtCiiZbpDt2nRjyDdmHTJLyW",
                    IsBanned = false,
                    Role = 1,
                    Cart = carts[0],
                    Info = infos[0]
                },
                new User
                {
                    FirstName = "User",
                    LastName = "Useric",
                    Email = "user@gmail.com",
                    Username = "user123",
                    Password = "$2a$11$Ahl8NDzdxDryw1hhJdck/OSJk8W6fZtCiiZbpDt2nRjyDdmHTJLyW",
                    IsBanned = false,
                    Role = 2,
                    Cart = carts[1],
                    Info = infos[1]
                },
                new User
                {
                    FirstName = "Anon",
                    LastName = "Anonic",
                    Email = "anonimous@gmail.com",
                    Username = "anonimous123",
                    Password = "$2a$11$Ahl8NDzdxDryw1hhJdck/OSJk8W6fZtCiiZbpDt2nRjyDdmHTJLyW",
                    IsBanned = false,
                    Role = 2,
                    Info = infos[2]
                }
            };

            var sizes = new List<Size>
            {
                new Size
                {
                    SizeName = "XS"
                },
                new Size
                {
                    SizeName = "S"
                },
                new Size
                {
                    SizeName = "M"
                },
                new Size
                {
                    SizeName = "L"
                },
                new Size
                {
                    SizeName = "XL"
                }
            };

            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "Shirts",
                    Src = "../../../assets/img/shirts/BSFN2.png",
                    Alt = "T-shirt with layering image 1"
                },
                new Category
                {
                    CategoryName = "Jeans",
                    Src = "../../../assets/img/jeans/GOMS1.png",
                    Alt = "Stone Washed Relaxed Denim image 1"
                },
                new Category
                {
                    CategoryName = "Jumpers",
                    Src = "../../../assets/img/jumpers/GOMS4.png",
                    Alt = "Male Hoodie image 4"
                },
                new Category
                {
                    CategoryName = "Jackets",
                    Src = "../../../assets/img/jackets/GOMS2.png",
                    Alt = "Jacket with stand-up collar image 1"
                },
                new Category
                {
                    CategoryName = "Dress",
                    Src = "../../../assets/img/dress/FbFashion1.png",
                    Alt = "Swinging mini dress image 1"
                },
                new Category
                {
                    CategoryName = "Skirts",
                    Src = "../../../assets/img/skirts/Suima1.png",
                    Alt = "Women mini skirt image 1"
                },
                new Category
                {
                    CategoryName = "Shorts",
                    Src = "../../../assets/img/shorts/BSFN1.png",
                    Alt = "Sweatshorts image 1"
                }
            };

            var brands = new List<Brand>
            {
                new Brand
                {
                    BrandName = "Suima",
                    Src = "assets/img/brands/SuimaLogo.jpg",
                    Alt = "Suima brand logo"
                },
                new Brand
                {
                    BrandName = "Fb Fashion",
                    Src = "assets/img/brands/FbFashionLogo.jpg",
                    Alt = "Fb Fashion brand logo"
                },
                new Brand
                {
                    BrandName = "GOMS",
                    Src = "assets/img/brands/GomsLogo.jpg",
                    Alt = "Goms brand logo"
                },
                new Brand
                {
                    BrandName = "BSFN",
                    Src = "assets/img/brands/BsfnLogo.jpg",
                    Alt = "Bsfn brand logo"
                }
            };

            var gender = new List<Gender>
            {
                new Gender
                {
                    GenderName = "Male"
                },
                new Gender
                {
                    GenderName = "Female"
                }
            };

            var countries = new List<Country>
            {
                new Country { Name = "Albania" },
                new Country { Name = "Austria" },
                new Country { Name = "Belgium" },
                new Country { Name = "Bosnia and Herzegovina" },
                new Country { Name = "Bulgaria" },
                new Country { Name = "Croatia" },
                new Country { Name = "Czech Republic" },
                new Country { Name = "Denmark" },
                new Country { Name = "Finland" },
                new Country { Name = "France" },
                new Country { Name = "Germany" },
                new Country { Name = "Greece" },
                new Country { Name = "Hungary" },
                new Country { Name = "Ireland" },
                new Country { Name = "Italy" },
                new Country { Name = "Montenegro" },
                new Country { Name = "Netherlands" },
                new Country { Name = "North Macedonia" },
                new Country { Name = "Norway" },
                new Country { Name = "Poland" },
                new Country { Name = "Portugal" },
                new Country { Name = "Romania" },
                new Country { Name = "Serbia" },
                new Country { Name = "Slovakia" },
                new Country { Name = "Slovenia" },
                new Country { Name = "Spain" },
                new Country { Name = "Sweden" },
                new Country { Name = "Switzerland" },
                new Country { Name = "Turkey" },
                new Country { Name = "UK" },
                new Country { Name = "USA" },
                new Country { Name = "China" },
                new Country { Name = "India" },
                new Country { Name = "Bangladesh" },
                new Country { Name = "Vietnam" }
            };

            var navigationItems = new List<NavigationItem>
            {
                new NavigationItem { Name = "Home", Path = "/home", DisplayOrder = 1 },
                new NavigationItem { Name = "Shop", Path = "/shop", DisplayOrder = 2 },
                new NavigationItem { Name = "About us", Path = "/about", DisplayOrder = 3 },
                new NavigationItem { Name = "Contact", Path = "/contact", DisplayOrder = 4 }
            };

            var heroSlides = new List<HeroSlide>
            {
                new HeroSlide
                {
                    Tagline = "New collection",
                    Title = "Fresh styles for every day",
                    Subtitle = "Check out the newest arrivals in our shop.",
                    ButtonText = "Shop now",
                    ButtonLink = "/shop",
                    MediaType = "image",
                    MediaSrc = "assets/img/hero/image-cover.jpg",
                    SortOrder = 1
                },
                new HeroSlide
                {
                    Tagline = "Sale",
                    Title = "Up to 30% off",
                    Subtitle = "Selected items on discount while the offer lasts.",
                    ButtonText = "See offers",
                    ButtonLink = "/shop",
                    MediaType = "image",
                    MediaSrc = null,
                    SortOrder = 2
                }
            };

            var products = new List<Product>
            {
                new Product
                {
                    ProductName = "Oversize sweatshirt",
                    Category = categories[2],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Hoodie",
                    Category = categories[2],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Sweatshirt with shoulder detail",
                    Category = categories[2],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = false,
                    Material = "Cotton ",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "Oversize sweatshirt",
                    Category = categories[2],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "Hoodie",
                    Category = categories[2],
                    Brand = brands[1],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Hoodie",
                    Category = categories[2],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Hoodie",
                    Category = categories[2],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = false,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Hoodie",
                    Category = categories[2],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Wrapped midi dress",
                    Category = categories[4],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Swinging mini dress",
                    Category = categories[4],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "Knit mini dress",
                    Category = categories[4],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Mini party dress",
                    Category = categories[4],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "Women mini skirt",
                    Category = categories[5],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = false,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "Skater skirt",
                    Category = categories[5],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "Women mini skirt",
                    Category = categories[5],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Women mini skirt",
                    Category = categories[5],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Sweatshorts",
                    Category = categories[6],
                    Brand = brands[3],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[19]
                },
                new Product
                {
                    ProductName = "Sweatshorts",
                    Category = categories[6],
                    Brand = brands[3],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[19]
                },
                new Product
                {
                    ProductName = "Basketball shorts",
                    Category = categories[6],
                    Brand = brands[3],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[19]
                },
                new Product
                {
                    ProductName = "Shorts",
                    Category = categories[6],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Highwaist Shorts",
                    Category = categories[6],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Shorts",
                    Category = categories[6],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = false,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Mom Fit Jeans",
                    Category = categories[1],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "Mom Fit Jeans",
                    Category = categories[1],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Mom Fit Jeans",
                    Category = categories[1],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Mom Fit Jeans",
                    Category = categories[1],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "Stone Washed Relaxed Denim",
                    Category = categories[1],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Tapered Denim mit destroys",
                    Category = categories[1],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Washed Slim Fit Denim",
                    Category = categories[1],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Basic Slim Fit Denim",
                    Category = categories[1],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = false,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Jacket with applications",
                    Category = categories[3],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Bomber jacket",
                    Category = categories[3],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "Coat with hood",
                    Category = categories[3],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "Quilted jacket with hood",
                    Category = categories[3],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "Jacket with material mix",
                    Category = categories[3],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Jacket with stand-up collar",
                    Category = categories[3],
                    Brand = brands[2],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Parka",
                    Category = categories[3],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "Jacket with hood",
                    Category = categories[3],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = false,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "T-Shirt with print",
                    Category = categories[0],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "T-Shirt with print",
                    Category = categories[0],
                    Brand = brands[1],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[14]
                },
                new Product
                {
                    ProductName = "T-Shirt with print",
                    Category = categories[0],
                    Brand = brands[0],
                    Gender = gender[1],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[10]
                },
                new Product
                {
                    ProductName = "T-shirt with round neck",
                    Category = categories[0],
                    Brand = brands[3],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[19]
                },
                new Product
                {
                    ProductName = "T-shirt with layering",
                    Category = categories[0],
                    Brand = brands[3],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[19]
                },
                new Product
                {
                    ProductName = "T-shirt with round neck",
                    Category = categories[0],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = true,
                    Material = "Cotton",
                    Country = countries[29]
                },
                new Product
                {
                    ProductName = "T-shirt with round neck",
                    Category = categories[0],
                    Brand = brands[2],
                    Gender = gender[0],
                    InStock = false,
                    Material = "Cotton",
                    Country = countries[10]
                }
            };

            var pictures = new List<Picture>
            {
                new Picture
                {
                    Product = products[1],
                    Src = "../../../assets/img/jumpers/Suima2.png",
                    Alt = "Hoodie image 1"
                },
                new Picture
                {
                    Product = products[2],
                    Src = "../../../assets/img/jumpers/FbFashion1.png",
                    Alt = "Sweatshirt with shoulder detail image 1"
                },
                new Picture
                {
                    Product = products[4],
                    Src = "../../../assets/img/jumpers/GOMS1.png",
                    Alt = "Male Hoodie image 1"
                },
                new Picture
                {
                    Product = products[5],
                    Src = "../../../assets/img/jumpers/GOMS2.png",
                    Alt = "Male Hoodie image 2"
                },
                new Picture
                {
                    Product = products[6],
                    Src = "../../../assets/img/jumpers/GOMS3.png",
                    Alt = "Male Hoodie image 3"
                },
                new Picture
                {
                    Product = products[7],
                    Src = "../../../assets/img/jumpers/GOMS4.png",
                    Alt = "Male Hoodie image 4"
                },
                new Picture
                {
                    Product = products[8],
                    Src = "../../../assets/img/dress/Suima1.png",
                    Alt = "Wrapped midi dress image 1"
                },
                new Picture
                {
                    Product = products[9],
                    Src = "../../../assets/img/dress/FbFashion1.png",
                    Alt = "Swinging mini dress image 1"
                },
                new Picture
                {
                    Product = products[10],
                    Src = "../../../assets/img/dress/Suima2.png",
                    Alt = "Knit mini dress image 1"
                },
                new Picture
                {
                    Product = products[11],
                    Src = "../../../assets/img/dress/FbFashion2.png",
                    Alt = "Mini party dress image 1"
                },
                new Picture
                {
                    Product = products[12],
                    Src = "../../../assets/img/skirts/FbFashion1.png",
                    Alt = "Women mini skirt image 1"
                },
                new Picture
                {
                    Product = products[13],
                    Src = "../../../assets/img/skirts/FbFashion2.png",
                    Alt = "Skater skirt image 1"
                },
                new Picture
                {
                    Product = products[14],
                    Src = "../../../assets/img/skirts/Suima1.png",
                    Alt = "Women mini skirt image 1"
                },
                new Picture
                {
                    Product = products[15],
                    Src = "../../../assets/img/skirts/Suima2.png",
                    Alt = "Women mini skirt image 2"
                },
                new Picture
                {
                    Product = products[16],
                    Src = "../../../assets/img/shorts/BSFN1.png",
                    Alt = "Sweatshorts image 1"
                },
                new Picture
                {
                    Product = products[17],
                    Src = "../../../assets/img/shorts/BSFN2.png",
                    Alt = "Sweatshorts image 2"
                },
                new Picture
                {
                    Product = products[18],
                    Src = "../../../assets/img/shorts/BSFN3.png",
                    Alt = "Basketballshorts image 3"
                },
                new Picture
                {
                    Product = products[19],
                    Src = "../../../assets/img/shorts/Suima1.png",
                    Alt = "Shorts image 1"
                },
                new Picture
                {
                    Product = products[20],
                    Src = "../../../assets/img/shorts/Suima2.png",
                    Alt = "Highwaist Shorts image 1"
                },
                new Picture
                {
                    Product = products[21],
                    Src = "../../../assets/img/shorts/Suima3.png",
                    Alt = "Shorts image 1"
                },
                new Picture
                {
                    Product = products[22],
                    Src = "../../../assets/img/jeans/FbFashion1.png",
                    Alt = "../../../assets/img/jeans/FbFashion1.png"
                },
                new Picture
                {
                    Product = products[23],
                    Src = "../../../assets/img/jeans/Suima1.png",
                    Alt = "Mom Fit Jeans image 2"
                },
                new Picture
                {
                    Product = products[24],
                    Src = "../../../assets/img/jeans/Suima2.png",
                    Alt = "Mom Fit Jeans image 3"
                },
                new Picture
                {
                    Product = products[25],
                    Src = "../../../assets/img/jeans/FbFashion2.png",
                    Alt = "Mom Fit Jeans image 4"
                },
                new Picture
                {
                    Product = products[26],
                    Src = "../../../assets/img/jeans/GOMS1.png",
                    Alt = "Stone Washed Relaxed Denim image 1"
                },
                new Picture
                {
                    Product = products[27],
                    Src = "../../../assets/img/jeans/GOMS2.png",
                    Alt = "Tapered Denim mit destroys image 1"
                },
                new Picture
                {
                    Product = products[28],
                    Src = "../../../assets/img/jeans/GOMS3.png",
                    Alt = "Washed Slim Fit Denim image 1"
                },
                new Picture
                {
                    Product = products[29],
                    Src = "../../../assets/img/jeans/GOMS4.png",
                    Alt = "Basic Slim Fit Denim image 1"
                },
                new Picture
                {
                    Product = products[30],
                    Src = "../../../assets/img/jackets/Suima1.png",
                    Alt = "Jacket with applications image 1"
                },
                new Picture
                {
                    Product = products[31],
                    Src = "../../../assets/img/jackets/FbFashion1.png",
                    Alt = "Bomber jacket image 1"
                },
                new Picture
                {
                    Product = products[32],
                    Src = "../../../assets/img/jackets/Suima2.png",
                    Alt = "Coat with hood image 1"
                },
                new Picture
                {
                    Product = products[33],
                    Src = "../../../assets/img/jackets/FbFashion2.png",
                    Alt = "Quilted jacket with hood image 1"
                },
                new Picture
                {
                    Product = products[34],
                    Src = "../../../assets/img/jackets/GOMS1.png",
                    Alt = "Jacket with material mix image 1"
                },
                new Picture
                {
                    Product = products[35],
                    Src = "../../../assets/img/jackets/GOMS2.png",
                    Alt = "Jacket with stand-up collar image 1"
                },
                new Picture
                {
                    Product = products[36],
                    Src = "../../../assets/img/jackets/GOMS3.png",
                    Alt = "Parka image 1"
                },
                new Picture
                {
                    Product = products[37],
                    Src = "../../../assets/img/jackets/GOMS4.png",
                    Alt = "Jacket with hood image 1"
                },
                new Picture
                {
                    Product = products[38],
                    Src = "../../../assets/img/shirts/FbFashion1.png",
                    Alt = "T-Shirt with print image 1"
                },
                new Picture
                {
                    Product = products[39],
                    Src = "../../../assets/img/shirts/FbFashion2.png",
                    Alt = "T-Shirt with print image 1"
                },
                new Picture
                {
                    Product = products[40],
                    Src = "../../../assets/img/shirts/Suima1.png",
                    Alt = "T-Shirt with print image 1"
                },
                new Picture
                {
                    Product = products[41],
                    Src = "../../../assets/img/shirts/BSFN1.png",
                    Alt = "T-shirt with round neck image 1"
                },
                new Picture
                {
                    Product = products[42],
                    Src = "../../../assets/img/shirts/BSFN2.png",
                    Alt = "T-shirt with layering image 1"
                },
                new Picture
                {
                    Product = products[43],
                    Src = "../../../assets/img/shirts/GOMS1.png",
                    Alt = "T-shirt with round neck image 1"
                },
                new Picture
                {
                    Product = products[44],
                    Src = "../../../assets/img/shirts/GOMS2.png",
                    Alt = "T-shirt with round neck image 1"
                },
                new Picture
                {
                    Product = products[3],
                    Src = "../../../assets/img/jumpers/FbFashion2.png",
                    Alt = "Oversize sweatshirt image 2"
                },
                new Picture
                {
                    Product = products[0],
                    Src = "../../../assets/img/jumpers/Suima1.png",
                    Alt = "Oversize sweatshirt image 1"
                }
            };

            var productSizes = new List<ProductSize>
            {
                new ProductSize
                {
                    Product = products[0],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[0],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[1],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[1],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[1],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[3],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[3],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[3],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[3],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[4],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[4],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[4],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[4],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[5],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[5],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[5],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[7],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[7],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[7],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[8],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[8],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[9],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[9],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[9],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[9],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[10],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[10],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[10],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[11],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[11],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[13],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[13],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[13],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[14],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[14],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[14],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[15],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[16],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[16],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[17],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[17],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[17],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[18],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[18],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[18],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[19],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[19],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[20],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[20],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[20],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[20],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[22],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[22],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[22],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[23],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[23],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[24],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[24],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[25],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[25],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[26],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[26],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[26],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[27],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[27],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[27],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[28],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[28],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[30],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[30],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[30],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[31],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[31],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[32],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[32],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[33],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[33],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[33],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[33],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[34],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[34],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[34],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[35],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[35],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[36],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[36],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[36],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[38],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[38],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[38],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[39],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[39],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[39],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[39],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[40],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[40],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[40],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[40],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[41],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[41],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[41],
                    Size = sizes[3],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[42],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[42],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[42],
                    Size = sizes[4],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[43],
                    Size = sizes[0],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[43],
                    Size = sizes[1],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[43],
                    Size = sizes[2],
                    Quantity = 20
                },
                new ProductSize
                {
                    Product = products[43],
                    Size = sizes[4],
                    Quantity = 20
                }
            };

            var prices = new List<Price>
            {
                new Price
                {
                    Product = products[0],
                    NewPrice = 12m,
                    ActiveFrom = DateTime.Parse("2026-09-15 21:00:11")
                },
                new Price
                {
                    Product = products[1],
                    NewPrice = 7m,
                    ActiveFrom = DateTime.Parse("2026-09-15 21:00:11")
                },
                new Price
                {
                    Product = products[2],
                    NewPrice = 7m,
                    ActiveFrom = DateTime.Parse("2026-09-15 18:15:42")
                },
                new Price
                {
                    Product = products[3],
                    NewPrice = 10m,
                    ActiveFrom = DateTime.Parse("2026-09-15 18:15:56")
                },
                new Price
                {
                    Product = products[4],
                    NewPrice = 15m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:10:57")
                },
                new Price
                {
                    Product = products[5],
                    NewPrice = 12m,
                    ActiveFrom = DateTime.Parse("2026-09-15 21:00:11")
                },
                new Price
                {
                    Product = products[6],
                    NewPrice = 9m,
                    ActiveFrom = DateTime.Parse("2026-09-15 21:00:11")
                },
                new Price
                {
                    Product = products[7],
                    NewPrice = 15m,
                    ActiveFrom = DateTime.Parse("2026-09-15 21:00:11")
                },
                new Price
                {
                    Product = products[8],
                    NewPrice = 20m,
                    ActiveFrom = DateTime.Parse("2026-09-15 21:00:25")
                },
                new Price
                {
                    Product = products[9],
                    NewPrice = 20m,
                    OldPrice = 25m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:11:43")
                },
                new Price
                {
                    Product = products[10],
                    NewPrice = 18m,
                    OldPrice = 20m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:11:53")
                },
                new Price
                {
                    Product = products[11],
                    NewPrice = 30m,
                    ActiveFrom = DateTime.Parse("2026-09-15 18:15:56")
                },
                new Price
                {
                    Product = products[12],
                    NewPrice = 30m,
                    OldPrice = 35m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:12:08")
                },
                new Price
                {
                    Product = products[13],
                    NewPrice = 30m,
                    OldPrice = 40m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:12:27")
                },
                new Price
                {
                    Product = products[14],
                    NewPrice = 28m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:12:33")
                },
                new Price
                {
                    Product = products[15],
                    NewPrice = 23m,
                    OldPrice = 25m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:12:40")
                },
                new Price
                {
                    Product = products[16],
                    NewPrice = 10m,
                    ActiveFrom = DateTime.Parse("2026-09-15 18:15:56")
                },
                new Price
                {
                    Product = products[17],
                    NewPrice = 12m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:12:51")
                },
                new Price
                {
                    Product = products[18],
                    NewPrice = 12m,
                    OldPrice = 25m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:13:01")
                },
                new Price
                {
                    Product = products[19],
                    NewPrice = 15m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:13:07")
                },
                new Price
                {
                    Product = products[20],
                    NewPrice = 9m,
                    OldPrice = 15m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:13:18")
                },
                new Price
                {
                    Product = products[21],
                    NewPrice = 10m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:13:23")
                },
                new Price
                {
                    Product = products[22],
                    NewPrice = 20m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:13:50")
                },
                new Price
                {
                    Product = products[23],
                    NewPrice = 18m,
                    OldPrice = 20m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:13:54")
                },
                new Price
                {
                    Product = products[24],
                    NewPrice = 25m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:14:49")
                },
                new Price
                {
                    Product = products[25],
                    NewPrice = 15m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:16:20")
                },
                new Price
                {
                    Product = products[26],
                    NewPrice = 20m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:16:26")
                },
                new Price
                {
                    Product = products[27],
                    NewPrice = 22m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:16:33")
                },
                new Price
                {
                    Product = products[28],
                    NewPrice = 28m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:16:40")
                },
                new Price
                {
                    Product = products[29],
                    NewPrice = 28m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:16:48")
                },
                new Price
                {
                    Product = products[30],
                    NewPrice = 50m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:16:55")
                },
                new Price
                {
                    Product = products[31],
                    NewPrice = 35m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:17:01")
                },
                new Price
                {
                    Product = products[32],
                    NewPrice = 45m,
                    OldPrice = 60m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:17:11")
                },
                new Price
                {
                    Product = products[33],
                    NewPrice = 55m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:17:17")
                },
                new Price
                {
                    Product = products[34],
                    NewPrice = 45m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:17:38")
                },
                new Price
                {
                    Product = products[35],
                    NewPrice = 40m,
                    OldPrice = 55m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:18:45")
                },
                new Price
                {
                    Product = products[36],
                    NewPrice = 70m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:18:52")
                },
                new Price
                {
                    Product = products[37],
                    NewPrice = 35m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:18:58")
                },
                new Price
                {
                    Product = products[38],
                    NewPrice = 10m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:19:04")
                },
                new Price
                {
                    Product = products[39],
                    NewPrice = 8m,
                    OldPrice = 10m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:19:10")
                },
                new Price
                {
                    Product = products[40],
                    NewPrice = 8m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:19:21")
                },
                new Price
                {
                    Product = products[41],
                    NewPrice = 15m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:19:28")
                },
                new Price
                {
                    Product = products[42],
                    NewPrice = 13m,
                    OldPrice = 10m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:19:35")
                },
                new Price
                {
                    Product = products[43],
                    NewPrice = 9m,
                    ActiveFrom = DateTime.Parse("2023-09-14 16:19:45")
                },
                new Price
                {
                    Product = products[44],
                    NewPrice = 20m,
                    OldPrice = 25m,
                    ActiveFrom = DateTime.Parse("2023-09-14 00:35:55")
                }
            };

            var useCases = new List<UseCase>
            {
                new UseCase
                {
                    Name = "Get categories",
                    Description = "Get categories with/without keyword."
                },
                new UseCase
                {
                    Name = "Create category",
                    Description = "Creating new category"
                },
                new UseCase
                {
                    Name = "Get users",
                    Description = "Get users with/without keyword."
                },
                new UseCase
                {
                    Name = "Create new user",
                    Description = "Create new user description"
                },
                new UseCase
                {
                    Name = "Get products",
                    Description = "Search and filter products."
                },
                new UseCase
                {
                    Name = "Update user info",
                    Description = "Update user information(Address, City, etc.)"
                },
                new UseCase
                {
                    Name = "Get single product",
                    Description = "Detailed product"
                },
                new UseCase
                {
                    Name = "Get single cart",
                    Description = "Get single cart details."
                },
                new UseCase
                {
                    Name = "Add to cart.",
                    Description = "Add product to cart."
                },
                new UseCase
                {
                    Name = "Update cart item.",
                    Description = "Update cart item details."
                },
                new UseCase
                {
                    Name = "Delete cart item",
                    Description = "Delete cart item with provided id"
                },
                new UseCase
                {
                    Name = "Create product.",
                    Description = "Create new product."
                },
                new UseCase
                {
                    Name = "Delete product",
                    Description = "Delete product with pictures, prices included."
                },
                new UseCase
                {
                    Name = "Get single receipt.",
                    Description = "Get single receipt informations."
                },
                new UseCase
                {
                    Name = "Confirm order",
                    Description = "Confirm order using user id"
                },
                new UseCase
                {
                    Name = "Search receipts",
                    Description = "Search receipts using dates and user id"
                },
                new UseCase
                {
                    Name = "Delete receipt",
                    Description = "Delete receipt with id provided."
                },
                new UseCase
                {
                    Name = "Fill database",
                    Description = "Initial fill database"
                },
                new UseCase
                {
                    Name = "Add product price",
                    Description = "Add new product price"
                },
                new UseCase
                {
                    Name = "Get single user",
                    Description = "Get single user with provided id"
                },
                new UseCase
                {
                    Name = "Update product",
                    Description = "Update existing product details and sizes."
                },
                new UseCase
                {
                    Name = "Delete user",
                    Description = "Ban user account (soft delete)."
                },
                new UseCase
                {
                    Name = "Forgot password",
                    Description = "Send password reset link by email."
                },
                new UseCase
                {
                    Name = "Reset password",
                    Description = "Set new password with reset token."
                },
                new UseCase
                {
                    Name = "Admin create user",
                    Description = "Create user and email link to set password."
                },
                new UseCase
                {
                    Name = "Get brands",
                    Description = "Get brands with/without keyword."
                },
                new UseCase
                {
                    Name = "Create brand",
                    Description = "Create new brand."
                },
                new UseCase
                {
                    Name = "Update brand",
                    Description = "Update brand name and logo."
                },
                new UseCase
                {
                    Name = "Delete brand",
                    Description = "Toggle brand active status."
                },
                new UseCase
                {
                    Name = "Update category",
                    Description = "Update category name and icon."
                },
                new UseCase
                {
                    Name = "Delete category",
                    Description = "Toggle category active status."
                },
                new UseCase
                {
                    Name = "Get admin stats",
                    Description = "Orders/revenue/category/brand breakdown."
                },
                new UseCase
                {
                    Name = "Get payment methods",
                    Description = "Get list of payment methods."
                },
                new UseCase
                {
                    Name = "Create contact message",
                    Description = "Send message from contact form."
                },
                new UseCase
                {
                    Name = "Delete contact message",
                    Description = "Toggle contact message active status."
                },
                new UseCase
                {
                    Name = "Get contact messages",
                    Description = "Get contact messages for admin."
                },
                new UseCase
                {
                    Name = "Answer contact message",
                    Description = "Reply to contact message."
                },
                new UseCase
                {
                    Name = "Subscribe to newsletter",
                    Description = "Subscribe email to newsletter."
                },
                new UseCase
                {
                    Name = "Get newsletter subscribers",
                    Description = "Get list of newsletter subscribers."
                },
                new UseCase
                {
                    Name = "Create coupon",
                    Description = "Create new discount coupon."
                },
                new UseCase
                {
                    Name = "Update coupon",
                    Description = "Update coupon discount and expiry date."
                },
                new UseCase
                {
                    Name = "Delete coupon",
                    Description = "Toggle coupon active status."
                },
                new UseCase
                {
                    Name = "Get coupons",
                    Description = "Get coupons with number of uses."
                },
                new UseCase
                {
                    Name = "Validate coupon",
                    Description = "Check if coupon can be used."
                },
                new UseCase
                {
                    Name = "Rate product",
                    Description = "Rate a product the user bought."
                },
                new UseCase
                {
                    Name = "Create promotion",
                    Description = "Create promotion with discount groups."
                },
                new UseCase
                {
                    Name = "Update promotion",
                    Description = "Update promotion name, dates and groups."
                },
                new UseCase
                {
                    Name = "Delete promotion",
                    Description = "Delete promotion and its scheduled prices."
                },
                new UseCase
                {
                    Name = "Get promotions",
                    Description = "Get promotions with groups and products."
                },
                new UseCase
                {
                    Name = "Get active promotions",
                    Description = "Get active promotions for home page."
                },
                new UseCase
                {
                    Name = "Get countries",
                    Description = "Get countries for dropdown."
                },
                new UseCase
                {
                    Name = "Create payment method",
                    Description = "Create new payment method."
                },
                new UseCase
                {
                    Name = "Update payment method",
                    Description = "Update payment method name."
                },
                new UseCase
                {
                    Name = "Delete payment method",
                    Description = "Toggle payment method active status."
                },
                new UseCase
                {
                    Name = "Get genders",
                    Description = "Get genders for shop filter."
                },
                new UseCase
                {
                    Name = "Get sizes",
                    Description = "Get sizes for shop filter."
                },
                new UseCase
                {
                    Name = "Get navigation",
                    Description = "Get navigation links."
                },
                new UseCase
                {
                    Name = "Get hero slides",
                    Description = "Get slides for home page hero."
                },
                new UseCase
                {
                    Name = "Create hero slide",
                    Description = "Create new hero slide."
                },
                new UseCase
                {
                    Name = "Update hero slide",
                    Description = "Update hero slide details."
                },
                new UseCase
                {
                    Name = "Delete hero slide",
                    Description = "Toggle hero slide active status."
                },
                new UseCase
                {
                    Name = "Update avatar",
                    Description = "Set or remove user avatar."
                },
                new UseCase
                {
                    Name = "Get audit logs",
                    Description = "Get audit log entries."
                },
                new UseCase
                {
                    Name = "Toggle promotion",
                    Description = "Toggle promotion active status."
                }
            };

            var paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { Name = "Paypal" },
                new PaymentMethod { Name = "Direct Check" },
                new PaymentMethod { Name = "Bank Transfer" }
            };

            var coupons = new List<Coupon>
            {
                new Coupon
                {
                    Code = "WELCOME20",
                    DiscountPercentage = 20,
                    ValidUntil = DateTime.Parse("2026-12-31 00:00:00")
                },
                new Coupon
                {
                    Code = "SAVE10",
                    DiscountPercentage = 10,
                    ValidUntil = DateTime.Parse("2026-12-31 00:00:00")
                }
            };

            var userUseCase = new List<UserUseCase>
            {
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[0]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[1]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[2]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[3]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[4]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[5]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[6]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[11]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[12]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[13]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[15]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[16]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[17]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[18]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[19]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[20]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[21]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[24]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[25]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[26]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[27]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[28]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[29]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[30]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[31]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[32]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[33]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[34]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[35]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[36]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[37]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[38]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[39]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[40]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[41]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[42]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[45]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[46]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[47]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[48]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[49]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[50]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[51]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[52]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[53]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[54]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[55]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[56]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[57]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[58]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[59]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[60]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[61]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[62]
                },
                new UserUseCase
                {
                    User = users[0],
                    UseCase = useCases[63]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[0]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[4]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[5]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[6]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[7]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[8]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[9]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[10]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[13]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[14]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[15]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[19]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[25]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[32]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[33]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[37]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[43]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[44]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[49]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[54]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[55]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[56]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[57]
                },
                new UserUseCase
                {
                    User = users[1],
                    UseCase = useCases[61]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[0]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[4]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[6]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[17]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[25]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[32]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[33]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[37]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[43]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[44]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[49]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[54]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[55]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[56]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[57]
                },
                new UserUseCase
                {
                    User = users[2],
                    UseCase = useCases[61]
                }
            };

            Context.Brands.AddRange(brands);
            Context.Categories.AddRange(categories);
            Context.PaymentMethods.AddRange(paymentMethods);
            Context.Countries.AddRange(countries);
            Context.NavigationItems.AddRange(navigationItems);
            Context.HeroSlides.AddRange(heroSlides);
            Context.Genders.AddRange(gender);
            Context.Sizes.AddRange(sizes);
            Context.Carts.AddRange(carts);
            Context.Products.AddRange(products);
            Context.Infos.AddRange(infos);
            Context.Users.AddRange(users);
            Context.Pictures.AddRange(pictures);
            Context.ProductSizes.AddRange(productSizes);
            Context.Prices.AddRange(prices);
            Context.Coupons.AddRange(coupons);
            Context.UseCases.AddRange(useCases);
            Context.UserUseCases.AddRange(userUseCase);

            Context.SaveChanges();
        }
    }
}
