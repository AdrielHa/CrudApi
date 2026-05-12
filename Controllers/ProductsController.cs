using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necesario para Entity Framework
using ProductApi.Models;

[ApiController]
[Route("Api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    // Inyectamos el contexto de la base de datos en el constructor
    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Api/Products 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        return await _context.Products.ToListAsync();
    }

    // GET: Api/Products/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound("Product not found");
        return Ok(product);
    }

    // POST: Api/Products
    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        if (string.IsNullOrEmpty(product.Name))
            return BadRequest("Name is required");

        _context.Products.Add(product);
        await _context.SaveChangesAsync(); // <--- ESTO ES LO QUE GUARDA EN SQL

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT: Api/Products/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product updateProduct)
    {
        if (id != updateProduct.Id) return BadRequest();

        _context.Entry(updateProduct).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Products.Any(p => p.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: Api/Products/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound("Product not found");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}