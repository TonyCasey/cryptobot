using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Api.CryptoBot.Controllers
{
    /// <summary>
    /// A controller to contain common functionality reusable in its children
    /// </summary>
    //[ApiExceptionFilter]
    [EnableCors("CorsPolicy")]
    public abstract class BaseController<TRequestDto, TResponseDto, TSearchRequestDto, TSearchResponseDto> : Controller 
        where TRequestDto : RequestBaseDto
        where TResponseDto : ResponseBaseDto
        where TSearchRequestDto : SearchBase
        where TSearchResponseDto : SearchResponseBase<TResponseDto>
    {
        protected readonly IMapper _mapper;
        
        public BaseController(IMapper mapper)
        {
            _mapper = mapper;
        }

        public abstract Task<TResponseDto> Get(long id);
        public abstract Task<CreatedResult> Post(TRequestDto dto);
        public abstract Task<TResponseDto> Put(long id, TRequestDto dto);
        public abstract Task<TSearchResponseDto> Search(TSearchRequestDto searchRequest);
    }
}
