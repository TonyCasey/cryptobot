using Api.CryptoBot.Data;
using Api.CryptoBot.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO.Exchange;
using Api.CryptoBot.Models.Extensions;
using CryptoBot.Model.Domain;
using Asp.Versioning;

namespace Api.CryptoBot.Controllers
{
    /// <summary>
    /// Exchange controller
    ///
    ///</summary>
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]")]
    public class ExchangeController : BaseController< ExchangeRequestDto, ExchangeResponseDto, ExchangeSearchRequestDto, ExchangeSearchResponseDto >
    {

        private readonly IMapper _mapper;
        private readonly CryptoBotApiDbContext _dbContext;

        /// <summary>
        /// Main constructor to inject mapper and IService
        ///
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public ExchangeController (IMapper mapper,  CryptoBotApiDbContext dbContext )
        :base(mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        // GET: api/Exchange/5
        /// <summary>
        /// returns a specific Exchange with the specified id
        ///
        ///
        /// </summary>
        /// <param name="exchangeId">id of Exchange to return</param>
        /// <returns>a single Exchange</returns>
        /// <response code="200">Exchange found - body contains data</response>
        /// <response code="404">Exchange does not exist</response>
        [Microsoft.AspNetCore.Mvc.HttpGet("{exchangeId}")]
        public override async Task<ExchangeResponseDto> Get(long exchangeId)
        {
            return _mapper.Map(_dbContext.Exchanges.FirstOrDefault(x=>x.ExchangeId == exchangeId), new ExchangeResponseDto());
        }

        // POST: api/Exchange
        /// <summary>
        /// adds a Exchange type to the system
        ///
        ///
        /// </summary>
        /// <param name="value">new Exchange DTO to add</param>
        /// <response code="201">Exchange created</response>
        /// <response code="400">Request is not valid</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(Uri))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(void))]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public override async Task<CreatedResult> Post([Microsoft.AspNetCore.Mvc.FromBody]ExchangeRequestDto requestDto)
        {

            var id =1; // create record

            return Created($"/api/Exchange/{id}", id );

        }

        // PUT: api/Exchange/5
        /// <summary>
        /// modifies the existing Exchange
        ///
        ///
        /// </summary>
        /// <param name="exchangeId">id of Exchange</param>
        /// <param name="value">DTO of Exchange to change</param>
        /// <returns></returns>
        /// <response code="204">resource modified</response>
        /// <response code="403">not allowed to modify the resource</response>
        /// <response code="404">resource does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [Microsoft.AspNetCore.Mvc.HttpPut("{exchangeId}")]
        public override async Task< ExchangeResponseDto > Put(long exchangeId, [Microsoft.AspNetCore.Mvc.FromBody]ExchangeRequestDto requestDto)
        {
            return null;

        }

        // DELETE: api/Exchange/5
        /// <summary>
        /// removes a Exchange from the system
        ///
        ///
        /// </summary>
        /// <param name="Id">id of Exchange to remove</param>
        /// <response code="204">Exchange removed</response>
        /// <response code="403">not allowed to remove Exchange</response>
        /// <response code="404">Exchange does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpDelete]
        public async Task<bool> Delete(int exchangeId)
        {
            return true;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("Search")]
        public override async Task<ExchangeSearchResponseDto> Search(ExchangeSearchRequestDto searchRequest)
        {
            ExchangeSearchResponseDto response = new ExchangeSearchResponseDto
            {
                Data = _mapper.Map(_dbContext
                    .Exchanges
                    .FilterByCode(searchRequest.Code)
                    .FilterByName(searchRequest.Name),
                    new List<ExchangeResponseDto>()
                )
            };
            return response;
        }



    }

            
            
}
