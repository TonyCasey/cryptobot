using Api.CryptoBot.Data;
using Api.CryptoBot.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO.Position;
using Api.CryptoBot.Models.Extensions;
using CryptoBot.Model.Domain;

namespace Api.CryptoBot.Controllers
{
    /// <summary>
    /// Position controller
    ///
    ///</summary>
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]")]
    public class PositionController : BaseController< PositionRequestDto, PositionResponseDto, PositionSearchRequestDto, PositionSearchResponseDto >
    {

        private readonly IMapper _mapper;
        private readonly CryptoBotApiDbContext _dbContext;

        /// <summary>
        /// Main constructor to inject mapper and IService
        ///
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public PositionController (IMapper mapper,  CryptoBotApiDbContext dbContext )
        :base(mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        // GET: api/Position/5
        /// <summary>
        /// returns a specific Position with the specified id
        ///
        ///
        /// </summary>
        /// <param name="Id">id of Position to return</param>
        /// <returns>a single Position</returns>
        /// <response code="200">Position found - body contains data</response>
        /// <response code="404">Position does not exist</response>
        [Microsoft.AspNetCore.Mvc.HttpGet("{positionId}")]
        public override async Task<PositionResponseDto> Get(long positionId)
        {
            var record = _dbContext.Positions.FirstOrDefault(x => x.PositionId == positionId);

            return record == null ? new PositionResponseDto() : _mapper.Map(record, new PositionResponseDto());
            
        }

        // POST: api/Position
        /// <summary>
        /// adds a Position type to the system
        ///
        ///
        /// </summary>
        /// <param name="value">new Position DTO to add</param>
        /// <response code="201">Position created</response>
        /// <response code="400">Request is not valid</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(Uri))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(void))]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public override async Task<CreatedResult> Post([Microsoft.AspNetCore.Mvc.FromBody]PositionRequestDto requestDto)
        {

            var id =1; // create record

            return Created($"/api/Position/{id}", id );

        }

        // PUT: api/Position/5
        /// <summary>
        /// modifies the existing Position
        ///
        ///
        /// </summary>
        /// <param name="Id">id of Position</param>
        /// <param name="value">DTO of Position to change</param>
        /// <returns></returns>
        /// <response code="204">resource modified</response>
        /// <response code="403">not allowed to modify the resource</response>
        /// <response code="404">resource does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [Microsoft.AspNetCore.Mvc.HttpPut("{positionId}")]
        public override async Task< PositionResponseDto > Put(long positionId, [Microsoft.AspNetCore.Mvc.FromBody]PositionRequestDto requestDto)
        {
            return null;

        }

        // DELETE: api/Position/5
        /// <summary>
        /// removes a Position from the system
        ///
        ///
        /// </summary>
        /// <param name="Id">id of Position to remove</param>
        /// <response code="204">Position removed</response>
        /// <response code="403">not allowed to remove Position</response>
        /// <response code="404">Position does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpDelete]
        public async Task<bool> Delete(int positionId)
        {
            return true;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("Search")]
        public override async Task<PositionSearchResponseDto> Search(PositionSearchRequestDto searchRequest)
        {
            
            PositionSearchResponseDto result = new PositionSearchResponseDto
            {
                Data = _mapper.Map(_dbContext
                    .Positions
                    .FilterByBotId(searchRequest.BotId)
                    .FilterBySide(searchRequest.Side)
                    .ToList(),
                    new List<PositionResponseDto>()
                )
            };

            return result;
        }



    }
    
}
