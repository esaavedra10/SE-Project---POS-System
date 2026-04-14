using System;
using Microsoft.AspNetCore.Mvc;
using MongoExample.Services;
using MongoExample.Models;

namespace MongoExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly MongoDBService _mongoDBService;

    public EmployeesController(MongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Employees>>> Get()
    {
        var employees = await _mongoDBService.GetAsync();
        return Ok(employees);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Employees employee)
    {
        if (employee == null)
        {
            return BadRequest("Employee data is required.");
        }

        await _mongoDBService.CreateAsync(employee);
        return CreatedAtAction(nameof(Get), new { id = employee.Id }, employee);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(string id, [FromBody] string EID)
    {
        var employee = await _mongoDBService.GetAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        employee.EID = EID;
        await _mongoDBService.UpdateAsync(id, employee);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var employee = await _mongoDBService.GetAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        await _mongoDBService.RemoveAsync(id);
        return NoContent();
    }
}