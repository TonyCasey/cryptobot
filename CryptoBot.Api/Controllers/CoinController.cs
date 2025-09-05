using Api.CryptoBot.Data;
using Api.CryptoBot.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO.Coin;
using Api.CryptoBot.Models.Extensions;
using CryptoBot.Model.Domain;
using Asp.Versioning;

namespace Api.CryptoBot.Controllers
{
    /// <summary>
    /// Coin controller
    ///
    ///</summary>
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]")]
    public class CoinController : BaseController< CoinRequestDto, CoinResponseDto, CoinSearchRequestDto, CoinSearchResponseDto >
    {

        private readonly IMapper _mapper;
        private readonly CryptoBotApiDbContext _dbContext;

        /// <summary>
        /// Main constructor to inject mapper and IService
        ///
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public CoinController (IMapper mapper,  CryptoBotApiDbContext dbContext )
        :base(mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        // GET: api/Coin/5
        /// <summary>
        /// returns a specific Coin with the specified id
        ///
        ///
        /// </summary>
        /// <param name="Id">id of Coin to return</param>
        /// <returns>a single Coin</returns>
        /// <response code="200">Coin found - body contains data</response>
        /// <response code="404">Coin does not exist</response>
        [Microsoft.AspNetCore.Mvc.HttpGet("{coinId}")]
        public override async Task< CoinResponseDto > Get(long coinId)
        {
            return _mapper.Map(_dbContext.Coins.FirstOrDefault(x=>x.CoinId == coinId), new CoinResponseDto());
        }

        // POST: api/Coin
        /// <summary>
        /// adds a Coin type to the system
        ///
        ///
        /// </summary>
        /// <param name="value">new Coin DTO to add</param>
        /// <response code="201">Coin created</response>
        /// <response code="400">Request is not valid</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(Uri))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(void))]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public override async Task<CreatedResult> Post([Microsoft.AspNetCore.Mvc.FromBody]CoinRequestDto requestDto)
        {

            var id =1; // create record

            return Created($"/api/Coin/{id}", id );

        }

        // PUT: api/Coin/5
        /// <summary>
        /// modifies the existing Coin
        ///
        ///
        /// </summary>
        /// <param name="Id">id of Coin</param>
        /// <param name="value">DTO of Coin to change</param>
        /// <returns></returns>
        /// <response code="204">resource modified</response>
        /// <response code="403">not allowed to modify the resource</response>
        /// <response code="404">resource does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [Microsoft.AspNetCore.Mvc.HttpPut("{coinId}")]
        public override async Task< CoinResponseDto > Put(long coinId, [Microsoft.AspNetCore.Mvc.FromBody]CoinRequestDto requestDto)
        {
            return null;

        }

        // DELETE: api/Coin/5
        /// <summary>
        /// removes a Coin from the system
        ///
        ///
        /// </summary>
        /// <param name="Id">id of Coin to remove</param>
        /// <response code="204">Coin removed</response>
        /// <response code="403">not allowed to remove Coin</response>
        /// <response code="404">Coin does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpDelete]
        public async Task<bool> Delete(int coinId)
        {
            return true;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("Search")]
        public override async Task<CoinSearchResponseDto> Search(CoinSearchRequestDto searchRequest)
        {
            CoinSearchResponseDto result = new CoinSearchResponseDto
            {
                Data =_mapper.Map(_dbContext
                    .Coins
                    .FilterByCode(searchRequest.Code),
                    new List<CoinResponseDto>())
            };
            return result;
        }



    }
            
}
