using Gimnasio.Api.Data;
using Gimnasio.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SociosController(GimnasioContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Socio>>> Get(string? nombre, string? correo)
    {
        var query = db.Socios.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(nombre)) query = query.Where(s => s.Nombre.Contains(nombre));
        if (!string.IsNullOrWhiteSpace(correo)) query = query.Where(s => s.Correo.Contains(correo));
        return Ok(await query.OrderBy(s => s.Nombre).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Socio>> Get(int id)
    {
        var socio = await db.Socios.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        return socio is null ? NotFound() : Ok(socio);
    }

    [HttpGet("{id:int}/membresias")]
    public async Task<ActionResult<IEnumerable<Membresia>>> GetMembresias(int id)
    {
        if (!await db.Socios.AnyAsync(s => s.Id == id)) return NotFound();
        return Ok(await db.Membresias.AsNoTracking().Where(m => m.SocioId == id).ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Socio>> Post(Socio socio)
    {
        if (await db.Socios.AnyAsync(s => s.Correo == socio.Correo))
            return Conflict(new { mensaje = "Ya existe un socio con ese correo." });
        socio.Id = 0;
        socio.FechaRegistro = DateTime.UtcNow;
        db.Socios.Add(socio);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = socio.Id }, socio);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Socio socio)
    {
        if (id != socio.Id) return BadRequest(new { mensaje = "El id de la ruta no coincide con el cuerpo." });
        var existente = await db.Socios.FindAsync(id);
        if (existente is null) return NotFound();
        if (await db.Socios.AnyAsync(s => s.Correo == socio.Correo && s.Id != id))
            return Conflict(new { mensaje = "El correo ya pertenece a otro socio." });
        existente.Nombre = socio.Nombre;
        existente.Correo = socio.Correo;
        existente.Telefono = socio.Telefono;
        existente.Edad = socio.Edad;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var socio = await db.Socios.FindAsync(id);
        if (socio is null) return NotFound();
        if (await db.Membresias.AnyAsync(m => m.SocioId == id))
            return Conflict(new { mensaje = "No se puede eliminar un socio con membresías registradas." });
        db.Socios.Remove(socio);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
