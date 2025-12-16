using LibraryBookTracking.Data;
using LibraryBookTracking.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using System.Text;

namespace LibraryBookTracking.Services
{
    public class BookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.BorrowRecords)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<List<Book>> GetAvailableBooksAsync()
        {
            return await _context.Books
                .Where(b => b.IsAvailable)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.BorrowRecords)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Book>> SearchBooksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllBooksAsync();

            // Trim and prepare search term
            searchTerm = searchTerm.Trim();
            
            // Fetch all books and filter in memory for reliable case-insensitive search
            // This works better with SQLite than ToLower() in LINQ
            var allBooks = await _context.Books
                .OrderBy(b => b.Title)
                .ToListAsync();
            
            var lowerSearchTerm = searchTerm.ToLower();
            return allBooks
                .Where(b => b.Title.ToLower().Contains(lowerSearchTerm) ||
                           b.Author.ToLower().Contains(lowerSearchTerm))
                .ToList();
        }

        public async Task<List<string>> GetSearchRecommendationsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<string>();

            // Trim and prepare search term
            searchTerm = searchTerm.Trim();
            var lowerSearchTerm = searchTerm.ToLower();
            
            // Fetch all books and filter in memory for reliable case-insensitive search
            var allBooks = await _context.Books
                .Select(b => new { b.Title, b.Author })
                .ToListAsync();

            var recommendations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            foreach (var book in allBooks)
            {
                // Check if title matches (case-insensitive)
                if (book.Title.ToLower().Contains(lowerSearchTerm))
                    recommendations.Add(book.Title);
                // Check if author matches (case-insensitive)
                if (book.Author.ToLower().Contains(lowerSearchTerm))
                    recommendations.Add(book.Author);
            }

            return recommendations.OrderBy(r => r, StringComparer.OrdinalIgnoreCase).Take(10).ToList();
        }

        public async Task<bool> AddBookAsync(Book book)
        {
            try
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            try
            {
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                var book = await GetBookByIdAsync(id);
                if (book != null)
                {
                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BorrowBookAsync(int bookId, string borrowerName, string? email, string? phone, DateTime? dueDate, string? notes)
        {
            try
            {
                var book = await GetBookByIdAsync(bookId);
                if (book == null || !book.IsAvailable)
                    return false;

                var record = new BorrowRecord
                {
                    BookId = bookId,
                    BorrowerName = borrowerName,
                    BorrowerEmail = email,
                    BorrowerPhone = phone,
                    BorrowDate = DateTime.Now,
                    DueDate = dueDate ?? DateTime.Now.AddDays(14),
                    Notes = notes
                };

                book.IsAvailable = false;
                _context.BorrowRecords.Add(record);
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ReturnBookAsync(int borrowRecordId)
        {
            try
            {
                var record = await _context.BorrowRecords
                    .Include(r => r.Book)
                    .FirstOrDefaultAsync(r => r.Id == borrowRecordId);

                if (record == null || record.IsReturned)
                    return false;

                record.IsReturned = true;
                record.ReturnDate = DateTime.Now;
                record.Book.IsAvailable = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<BorrowRecord>> GetActiveBorrowsAsync()
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Where(r => !r.IsReturned)
                .OrderByDescending(r => r.BorrowDate)
                .ToListAsync();
        }

        public async Task<List<BorrowRecord>> GetAllBorrowRecordsAsync()
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .OrderByDescending(r => r.BorrowDate)
                .ToListAsync();
        }

        public async Task<Dictionary<string, object>> GetStatisticsAsync()
        {
            var totalBooks = await _context.Books.CountAsync();
            var availableBooks = await _context.Books.CountAsync(b => b.IsAvailable);
            var borrowedBooks = totalBooks - availableBooks;
            var totalBorrows = await _context.BorrowRecords.CountAsync();
            var activeBorrows = await _context.BorrowRecords.CountAsync(r => !r.IsReturned);
            var overdueBorrows = await _context.BorrowRecords
                .CountAsync(r => !r.IsReturned && r.DueDate < DateTime.Now);

            return new Dictionary<string, object>
            {
                { "TotalBooks", totalBooks },
                { "AvailableBooks", availableBooks },
                { "BorrowedBooks", borrowedBooks },
                { "TotalBorrows", totalBorrows },
                { "ActiveBorrows", activeBorrows },
                { "OverdueBorrows", overdueBorrows }
            };
        }

        public async Task<(int booksImported, int borrowsImported, int returnsProcessed, List<string> errors)> ImportFromCsvAsync(string filePath)
        {
            var errors = new List<string>();
            int booksImported = 0;
            int borrowsImported = 0;
            int returnsProcessed = 0;

            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                if (lines.Length < 2)
                {
                    errors.Add("CSV file must have at least a header row and one data row.");
                    return (0, 0, 0, errors);
                }

                // Parse header
                var header = lines[0].Split(',');
                var isBooksCsv = header.Any(h => h.Contains("Title", StringComparison.OrdinalIgnoreCase) && 
                                                 !h.Contains("BookTitle", StringComparison.OrdinalIgnoreCase));
                var isBorrowsCsv = header.Any(h => h.Contains("Borrower", StringComparison.OrdinalIgnoreCase) || 
                                                   h.Contains("BookTitle", StringComparison.OrdinalIgnoreCase));
                var hasReturnData = header.Any(h => h.Contains("ReturnDate", StringComparison.OrdinalIgnoreCase) || 
                                                    h.Contains("IsReturned", StringComparison.OrdinalIgnoreCase));

                if (isBooksCsv)
                {
                    // Fetch all existing books once for duplicate checking
                    var allBooks = await _context.Books.ToListAsync();
                    
                    // Import books
                    for (int i = 1; i < lines.Length; i++)
                    {
                        try
                        {
                            var line = lines[i];
                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            var values = ParseCsvLine(line);
                            if (values.Count < 3)
                            {
                                errors.Add($"Line {i + 1}: Insufficient columns");
                                continue;
                            }

                            var title = values[0].Trim().Trim('"');
                            var author = values[1].Trim().Trim('"');
                            
                            if (!int.TryParse(values[2].Trim(), out int year))
                            {
                                errors.Add($"Line {i + 1}: Invalid year format");
                                continue;
                            }

                            // Check if book already exists (case-insensitive)
                            var existingBook = allBooks
                                .FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase) &&
                                                     b.Author.Equals(author, StringComparison.OrdinalIgnoreCase));

                            if (existingBook == null)
                            {
                                var book = new Book
                                {
                                    Title = title,
                                    Author = author,
                                    Year = year,
                                    IsAvailable = true,
                                    DateAdded = DateTime.Now
                                };

                                _context.Books.Add(book);
                                allBooks.Add(book); // Add to local list for subsequent checks
                                booksImported++;
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Line {i + 1}: {ex.Message}");
                        }
                    }
                }
                else if (isBorrowsCsv)
                {
                    // Fetch all books and records once for lookup
                    var allBooks = await _context.Books.ToListAsync();
                    var allRecords = await _context.BorrowRecords
                        .Include(r => r.Book)
                        .Where(r => !r.IsReturned)
                        .ToListAsync();
                    
                    // Import borrow records
                    for (int i = 1; i < lines.Length; i++)
                    {
                        try
                        {
                            var line = lines[i];
                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            var values = ParseCsvLine(line);
                            if (values.Count < 4)
                            {
                                errors.Add($"Line {i + 1}: Insufficient columns");
                                continue;
                            }

                            var bookTitle = values[0].Trim().Trim('"');
                            var borrowerName = values[1].Trim().Trim('"');
                            var borrowerEmail = values.Count > 2 ? values[2].Trim().Trim('"') : "";
                            var borrowerPhone = values.Count > 3 ? values[3].Trim().Trim('"') : "";
                            
                            // Find book (case-insensitive)
                            var book = allBooks
                                .FirstOrDefault(b => b.Title.Equals(bookTitle, StringComparison.OrdinalIgnoreCase));

                            if (book == null)
                            {
                                errors.Add($"Line {i + 1}: Book '{bookTitle}' not found");
                                continue;
                            }

                            // Check if this is a return record
                            bool isReturned = false;
                            DateTime? returnDate = null;
                            
                            if (hasReturnData)
                            {
                                // Check IsReturned column (usually last or second to last)
                                var isReturnedIndex = Array.FindIndex(header, h => h.Contains("IsReturned", StringComparison.OrdinalIgnoreCase));
                                if (isReturnedIndex >= 0 && isReturnedIndex < values.Count)
                                {
                                    var isReturnedValue = values[isReturnedIndex].Trim().Trim('"').ToLower();
                                    isReturned = isReturnedValue == "yes" || isReturnedValue == "true" || isReturnedValue == "1";
                                }
                                
                                // Check ReturnDate column
                                var returnDateIndex = Array.FindIndex(header, h => h.Contains("ReturnDate", StringComparison.OrdinalIgnoreCase));
                                if (returnDateIndex >= 0 && returnDateIndex < values.Count && !string.IsNullOrWhiteSpace(values[returnDateIndex]))
                                {
                                    if (DateTime.TryParse(values[returnDateIndex].Trim().Trim('"'), out DateTime parsedReturnDate))
                                    {
                                        returnDate = parsedReturnDate;
                                        isReturned = true;
                                    }
                                }
                            }

                            if (isReturned && returnDate.HasValue)
                            {
                                // This is a return - find existing borrow record (case-insensitive)
                                var existingRecord = allRecords
                                    .FirstOrDefault(r => r.Book.Title.Equals(bookTitle, StringComparison.OrdinalIgnoreCase) &&
                                                         r.BorrowerName.Equals(borrowerName, StringComparison.OrdinalIgnoreCase));

                                if (existingRecord != null)
                                {
                                    existingRecord.IsReturned = true;
                                    existingRecord.ReturnDate = returnDate;
                                    existingRecord.Book.IsAvailable = true;
                                    returnsProcessed++;
                                }
                                else
                                {
                                    errors.Add($"Line {i + 1}: No active borrow record found for return");
                                }
                            }
                            else
                            {
                                // This is a new borrow
                                if (!book.IsAvailable)
                                {
                                    errors.Add($"Line {i + 1}: Book '{bookTitle}' is already borrowed");
                                    continue;
                                }

                                var borrowDate = DateTime.Now;
                                var dueDate = DateTime.Now.AddDays(14);

                                var borrowDateIndex = Array.FindIndex(header, h => h.Contains("BorrowDate", StringComparison.OrdinalIgnoreCase));
                                var dueDateIndex = Array.FindIndex(header, h => h.Contains("DueDate", StringComparison.OrdinalIgnoreCase));

                                if (borrowDateIndex >= 0 && borrowDateIndex < values.Count && DateTime.TryParse(values[borrowDateIndex].Trim().Trim('"'), out DateTime parsedBorrowDate))
                                    borrowDate = parsedBorrowDate;

                                if (dueDateIndex >= 0 && dueDateIndex < values.Count && DateTime.TryParse(values[dueDateIndex].Trim().Trim('"'), out DateTime parsedDueDate))
                                    dueDate = parsedDueDate;

                                var notesIndex = Array.FindIndex(header, h => h.Contains("Notes", StringComparison.OrdinalIgnoreCase));
                                var notes = notesIndex >= 0 && notesIndex < values.Count ? values[notesIndex].Trim().Trim('"') : "";

                                var record = new BorrowRecord
                                {
                                    BookId = book.Id,
                                    BorrowerName = borrowerName,
                                    BorrowerEmail = string.IsNullOrWhiteSpace(borrowerEmail) ? null : borrowerEmail,
                                    BorrowerPhone = string.IsNullOrWhiteSpace(borrowerPhone) ? null : borrowerPhone,
                                    BorrowDate = borrowDate,
                                    DueDate = dueDate,
                                    ReturnDate = returnDate,
                                    IsReturned = isReturned,
                                    Notes = string.IsNullOrWhiteSpace(notes) ? null : notes
                                };

                                book.IsAvailable = !isReturned;
                                _context.BorrowRecords.Add(record);
                                borrowsImported++;
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Line {i + 1}: {ex.Message}");
                        }
                    }
                }
                else
                {
                    errors.Add("CSV file format not recognized. Expected books or borrows/returns format.");
                    return (0, 0, 0, errors);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                errors.Add($"Error reading CSV file: {ex.Message}");
            }

            return (booksImported, borrowsImported, returnsProcessed, errors);
        }

        private List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            var currentValue = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            values.Add(currentValue.ToString());
            return values;
        }
    }
}

