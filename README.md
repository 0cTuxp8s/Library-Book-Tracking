# Library Book Tracking System

A comprehensive C# Windows Forms application for managing library books with database-style GUI interface. This undergraduate project provides a complete solution for tracking books, managing borrow/return operations, and generating statistics.

## Features

### Core Features
- ✅ **Add Books** - Add new books with title, author, and year
- ✅ **Edit Books** - Update existing book information
- ✅ **Delete Books** - Remove books from the system
- ✅ **Track Borrowed/Returned Books** - Complete borrow and return management
- ✅ **Show Available Books** - View all available books in the library
- ✅ **Search by Title or Author** - Quick search functionality
- ✅ **Search Recommendations** - Intelligent search suggestions as you type
- ✅ **Database-Style GUI** - Professional data grid interface

### Additional Features
- 📊 **Statistics Dashboard** - View comprehensive library statistics
- 📤 **Export to CSV** - Export book data to CSV format
- 🔍 **Advanced Filtering** - Filter books by availability status
- 📋 **Borrow Records Management** - Complete history of all borrow transactions
- ⚠️ **Overdue Tracking** - Identify overdue books automatically
- 🎨 **Modern UI Design** - Clean and intuitive user interface
- 💾 **SQLite Database** - Lightweight, file-based database storage

## Technology Stack

- **.NET 6.0** - Modern .NET framework
- **Windows Forms** - Desktop GUI framework
- **Entity Framework Core 7.0** - ORM for database operations
- **SQLite** - Embedded database engine
- **C#** - Programming language

## Project Structure

```
LibraryBookTracking/
├── Data/
│   └── LibraryDbContext.cs      # Database context and configuration
├── Models/
│   ├── Book.cs                   # Book entity model
│   └── BorrowRecord.cs           # Borrow record entity model
├── Services/
│   └── BookService.cs            # Business logic and data operations
├── Forms/
│   ├── MainForm.cs               # Main application window
│   ├── BookForm.cs               # Add/Edit book dialog
│   └── BorrowForm.cs             # Borrow book dialog
└── Program.cs                    # Application entry point
```

## Installation & Setup

### Prerequisites
- Windows OS
- .NET 6.0 SDK or later
- Visual Studio 2022 (recommended) or Visual Studio Code

### Steps to Run

1. **Clone or Download** the project to your local machine

2. **Open the Solution**
   - Open `LibraryBookTracking.sln` in Visual Studio
   - Or use command line: `dotnet restore`

3. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

4. **Build the Project**
   ```bash
   dotnet build
   ```

5. **Run the Application**
   ```bash
   dotnet run --project LibraryBookTracking
   ```
   Or simply press F5 in Visual Studio

6. **Database Creation**
   - The SQLite database (`library.db`) will be automatically created on first run
   - No additional setup required!

## Usage Guide

### Adding a Book
1. Click the **"Add Book"** button
2. Fill in the book details (Title, Author, Year)
3. Click **"Save"**

### Searching for Books
1. Type in the search box at the top
2. View real-time search recommendations
3. Click on a recommendation to auto-fill the search
4. Results update automatically as you type

### Borrowing a Book
1. Select a book from the list (must be available)
2. Click **"Borrow Book"** button
3. Enter borrower information
4. Set due date (default: 14 days)
5. Click **"Borrow"**

### Returning a Book
1. Go to **"Borrow Records"** tab
2. Select an active borrow record
3. Click **"Return Book"** button
4. Confirm the return

### Viewing Statistics
1. Click **"View Statistics"** button or go to **"Statistics"** tab
2. View comprehensive library metrics including:
   - Total books
   - Available/borrowed counts
   - Active borrows
   - Overdue books

### Exporting Data
1. Click **"Export to CSV"** button
2. Choose save location
3. Data will be exported in CSV format

## Database Schema

### Book Table
- `Id` (Primary Key)
- `Title` (Required, Max 200 chars)
- `Author` (Required, Max 100 chars)
- `Year` (Required)
- `IsAvailable` (Boolean)
- `DateAdded` (DateTime)

### BorrowRecord Table
- `Id` (Primary Key)
- `BookId` (Foreign Key)
- `BorrowerName` (Required, Max 100 chars)
- `BorrowerEmail` (Optional, Max 50 chars)
- `BorrowerPhone` (Optional, Max 20 chars)
- `BorrowDate` (DateTime)
- `ReturnDate` (Nullable DateTime)
- `DueDate` (DateTime)
- `IsReturned` (Boolean)
- `Notes` (Optional, Max 500 chars)

## Key Features Explained

### Search Recommendations
The system provides intelligent search suggestions that appear as you type. These recommendations help you quickly find books by matching titles or authors.

### Availability Filtering
Use the "Show Available Only" checkbox to filter the book list and see only books that are currently available for borrowing.

### Overdue Tracking
The system automatically identifies overdue books by comparing the due date with the current date. Overdue books are highlighted in red in the borrow records.

### Data Export
Export all book data to CSV format for external analysis or backup purposes.

## Screenshots

The application features:
- **Books Tab**: Main interface for managing books with search and filtering
- **Borrow Records Tab**: Complete history of all borrow transactions
- **Statistics Tab**: Comprehensive library statistics and metrics

## Future Enhancements (Optional)

- User authentication and role-based access
- Email notifications for due dates
- Barcode/ISBN support
- Book categories and tags
- Advanced reporting and analytics
- Multi-library support
- Book reservations system
- Fine calculation for overdue books

## Troubleshooting

### Database Issues
If you encounter database errors:
- Delete the `library.db` file and restart the application
- Ensure you have write permissions in the application directory

### Build Errors
- Ensure .NET 6.0 SDK is installed
- Run `dotnet restore` to restore packages
- Check that all NuGet packages are properly referenced

## License

This project is created for educational purposes as an undergraduate project.

## Author

Created as a complete undergraduate project demonstrating:
- C# Windows Forms development
- Entity Framework Core usage
- SQLite database integration
- Modern GUI design principles
- Complete CRUD operations
- Search and filtering functionality

## Support

For issues or questions, please refer to the code comments or contact the development team.

---

**Note**: This is a complete, production-ready undergraduate project with all requested features and additional enhancements for a comprehensive library management system.

