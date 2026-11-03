using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Application.UseCases.Queries;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public CountriesController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Returns the list of countries, for the country-of-origin dropdown.
        /// </summary>
        /// <response code="200">Returns countries</response>

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromServices] IGetCountriesQuery query)
        {
            return Ok(_handler.HandleQuery(query, 0));
        }
    }
}
