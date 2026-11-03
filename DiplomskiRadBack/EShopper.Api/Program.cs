using Microsoft.OpenApi.Models;
using EShopper.Api.Core;
using EShopper.Api.Extensions;
using EShopper.Application.Email;
using EShopper.Application.Loggers;
using EShopper.Application.Storage;
using EShopper.Application.UseCases;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.Queries;
using EShopper.Implementation;
using EShopper.Implementation.Email;
using EShopper.Implementation.Loggers;
using EShopper.Implementation.Storage;
using EShopper.Implementation.UseCases.Commands;
using EShopper.Implementation.UseCases.Queries;
using EShopper.Implementation.Validations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var settings = new AppSettings();

builder.Configuration.Bind(settings);
builder.Services.AddSingleton(settings);
builder.Services.AddApplicationUser();
builder.Services.AddJwt(settings);
builder.Services.AddEShopperDbContext();

#region Use Cases

builder.Services.AddTransient<AssetCleaner>();

builder.Services.AddTransient<IUpdateAvatarCommand, UpdateAvatarCommandEf>();

builder.Services.AddTransient<IGetBrandQuery, GetBrandQueryEf>();
builder.Services.AddTransient<ICreateBrandCommand, CreateBrandCommandEf>();
builder.Services.AddTransient<IUpdateBrandCommand, UpdateBrandCommandEf>();
builder.Services.AddTransient<IDeleteBrandCommand, DeleteBrandCommandEf>();

builder.Services.AddTransient<IGetCartQuery, GetSingleCartQueryEf>();

builder.Services.AddTransient<ICreateCartItemCommand, CreateCartItemCommandEf>();
builder.Services.AddTransient<IUpdateCartItemCommand, UpdateCartItemCommandEf>();
builder.Services.AddTransient<IDeleteCartItemCommand, DeleteCartItemCommandEf>();

builder.Services.AddTransient<IGetCategoryQuery, GetCategoryQueryEf>();
builder.Services.AddTransient<ICreateCategoryCommand, CreateCategoryCommandEf>();
builder.Services.AddTransient<IUpdateCategoryCommand, UpdateCategoryCommandEf>();
builder.Services.AddTransient<IDeleteCategoryCommand, DeleteCategoryCommandEf>();

builder.Services.AddTransient<IGetCountriesQuery, GetCountriesQueryEf>();

builder.Services.AddTransient<IFillDatabaseCommand, FillDatabaseCommandEf>();

builder.Services.AddTransient<IGetGenderQuery, GetGenderQueryEf>();

builder.Services.AddTransient<IGetHeroSlidesQuery, GetHeroSlidesQueryEf>();
builder.Services.AddTransient<ICreateHeroSlideCommand, CreateHeroSlideCommandEf>();
builder.Services.AddTransient<IUpdateHeroSlideCommand, UpdateHeroSlideCommandEf>();
builder.Services.AddTransient<IDeleteHeroSlideCommand, DeleteHeroSlideCommandEf>();

builder.Services.AddTransient<IGetNavigationQuery, GetNavigationQueryEf>();

builder.Services.AddTransient<IConfirmOrderCommand, ConfirmOrderCommandEf>();

builder.Services.AddTransient<IForgotPasswordCommand, ForgotPasswordCommandEf>();
builder.Services.AddTransient<IResetPasswordCommand, ResetPasswordCommandEf>();

builder.Services.AddTransient<IGetPaymentMethodsQuery, GetPaymentMethodsQueryEf>();
builder.Services.AddTransient<ICreatePaymentMethodCommand, CreatePaymentMethodCommandEf>();
builder.Services.AddTransient<IUpdatePaymentMethodCommand, UpdatePaymentMethodCommandEf>();
builder.Services.AddTransient<IDeletePaymentMethodCommand, DeletePaymentMethodCommandEf>();

builder.Services.AddTransient<IGetProductQuery, GetProductsQueryEf>();
builder.Services.AddTransient<IGetSingleProductQuery, GetSingleProductQueryEf>();
builder.Services.AddTransient<ICreateProductCommand, CreateProductCommandEf>();
builder.Services.AddTransient<IDeleteProductCommand, DeleteProductCommandEf>();
builder.Services.AddTransient<IUpdateProductCommand, UpdateProductCommandEf>();

builder.Services.AddTransient<IAddProductPriceCommand, AddProductPriceCommandEf>();

builder.Services.AddTransient<IGetSingleReceiptQuery, GetSingleReceiptQueryEf>();
builder.Services.AddTransient<IGetReceiptsQuery, GetReceiptsQueryEf>();
builder.Services.AddTransient<IDeleteReceiptCommand, DeleteReceiptCommandEf>();

