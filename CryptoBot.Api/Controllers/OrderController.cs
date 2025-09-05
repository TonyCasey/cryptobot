using Api.CryptoBot.Data;
using Api.CryptoBot.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO.Order;
using Api.CryptoBot.Models.Extensions;
using CryptoBot.Model.Domain;
using Asp.Versioning;

namespace Api.CryptoBot.Controllers
{
    /// <summary>
    /// Order controller
    ///
    ///</summary>
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/[controller]")]
    public class OrderController : BaseController< OrderRequestDto, OrderResponseDto, OrderSearchRequestDto, OrderSearchResponseDto >
    {

        private readonly IMapper _mapper;
        private readonly CryptoBotApiDbContext _dbContext;

        /// <summary>
        /// Main constructor to inject mapper and IService
        ///
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public OrderController (IMapper mapper,  CryptoBotApiDbContext dbContext )
        :base(mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        // GET: api/Order/5
        /// <summary>
        /// returns a specific Order with the specified id
        ///
        ///
        /// </summary>
        /// <param name="id">id of Order to return</param>
        /// <returns>a single Order</returns>
        /// <response code="200">Order found - body contains data</response>
        /// <response code="404">Order does not exist</response>
        [Microsoft.AspNetCore.Mvc.HttpGet("{orderId}")]
        public override async Task<OrderResponseDto> Get(long orderId)
        {
            return _mapper.Map(_dbContext.Orders.FirstOrDefault(x=>x.OrderId == orderId), new OrderResponseDto());
        }

        // POST: api/Order
        /// <summary>
        /// adds a Order type to the system
        ///
        ///
        /// </summary>
        /// <param name="value">new Order DTO to add</param>
        /// <response code="201">Order created</response>
        /// <response code="400">Request is not valid</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(Uri))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(void))]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public override async Task<CreatedResult> Post([Microsoft.AspNetCore.Mvc.FromBody]OrderRequestDto requestDto)
        {

            var id =1; // create record

            return Created($"/api/Order/{id}", id );

        }

        // PUT: api/Order/5
        /// <summary>
        /// modifies the existing Order
        ///
        ///
        /// </summary>
        /// <param name="id">id of Order</param>
        /// <param name="value">DTO of Order to change</param>
        /// <returns></returns>
        /// <response code="204">resource modified</response>
        /// <response code="403">not allowed to modify the resource</response>
        /// <response code="404">resource does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [Microsoft.AspNetCore.Mvc.HttpPut("{orderId}")]
        public override async Task< OrderResponseDto > Put(long orderId, [Microsoft.AspNetCore.Mvc.FromBody]OrderRequestDto requestDto)
        {
            return null;

        }

        // DELETE: api/Order/5
        /// <summary>
        /// removes a Order from the system
        ///
        ///
        /// </summary>
        /// <param name="id">id of Order to remove</param>
        /// <response code="204">Order removed</response>
        /// <response code="403">not allowed to remove Order</response>
        /// <response code="404">Order does not exist</response>
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [HttpDelete]
        public async Task<bool> Delete(long orderId)
        {
            return true;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("Search")]
        public override async Task< OrderSearchResponseDto > Search(OrderSearchRequestDto searchRequest)
        {
            var test = _dbContext
                .Orders
                //.FilterByBotId(searchRequest.BotId)
                //.FilterByPositionId(searchRequest.PositionId)
                //.FilterBySide(searchRequest.Side)
                .ToList();

            OrderSearchResponseDto result = new OrderSearchResponseDto
            {
                Data = _mapper.Map(
                    _dbContext
                    .Orders
                    .FilterByBotId(searchRequest.BotId)
                    .FilterByPositionId(searchRequest.PositionId)
                    .FilterBySide(searchRequest.Side)
                    .ToList()
                    , new List<OrderResponseDto>())
            };
            return result;
        }



    }
            
}
