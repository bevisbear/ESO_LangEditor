﻿using AutoMapper;
using ESO_LangEditor.Core.Entities;
using ESO_LangEditor.EFCore.RepositoryWrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESO_LangEditor.API.Controllers
{
    [Route("api/langtext/archive")]
    [ApiController]
    public class LangTextArchiveController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private IMapper _mapper;

        public LangTextArchiveController(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }

        public async Task<ActionResult<IEnumerable<LangTextArchive>>> GetLangTextAllAsync()
        {
            var langtextList = await _repositoryWrapper.LangTextArchiveRepo.GetAllAsync();

            return langtextList.ToList();
        }

        //[AllowAnonymous]
        [HttpGet("{langtextGuid}")]
        public async Task<ActionResult<LangTextArchive>> GetLangTextByGuidAsync(Guid langtextGuid)
        {
            var langtext = await _repositoryWrapper.LangTextArchiveRepo.GetByIdAsync(langtextGuid);

            if (langtext == null)
            {
                return NotFound();
            }
            else
            {
                return langtext;
            }

        }


    }
}
