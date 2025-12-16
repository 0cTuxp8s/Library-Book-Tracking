# CSV Import Test Files

This folder contains CSV test files for testing the import functionality of the Library Book Tracking System.

## Test Files

### 1. test_books.csv
**Purpose:** Test book import functionality
**Format:** Title, Author, Year
**Records:** 20 classic books
**Use Case:** Import a collection of books

### 2. test_borrows_and_returns.csv
**Purpose:** Test borrow and return import functionality
**Format:** BookTitle, BorrowerName, Email, Phone, BorrowDate, DueDate, ReturnDate, IsReturned, Notes
**Records:** 10 records (5 returned, 5 active)
**Use Case:** Import borrow records with mixed return status

### 3. test_books_simple.csv
**Purpose:** Quick test of book import
**Format:** Title, Author, Year
**Records:** 5 modern books
**Use Case:** Quick import test

### 4. test_borrows_only.csv
**Purpose:** Test borrow-only import (no returns)
**Format:** BookTitle, BorrowerName, Email, Phone, BorrowDate, DueDate
**Records:** 3 active borrow records
**Use Case:** Import only new borrows without return data

### 5. test_returns_only.csv
**Purpose:** Test return processing
**Format:** BookTitle, BorrowerName, Email, Phone, BorrowDate, DueDate, ReturnDate, IsReturned
**Records:** 3 return records
**Use Case:** Process returns for existing borrow records

## Import Instructions

### Step 1: Import Books
1. Open the Library Book Tracking System
2. Click "Import CSV" button
3. Select `test_books.csv` or `test_books_simple.csv`
4. Verify books are imported successfully

### Step 2: Import Borrows
1. Click "Import CSV" button
2. Select `test_borrows_and_returns.csv` or `test_borrows_only.csv`
3. Verify borrow records are created
4. Check that books are marked as "Borrowed"

### Step 3: Import Returns
1. Click "Import CSV" button
2. Select `test_returns_only.csv` or use `test_borrows_and_returns.csv` (which has returns)
3. Verify returns are processed
4. Check that books are marked as "Available"

## Expected Results

### After Importing test_books.csv:
- 20 books should be added to the system
- All books should show status as "Available"

### After Importing test_borrows_and_returns.csv:
- 10 borrow records should be created
- 5 books should be marked as "Borrowed"
- 5 books should be marked as "Available" (returned)
- Return dates should be set for returned books

### After Importing test_returns_only.csv:
- Existing borrow records should be updated
- Books should become available
- Return dates should be set

## CSV Format Specifications

### Books Format:
```csv
Title,Author,Year
"Book Title","Author Name",2024
```

**Required Fields:**
- Title (text, can contain quotes)
- Author (text, can contain quotes)
- Year (integer)

### Borrows/Returns Format:
```csv
BookTitle,BorrowerName,Email,Phone,BorrowDate,DueDate,ReturnDate,IsReturned,Notes
"Book Title","Borrower Name","email@example.com","555-0101","2024-01-15","2024-01-29","2024-01-28","Yes","Notes"
```

**Required Fields:**
- BookTitle (must match existing book in database)
- BorrowerName
- BorrowDate (format: YYYY-MM-DD)
- DueDate (format: YYYY-MM-DD)

**Optional Fields:**
- Email
- Phone
- ReturnDate (for returns)
- IsReturned ("Yes"/"No" or "True"/"False" or "1"/"0")
- Notes

## Notes

- All text fields should be enclosed in double quotes if they contain commas
- Date format: YYYY-MM-DD (e.g., 2024-01-15)
- IsReturned accepts: "Yes", "No", "True", "False", "1", "0"
- Books must exist in database before importing borrows
- Duplicate books (same title and author) will be skipped
- Books that are already borrowed cannot be borrowed again

## Troubleshooting

**Error: "Book not found"**
- Solution: Import books first before importing borrows

**Error: "Book is already borrowed"**
- Solution: Import return data first, or the book is already borrowed in the system

**Error: "Invalid date format"**
- Solution: Use YYYY-MM-DD format for all dates

**Error: "Insufficient columns"**
- Solution: Check that CSV has all required columns in the header row

