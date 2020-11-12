using Api.CryptoBot.Data;
using Api.CryptoBot.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO.Indicator;
using Api.CryptoBot.Models.Extensions;
using CryptoBot.Model.Domain;

namespace Api.CryptoBot.Controllers
{
    /// <summary>
    /// Indicator controller
    ///
    ///</summary>
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]")]
    public class IndicatorController : BaseController< IndicatorRequestDto, IndicatorResponseDto, IndicatorSearchRequestDto, IndicatorSearchResponseDto >
    {

        private readonly IMapper _mapper;
        private readonly CryptoBotApiDbContext _dbContext;

        /// <summary>
        /// Main constructor to inject mapper and IService
        ///
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public IndicatorController (IMapper mapper,  CryptoBotApiDbContext dbContext )
        :base(mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        // GET: api/Indicator/5
        /// <summary>
        /// returns a specific Indicator with the specified id
        ///
        ///
        /// </summary>
        /// <param name="indicatorId">id of Indicator to return</param>
        /// <returns>a single Indicator</returns>
        /// <response code="200">Indicator found - body contains data</response>
        /// <response code="404">Indicator does not exist</response>
        [Microsoft.AspNetCore.Mvc.HttpGet("{indicatorId}")]
        public override async Task<IndicatorResponseDto> Get(long indicatorId)
        {
            return _mapper.Map(_dbContext.Indicators.FirstOrDefault(x => x.IndicatorId == indicatorId ), new IndicatorResponseDto()) ;
        }

        // POST: api/Indicator
        /// <summary>
        /// adds a Indicator type to the system
        ///
        ///
        /// </summary>
        /// <param name="value">new Indicator DTO to add</param>
        /// <response code="201">Indicator created</response>
        /// <response code="400">Request is not valid</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(Uri))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(void))]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public override async Task<CreatedResult> Post([Microsoft.AspNetCore.Mvc.FromBody]IndicatorRequestDto requestDto)
        {

            var id =1; // create record

            return Created($"/api/Indicator/{id}", id );

        }

        // PUT: api/Indicator/5
        /// <summary>
        /// modifies the existing Indicator
        ///
        ///
        /// </summary>
        /// <param name="indicatorId">id of Indicator</param>
        /// <param name="value">DTO of Indicator to change</param>
        /// <returns></returns>
        /// <response code="204">resource modified</response>
        /// <response code="403">not allowed to modify the resource</response>
        /// <response code="404">resource does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [Microsoft.AspNetCore.Mvc.HttpPut("{indicatorId}")]
        public override async Task<IndicatorResponseDto> Put(long indicatorId, [Microsoft.AspNetCore.Mvc.FromBody]IndicatorRequestDto requestDto)
        {
            return null;

        }

        // DELETE: api/Indicator/5
        /// <summary>
        /// removes a Indicator from the system
        ///
        ///
        /// </summary>
        /// <param name="indicatorId">id of Indicator to remove</param>
        /// <response code="204">Indicator removed</response>
        /// <response code="403">not allowed to remove Indicator</response>
        /// <response code="404">Indicator does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpDelete]
        public async Task<bool> Delete(int indicatorId)
        {
            return true;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("Search")]
        public override async Task<IndicatorSearchResponseDto> Search(IndicatorSearchRequestDto searchRequest)
        {
            IndicatorSearchResponseDto response = new IndicatorSearchResponseDto
            {
                Data = _mapper.Map(_dbContext
                    .Indicators
                    .FilterByBotId(searchRequest.BotId)
                    .FilterByType(searchRequest.IndicatorType)
                    , new List<IndicatorResponseDto>())
            };
            return response;
        }



    }

            
}
