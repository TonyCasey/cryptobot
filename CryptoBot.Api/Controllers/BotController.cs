using Api.CryptoBot.Data;
using Api.CryptoBot.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO.Bot;
using CryptoBot.Model.Domain;
using Asp.Versioning;

namespace Api.CryptoBot.Controllers
{
    /// <summary>
    /// Bot controller
    ///
    ///</summary>
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]")]
    public class BotController : BaseController< BotRequestDto, BotResponseDto, BotSearchRequestDto, BotSearchResponseDto >
    {

        private new readonly IMapper _mapper;
        private readonly CryptoBotApiDbContext _dbContext;

        /// <summary>
        /// Main constructor to inject mapper and IService
        ///
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public BotController (IMapper mapper,  CryptoBotApiDbContext dbContext )
        :base(mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }



        // GET: api/Bot/5
        /// <summary>
        /// returns a specific Bot with the specified id
        ///
        ///
        /// </summary>
        /// <param name="id">id of Bot to return</param>
        /// <returns>a single Bot</returns>
        /// <response code="200">Bot found - body contains data</response>
        /// <response code="404">Bot does not exist</response>
        [Microsoft.AspNetCore.Mvc.HttpGet("{botId}")]
        public override Task<BotResponseDto> Get(long botId)
        {
            var result = _dbContext.Bots.FirstOrDefault(x => x.BotId == botId);
            return Task.FromResult(_mapper.Map(result, new BotResponseDto()));
        }

        
        // POST: api/Bot
        /// <summary>
        /// adds a Bot type to the system
        ///
        ///
        /// </summary>
        /// <param name="value">new Bot DTO to add</param>
        /// <response code="201">Bot created</response>
        /// <response code="400">Request is not valid</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(Uri))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(void))]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public override Task<CreatedResult> Post([Microsoft.AspNetCore.Mvc.FromBody]BotRequestDto requestDto)
        {

            var id =1; // create record

            return Task.FromResult(Created($"/api/Bot/{id}", id ));

        }

        // PUT: api/Bot/5
        /// <summary>
        /// modifies the existing Bot
        ///
        ///
        /// </summary>
        /// <param name="id">id of Bot</param>
        /// <param name="value">DTO of Bot to change</param>
        /// <returns></returns>
        /// <response code="204">resource modified</response>
        /// <response code="403">not allowed to modify the resource</response>
        /// <response code="404">resource does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [Microsoft.AspNetCore.Mvc.HttpPut("{id}")]
        public override Task< BotResponseDto > Put(long id, [Microsoft.AspNetCore.Mvc.FromBody]BotRequestDto requestDto)
        {
            return Task.FromResult<BotResponseDto>(null);

        }

        // DELETE: api/Bot/5
        /// <summary>
        /// removes a Bot from the system
        ///
        ///
        /// </summary>
        /// <param name="id">id of Bot to remove</param>
        /// <response code="204">Bot removed</response>
        /// <response code="403">not allowed to remove Bot</response>
        /// <response code="404">Bot does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpDelete]
        public Task<bool> Delete(int id)
        {
            return Task.FromResult(true);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("Search")]
        public override Task< BotSearchResponseDto > Search(BotSearchRequestDto searchRequest)
        {
            // TODO: lots to do here on the search, just return all for now
            return Task.FromResult(new BotSearchResponseDto
            {
                Data = _mapper.Map(_dbContext.Bots, new List<BotResponseDto>())
            });
        }


    }
            
}
