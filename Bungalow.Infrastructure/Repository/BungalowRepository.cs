﻿using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Domain.Entities;
using BungalowApi.Infrastructure.Data;

namespace BungalowApi.Infrastructure.Repository;

public class BungalowRepository : Repository<Bungalow>, IBungalowRepository
{
    private readonly ApplicationDbContext _context;
    public BungalowRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public void Update(Bungalow entity)
    {
        _context.Update(entity);
    }
}
