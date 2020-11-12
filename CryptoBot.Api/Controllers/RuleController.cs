using Api.CryptoBot.Data;
using Api.CryptoBot.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO.Rule;
using Api.CryptoBot.Models.Extensions;
using CryptoBot.Model.Domain;

namespace Api.CryptoBot.Controllers
{
    /// <summary>
    /// Rule controller
    ///
    ///</summary>
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]")]
    public class RuleController : BaseController< RuleRequestDto, RuleResponseDto, RuleSearchRequestDto, RuleSearchResponseDto >
    {

        private readonly IMapper _mapper;
        private readonly CryptoBotApiDbContext _dbContext;

        /// <summary>
        /// Main constructor to inject mapper and IService
        ///
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public RuleController (IMapper mapper,  CryptoBotApiDbContext dbContext )
        :base(mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        // GET: api/Rule/5
        /// <summary>
        /// returns a specific Rule with the specified id
        ///
        ///
        /// </summary>
        /// <param name="ruleId">id of Rule to return</param>
        /// <returns>a single Rule</returns>
        /// <response code="200">Rule found - body contains data</response>
        /// <response code="404">Rule does not exist</response>
        [Microsoft.AspNetCore.Mvc.HttpGet("{ruleId}")]
        public override async Task< RuleResponseDto > Get(long ruleId)
        {
            return _mapper.Map(_dbContext.Rules.FirstOrDefault(x=>x.RuleId == ruleId), new RuleResponseDto());
        }

        // POST: api/Rule
        /// <summary>
        /// adds a Rule type to the system
        ///
        ///
        /// </summary>
        /// <param name="value">new Rule DTO to add</param>
        /// <response code="201">Rule created</response>
        /// <response code="400">Request is not valid</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(Uri))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(void))]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public override async Task<CreatedResult> Post([Microsoft.AspNetCore.Mvc.FromBody]RuleRequestDto requestDto)
        {

            var id =1; // create record

            return Created($"/api/Rule/{id}", id );

        }

        // PUT: api/Rule/5
        /// <summary>
        /// modifies the existing Rule
        ///
        ///
        /// </summary>
        /// <param name="ruleId">id of Rule</param>
        /// <param name="value">DTO of Rule to change</param>
        /// <returns></returns>
        /// <response code="204">resource modified</response>
        /// <response code="403">not allowed to modify the resource</response>
        /// <response code="404">resource does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [Microsoft.AspNetCore.Mvc.HttpPut("{ruleId}")]
        public override async Task< RuleResponseDto > Put(long ruleId, [Microsoft.AspNetCore.Mvc.FromBody]RuleRequestDto requestDto)
        {
            return null;

        }

        // DELETE: api/Rule/5
        /// <summary>
        /// removes a Rule from the system
        ///
        ///
        /// </summary>
        /// <param name="ruleId">id of Rule to remove</param>
        /// <response code="204">Rule removed</response>
        /// <response code="403">not allowed to remove Rule</response>
        /// <response code="404">Rule does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpDelete]
        public async Task<bool> Delete(int ruleId)
        {
            return true;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("Search")]
        public override async Task<RuleSearchResponseDto> Search(RuleSearchRequestDto searchRequest)
        {
            RuleSearchResponseDto result = new RuleSearchResponseDto
            {
                Data = _mapper.Map(_dbContext.Rules.FilterByIndicatorRuleTypeId(searchRequest.IndicatorRuleTypeId), new List<RuleResponseDto>())
            };

            return result;
        }



    }
    
            
}
