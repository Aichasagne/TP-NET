using BooksApi.Models;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly BooksService _booksService;

    public BooksController(BooksService booksService) =>
        _booksService = booksService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<List<Book>> Get() =>
        await _booksService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Book>> Get(string id)
    {
        var book = await _booksService.GetAsync(id);
        if (book is null) return NotFound();
        return book;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(Book newBook)
    {
        await _booksService.CreateAsync(newBook);
        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id:length(24)}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(string id, Book updatedBook)
    {
        var book = await _booksService.GetAsync(id);
        if (book is null) return NotFound();
        updatedBook.Id = book.Id;
        await _booksService.UpdateAsync(id, updatedBook);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _booksService.GetAsync(id);
        if (book is null) return NotFound();
        await _booksService.RemoveAsync(id);
        return NoContent();
    }
}