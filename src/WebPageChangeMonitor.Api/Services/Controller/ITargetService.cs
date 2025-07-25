﻿using System;
using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Requests;
using WebPageChangeMonitor.Models.Responses;

namespace WebPageChangeMonitor.Api.Services.Controller;

public interface ITargetService
{
    Task<TargetPaginatedResponse> GetAsync(SortDirection? sortDirection, string sortBy, int? page, int count);
    Task<TargetDto> GetAsync(Guid id);
    Task<TargetPaginatedResponse> GetByResourceIdAsync(Guid id, SortDirection? sortDirection, string sortBy, int? page, int count);
    Task<TargetDto> CreateAsync(CreateTargetRequest request);
    Task<TargetDto> UpdateAsync(Target updatedTarget);
    Task RemoveAsync(Guid id);
    Task RemoveByResourceIdAsync(Guid id);
}
