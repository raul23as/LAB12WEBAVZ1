using Gimnasio.Api.Data;
using Gimnasio.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembresiasController(GimnasioContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Membresia>>> Get(string? tipoPlan, int? socioId)
    {
        var query = db.Membresias.AsNoTracking().Include(m => m.Socio).AsQueryable();
        if (!string.IsNullOrWhiteSpace(tipoPlan)) query = query.Where(m => m.TipoPlan == tipoPlan);
        if (socioId.HasValue) query = query.Where(m => m.SocioId == socioId.Value);
        return Ok(await query.OrderBy(m => m.FechaInicio).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Membresia>> Get(int id)
    {
        var membresia = await db.Membresias.AsNoTracking().Include(m => m.Socio)
            .FirstOrDefaultAsync(m => m.Id == id);
        return membresia is null ? NotFound() : Ok(membresia);
    }

    [HttpPost]
    public async Task<ActionResult<Membresia>> Post(Membresia membresia)
    {
        if (membresia.FechaFin <= membresia.FechaInicio)
            return BadRequest(new { mensaje = "La fecha fin debe ser posterior a la fecha inicio." });
        if (!await db.Socios.AnyAsync(s => s.Id == membresia.SocioId))
            return BadRequest(new { mensaje = "El socio indicado no existe." });
        if (await db.Membresias.AnyAsync(m => m.Codigo == membresia.Codigo))
            return Conflict(new { mensaje = "El código de membresía ya existe." });
        membresia.Id = 0;
        db.Membresias.Add(membresia);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = membresia.Id }, membresia);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Membresia membresia)
    {
        if (id != membresia.Id) return BadRequest(new { mensaje = "El id de la ruta no coincide con el cuerpo." });
        if (!await db.Membresias.AnyAsync(m => m.Id == id)) return NotFound();
        if (membresia.FechaFin <= membresia.FechaInicio)
            return BadRequest(new { mensaje = "La fecha fin debe ser posterior a la fecha inicio." });
        if (!await db.Socios.AnyAsync(s => s.Id == membresia.SocioId))
            return BadRequest(new { mensaje = "El socio indicado no existe." });
        if (await db.Membresias.AnyAsync(m => m.Codigo == membresia.Codigo && m.Id != id))
            return Conflict(new { mensaje = "El código de membresía ya existe." });
        db.Entry(membresia).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var membresia = await db.Membresias.FindAsync(id);
        if (membresia is null) return NotFound();
        db.Membresias.Remove(membresia);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
