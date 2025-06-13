using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers;

public class BooksController : Controller
{
    private readonly LibraryDbContext _context;

    public BooksController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var books = await _context.Books
                .Include(b => b.BorrowRecords)
                .AsNoTracking()
                .ToListAsync();

            return View(books);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the books.";
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || id == 0)
        {
            TempData["ErrorMessage"] = "Book id was not provided.";
            return View("NotFound");
        }
        try
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                TempData["ErrorMessage"] = $"No book found with ID {id}.";
                return View("NotFound");
            }

            return View(book);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the book details.";
            return View("Error");
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( Book book)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully added the book : {book.Title}.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the book.";
                return View(book);
            }
        }

        return View(book);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0)
        {
            TempData["ErrorMessage"] = $"Book ID was not provided for editing.";
            return View("NotFound");
        }
        try
        {
            var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                TempData["ErrorMessage"] = $"No book found with ID {id} for editing.";
                return View("NotFound");
            }

            return View(book);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the book for editing.";
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, Book book)
    {
        if (id == null || id == 0)
        {
            TempData["ErrorMessage"] = "Book ID was not provided for updating.";
            return View("NotFound");
        }
        if (ModelState.IsValid)
        {
            try
            {
                var existingBook = await _context.Books.FindAsync(id);

                if (existingBook == null)
                {
                    TempData["ErrorMessage"] = $"No Book found with ID {id} for updating.";
                    return View("NotFound");
                }

                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.ISBN = book.ISBN;
                existingBook.PublishedDate = book.PublishedDate;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully updated the book : {book.Title}.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookExists(book.BookId))
                {
                    TempData["ErrorMessage"] = $"No book found with ID {book.BookId} during concurrency check.";
                    return View("Not Found");
                }
                else
                {
                    TempData["ErrorMessage"] = "A Concurrency error occurred during the update.";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the book.";
                return View("Error");
            }
        }

        return View(book);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || id == 0)
        {
            TempData["ErrorMessage"] = "Book ID not provided for deletion.";
            return View("NotFound");
        }

        try
        {
            var book = await _context.Books
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                TempData["ErrorMessage"] = $"No Book found with ID {id} for deletion.";
                return View("NotFound");
            }

            return View(book);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurrde while loading the book for delection";
            return View("Error");
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        try
        {
            var existingBook = await _context.Books.FindAsync(id);

            if (existingBook == null)
            {
                TempData["ErrorMessage"] = $"No book found with ID {id} for deletion.";
                return View("NotFound");
            }

            _context.Remove(existingBook);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully deleted the book : {existingBook.Title}.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while deleting the book";
            return View("Error");
        }
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.BookId == id);
    }
}