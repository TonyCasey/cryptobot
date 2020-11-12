using Api.CryptoBot.Data;
using Api.CryptoBot.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO.RuleSet;
using Api.CryptoBot.Models.Extensions;
using CryptoBot.Model.Domain;

namespace Api.CryptoBot.Controllers
{
    /// <summary>
    /// RuleSet controller
    ///
    ///</summary>
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]")]
    public class RuleSetController : BaseController< RuleSetRequestDto, RuleSetResponseDto, RuleSetSearchRequestDto, RuleSetSearchResponseDto >
    {

        private readonly IMapper _mapper;
        private readonly CryptoBotApiDbContext _dbContext;

        /// <summary>
        /// Main constructor to inject mapper and IService
        ///
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public RuleSetController (IMapper mapper,  CryptoBotApiDbContext dbContext )
        :base(mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        // GET: api/RuleSet/5
        /// <summary>
        /// returns a specific RuleSet with the specified id
        ///
        ///
        /// </summary>
        /// <param name="rulesetId">id of RuleSet to return</param>
        /// <returns>a single RuleSet</returns>
        /// <response code="200">RuleSet found - body contains data</response>
        /// <response code="404">RuleSet does not exist</response>
        [Microsoft.AspNetCore.Mvc.HttpGet("{rulesetId}")]
        public override async Task<RuleSetResponseDto> Get(long rulesetId)
        {
            return _mapper.Map(_dbContext.RuleSets.FirstOrDefault(x => x.RuleSetId == rulesetId), new RuleSetResponseDto());
        }

        // POST: api/RuleSet
        /// <summary>
        /// adds a RuleSet type to the system
        ///
        ///
        /// </summary>
        /// <param name="value">new RuleSet DTO to add</param>
        /// <response code="201">RuleSet created</response>
        /// <response code="400">Request is not valid</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(Uri))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(void))]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public override async Task<CreatedResult> Post([Microsoft.AspNetCore.Mvc.FromBody]RuleSetRequestDto requestDto)
        {

            var id =1; // create record

            return Created($"/api/RuleSet/{id}", id );

        }

        // PUT: api/RuleSet/5
        /// <summary>
        /// modifies the existing RuleSet
        ///
        ///
        /// </summary>
        /// <param name="rulesetId">id of RuleSet</param>
        /// <param name="value">DTO of RuleSet to change</param>
        /// <returns></returns>
        /// <response code="204">resource modified</response>
        /// <response code="403">not allowed to modify the resource</response>
        /// <response code="404">resource does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [Microsoft.AspNetCore.Mvc.HttpPut("{rulesetId}")]
        public override async Task< RuleSetResponseDto > Put(long rulesetId, [Microsoft.AspNetCore.Mvc.FromBody]RuleSetRequestDto requestDto)
        {
            return null;

        }

        // DELETE: api/RuleSet/5
        /// <summary>
        /// removes a RuleSet from the system
        ///
        ///
        /// </summary>
        /// <param name="Id">id of RuleSet to remove</param>
        /// <response code="204">RuleSet removed</response>
        /// <response code="403">not allowed to remove RuleSet</response>
        /// <response code="404">RuleSet does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpDelete]
        public async Task<bool> Delete(int rulesetId)
        {
            return true;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("Search")]
        public override async Task< RuleSetSearchResponseDto > Search(RuleSetSearchRequestDto searchRequest)
        {
            RuleSetSearchResponseDto response = new RuleSetSearchResponseDto
            {
                Data = _mapper.Map(_dbContext.RuleSets.FilterByIndicatorId(searchRequest.IndicatorId), new List<RuleSetResponseDto>())
            };
               
            return response;
        }



    }

           
}
