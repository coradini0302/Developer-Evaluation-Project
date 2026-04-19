using Ambev.DeveloperEvaluation.Common.Validation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected int GetCurrentUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new NullReferenceException());

    protected string GetCurrentUserEmail() =>
        User.FindFirst(ClaimTypes.Email)?.Value ?? throw new NullReferenceException();

    protected IActionResult Success<T>(T data, string? message = null) =>
        base.Ok(new ApiResponseWithData<T>
        {
            Data = data,
            Success = true,
            Message = message,
            Errors = null
        });

    protected IActionResult Success(string message) =>
        base.Ok(new ApiResponse
        {
            Success = true,
            Message = message,
            Errors = null
        });

    protected IActionResult Created<T>(string routeName, object routeValues, T data) =>
        base.CreatedAtRoute(routeName, routeValues, new ApiResponseWithData<T>
        {
            Data = data,
            Success = true
        });

    protected IActionResult BadRequest(IEnumerable<ValidationErrorDetail> errors, string message = "Validation failed") =>
        base.BadRequest(new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        });

    protected IActionResult NotFound(string message = "Resource not found") =>
        base.NotFound(new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = null
        });

    protected IActionResult SuccessPaginated<T>(PaginatedList<T> pagedList) =>
        base.Ok(new PaginatedResponse<T>
        {
            Data = pagedList,
            CurrentPage = pagedList.CurrentPage,
            TotalPages = pagedList.TotalPages,
            TotalCount = pagedList.TotalCount,
            Success = true
        });
}