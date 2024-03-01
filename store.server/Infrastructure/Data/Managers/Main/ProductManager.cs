﻿using store.server.Infrastructure.Models.Main;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Data.Repo.Main;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Data.Interface.Main;

namespace store.server.Infrastructure.Data.Managers.Main
{
    public class ProductManager : AppSettings, IProducts
    {
        private readonly IProducts _repo;
        public ProductManager()
        {
            _repo = new ProductRepository();
        }
        public async Task<bool> Archive(Products entity)
        {
            return await _repo.Archive(entity);
        }

        public async Task<FilteredList<Products>?> FilteredList(Filter filter)
        {
            return await _repo.FilteredList(filter); ;
        }

        public async Task<Products?> Get(int ID)
        {
            return await _repo.Get(ID);
        }

        public async Task<Products?> Manage(Products entity)
        {
            return await _repo.Manage(entity);
        }
    }
}