builder.Services.AddTransient<IGetSizeQuery, GetSizeQueryEf>();

builder.Services.AddTransient<IGetAdminStatsQuery, GetAdminStatsQueryEf>();

builder.Services.AddTransient<IUseCaseLogger, UseCaseLogger>();

builder.Services.AddTransient<ICreateUserCommand, CreateUserCommandEf>();
builder.Services.AddTransient<IGetUserQuery, GetUserQueryEf>();
builder.Services.AddTransient<IGetSingleUserQuery, GetSingleUserQueryEf>();
builder.Services.AddTransient<IDeleteUserCommand, DeleteUserCommandEf>();
builder.Services.AddTransient<IAdminCreateUserQuery, AdminCreateUserQueryEf>();

builder.Services.AddTransient<IUpdateUserInfoCommand, UpdateUserCommandEf>();
builder.Services.AddSingleton<IAssetStorage>(new AssetStorage(AssetPaths.Root(builder.Environment.ContentRootPath)));
builder.Services.AddTransient<IGetAuditLogsQuery, GetAuditLogsQueryEf>();

builder.Services.AddTransient<ICreateContactMessageCommand, CreateContactMessageCommandEf>();
builder.Services.AddTransient<IDeleteContactMessageCommand, DeleteContactMessageCommandEf>();
builder.Services.AddTransient<IGetContactMessagesQuery, GetContactMessagesQueryEf>();
builder.Services.AddTransient<IAnswerContactMessageCommand, AnswerContactMessageCommandEf>();

builder.Services.AddTransient<ICreateCouponCommand, CreateCouponCommandEf>();
builder.Services.AddTransient<IUpdateCouponCommand, UpdateCouponCommandEf>();
builder.Services.AddTransient<IDeleteCouponCommand, DeleteCouponCommandEf>();
builder.Services.AddTransient<IGetCouponsQuery, GetCouponsQueryEf>();
builder.Services.AddTransient<IValidateCouponQuery, ValidateCouponQueryEf>();

builder.Services.AddTransient<ISubscribeNewsletterCommand, SubscribeNewsletterCommandEf>();

builder.Services.AddTransient<IGetNewsletterSubscribersQuery, GetNewsletterSubscribersQueryEf>();

builder.Services.AddTransient<IRateProductCommand, RateProductCommandEf>();

builder.Services.AddTransient<ICreatePromotionCommand, CreatePromotionCommandEf>();
builder.Services.AddTransient<IUpdatePromotionCommand, UpdatePromotionCommandEf>();
builder.Services.AddTransient<IDeletePromotionCommand, DeletePromotionCommandEf>();
builder.Services.AddTransient<ITogglePromotionCommand, TogglePromotionCommandEf>();
builder.Services.AddHostedService<PromotionExpiryService>();
builder.Services.AddTransient<IGetActivePromotionsQuery, GetActivePromotionsQueryEf>();

builder.Services.AddTransient<IGetPromotionsQuery, GetPromotionsQueryEf>();

#endregion

builder.Services.AddTransient<IEmail, SimulatedEmailSender>();

#region Validators

builder.Services.AddTransient<CreateBrandValidator>();
builder.Services.AddTransient<UpdateBrandValidator>();

builder.Services.AddTransient<CreateCartItemValidator>();
builder.Services.AddTransient<UpdateCartItemValidator>();

builder.Services.AddTransient<CreateCategoryValidator>();
builder.Services.AddTransient<UpdateCategoryValidator>();

builder.Services.AddTransient<CreateContactMessageValidator>();

builder.Services.AddTransient<ResetPasswordValidator>();

builder.Services.AddTransient<PictureDtoValidator>();

builder.Services.AddTransient<AddPriceValidator>();

builder.Services.AddTransient<CreateProductValidator>();
builder.Services.AddTransient<UpdateProductValidator>();

builder.Services.AddTransient<CreateUserValidator>();
builder.Services.AddTransient<AdminCreateUserValidator>();

builder.Services.AddTransient<UpdateUserInfoValidator>();

#endregion

builder.Services.AddTransient<IExceptionLogger, ConsoleLogger>();

builder.Services.AddTransient<UseCaseHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors();

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Token with Bearer format like Bearer[space] token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
        new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[]{ }
        }
    });

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Web shop API",
        Version = "v1",
        Description = "An API to perform shopping operations",
        Contact = new OpenApiContact
        {
            Name = "Nenad Jevtic",
            Email = "nenad.jevtic.60.20@ict.edu.rs",
        },
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandler>();
app.UseCors(builder => builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
