using EShopper.DataAccess;

namespace EShopper.Implementation.UseCases
{
    public abstract class UseCaseEf
    {
        protected EShopperDbContext Context { get; set; }

        protected UseCaseEf(EShopperDbContext context)
        {
            Context = context;
        }
    }
}
