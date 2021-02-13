﻿using ESO_LangEditor.EFCore.DataRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESO_LangEditor.EFCore.RepositoryWrapper
{
    public class RepositoryWrapperClient : IRepositoryWrapperClient
    {
        private readonly ILangTextRepository _langtextRepository = null;
        private readonly ILangTextRevNumberRepository _langTextRevNumberRepository = null;

        public LangtextClientDbContext LangtextClientDbContext { get; }

        public ILangTextRevNumberRepository LangTextRevNumberRepo => _langTextRevNumberRepository ?? new LangTextRevNumberRepository(LangtextClientDbContext);

        public ILangTextRepository LangTextRepo => _langtextRepository ?? new LangTextRepositoryClient(LangtextClientDbContext);

        public RepositoryWrapperClient(LangtextClientDbContext langtextClientDbContext)
        {
            LangtextClientDbContext = langtextClientDbContext;
        }
    }
}
