using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryBookTracking.Data;
using LibraryBookTracking.Models;
using LibraryBookTracking.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraryBookTracking.Forms
{
    public partial class MainForm : Form
    {
        private LibraryDbContext _context;
        private BookService _bookService;
        private DataGridView dgvBooks;
        private DataGridView dgvBorrows;
        private TextBox txtSearch;
        private ListBox lstRecommendations;
        private bool _isUpdatingSearch = false;
        private Button btnAddBook;
        private Button btnEditBook;
        private Button btnDeleteBook;
        private Button btnBorrowBook;
        private Button btnReturnBook;
        private Button btnRefresh;
        private Button btnStatistics;
        private Button btnExport;
        private Button btnImport;
        private CheckBox chkShowAvailableOnly;
        private TabControl tabControl;
        private Label lblStats;
        private Panel pnlSearch;

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeServices();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Library Book Tracking System";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);

            // Create TabControl
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10F);

            // Books Tab
            TabPage tabBooks = new TabPage("Books");
            tabBooks.Padding = new Padding(10);
            CreateBooksTab(tabBooks);
            tabControl.TabPages.Add(tabBooks);

            // Borrows Tab
            TabPage tabBorrows = new TabPage("Borrow Records");
            tabBorrows.Padding = new Padding(10);
            CreateBorrowsTab(tabBorrows);
            tabControl.TabPages.Add(tabBorrows);

            // Statistics Tab
            TabPage tabStats = new TabPage("Statistics");
            tabStats.Padding = new Padding(10);
            CreateStatisticsTab(tabStats);
            tabControl.TabPages.Add(tabStats);

            this.Controls.Add(tabControl);
            this.ResumeLayout(false);
        }

        private void CreateBooksTab(TabPage tab)
        {
            // Search Panel
            pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 80;
            pnlSearch.BackColor = Color.FromArgb(240, 240, 240);
            pnlSearch.Padding = new Padding(10);

            Label lblSearch = new Label();
            lblSearch.Text = "Search:";
            lblSearch.Location = new Point(10, 15);
            lblSearch.AutoSize = true;
            lblSearch.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(80, 12);
            txtSearch.Size = new Size(300, 25);
            txtSearch.Font = new Font("Segoe UI", 10F);
            txtSearch.TextChanged += TxtSearch_TextChanged;

            lstRecommendations = new ListBox();
            lstRecommendations.Location = new Point(80, 40);
            lstRecommendations.Size = new Size(300, 100);
            lstRecommendations.Visible = false;
            lstRecommendations.Font = new Font("Segoe UI", 9F);
            lstRecommendations.SelectedIndexChanged += LstRecommendations_SelectedIndexChanged;
            lstRecommendations.LostFocus += (s, e) => { lstRecommendations.Visible = false; };

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(400, 10);
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.Font = new Font("Segoe UI", 9F);
            btnRefresh.Click += BtnRefresh_Click;

            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, lstRecommendations, btnRefresh });

            // Buttons Panel
            Panel pnlButtons = new Panel();
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Height = 50;
            pnlButtons.BackColor = Color.FromArgb(240, 240, 240);
            pnlButtons.Padding = new Padding(10);

            btnAddBook = new Button();
            btnAddBook.Text = "Add Book";
            btnAddBook.Size = new Size(100, 35);
            btnAddBook.Location = new Point(10, 7);
            btnAddBook.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAddBook.BackColor = Color.FromArgb(0, 120, 215);
            btnAddBook.ForeColor = Color.White;
            btnAddBook.FlatStyle = FlatStyle.Flat;
            btnAddBook.FlatAppearance.BorderSize = 0;
            btnAddBook.Click += BtnAddBook_Click;

            btnEditBook = new Button();
            btnEditBook.Text = "Edit Book";
            btnEditBook.Size = new Size(100, 35);
            btnEditBook.Location = new Point(120, 7);
            btnEditBook.Font = new Font("Segoe UI", 9F);
            btnEditBook.Click += BtnEditBook_Click;

            btnDeleteBook = new Button();
            btnDeleteBook.Text = "Delete Book";
            btnDeleteBook.Size = new Size(100, 35);
            btnDeleteBook.Location = new Point(230, 7);
            btnDeleteBook.Font = new Font("Segoe UI", 9F);
            btnDeleteBook.BackColor = Color.FromArgb(196, 43, 28);
            btnDeleteBook.ForeColor = Color.White;
            btnDeleteBook.FlatStyle = FlatStyle.Flat;
            btnDeleteBook.FlatAppearance.BorderSize = 0;
            btnDeleteBook.Click += BtnDeleteBook_Click;

            btnBorrowBook = new Button();
            btnBorrowBook.Text = "Borrow Book";
            btnBorrowBook.Size = new Size(120, 35);
            btnBorrowBook.Location = new Point(340, 7);
            btnBorrowBook.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBorrowBook.BackColor = Color.FromArgb(0, 153, 51);
            btnBorrowBook.ForeColor = Color.White;
            btnBorrowBook.FlatStyle = FlatStyle.Flat;
            btnBorrowBook.FlatAppearance.BorderSize = 0;
            btnBorrowBook.Click += BtnBorrowBook_Click;

            btnStatistics = new Button();
            btnStatistics.Text = "View Statistics";
            btnStatistics.Size = new Size(130, 35);
            btnStatistics.Location = new Point(470, 7);
            btnStatistics.Font = new Font("Segoe UI", 9F);
            btnStatistics.Click += BtnStatistics_Click;

            btnExport = new Button();
            btnExport.Text = "Export to CSV";
            btnExport.Size = new Size(120, 35);
            btnExport.Location = new Point(610, 7);
            btnExport.Font = new Font("Segoe UI", 9F);
            btnExport.Click += BtnExport_Click;

            btnImport = new Button();
            btnImport.Text = "Import CSV";
            btnImport.Size = new Size(120, 35);
            btnImport.Location = new Point(740, 7);
            btnImport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnImport.BackColor = Color.FromArgb(0, 120, 215);
            btnImport.ForeColor = Color.White;
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.FlatAppearance.BorderSize = 0;
            btnImport.Click += BtnImport_Click;

            chkShowAvailableOnly = new CheckBox();
            chkShowAvailableOnly.Text = "Show Available Only";
            chkShowAvailableOnly.Location = new Point(500, 15);
            chkShowAvailableOnly.Font = new Font("Segoe UI", 9F);
            chkShowAvailableOnly.AutoSize = true;
            chkShowAvailableOnly.CheckedChanged += ChkShowAvailableOnly_CheckedChanged;

            pnlButtons.Controls.AddRange(new Control[] { btnAddBook, btnEditBook, btnDeleteBook, btnBorrowBook, btnStatistics, btnExport, btnImport, chkShowAvailableOnly });

            // DataGridView
            dgvBooks = new DataGridView();
            dgvBooks.Dock = DockStyle.Fill;
            dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBooks.MultiSelect = false;
            dgvBooks.ReadOnly = true;
            dgvBooks.BackgroundColor = Color.White;
            dgvBooks.BorderStyle = BorderStyle.None;
            dgvBooks.Font = new Font("Segoe UI", 9F);
            dgvBooks.RowHeadersVisible = false;
            dgvBooks.AllowUserToAddRows = false;
            dgvBooks.CellFormatting += DgvBooks_CellFormatting;

            tab.Controls.Add(dgvBooks);
            tab.Controls.Add(pnlSearch);
            tab.Controls.Add(pnlButtons);
        }

        private void CreateBorrowsTab(TabPage tab)
        {
            // Buttons Panel
            Panel pnlButtons = new Panel();
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Height = 50;
            pnlButtons.BackColor = Color.FromArgb(240, 240, 240);
            pnlButtons.Padding = new Padding(10);

            btnReturnBook = new Button();
            btnReturnBook.Text = "Return Book";
            btnReturnBook.Size = new Size(120, 35);
            btnReturnBook.Location = new Point(10, 7);
            btnReturnBook.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnReturnBook.BackColor = Color.FromArgb(0, 153, 51);
            btnReturnBook.ForeColor = Color.White;
            btnReturnBook.FlatStyle = FlatStyle.Flat;
            btnReturnBook.FlatAppearance.BorderSize = 0;
            btnReturnBook.Click += BtnReturnBook_Click;

            Button btnRefreshBorrows = new Button();
            btnRefreshBorrows.Text = "Refresh";
            btnRefreshBorrows.Size = new Size(100, 35);
            btnRefreshBorrows.Location = new Point(140, 7);
            btnRefreshBorrows.Font = new Font("Segoe UI", 9F);
            btnRefreshBorrows.Click += BtnRefresh_Click;

            pnlButtons.Controls.AddRange(new Control[] { btnReturnBook, btnRefreshBorrows });

            // DataGridView
            dgvBorrows = new DataGridView();
            dgvBorrows.Dock = DockStyle.Fill;
            dgvBorrows.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBorrows.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBorrows.MultiSelect = false;
            dgvBorrows.ReadOnly = true;
            dgvBorrows.BackgroundColor = Color.White;
            dgvBorrows.BorderStyle = BorderStyle.None;
            dgvBorrows.Font = new Font("Segoe UI", 9F);
            dgvBorrows.RowHeadersVisible = false;
            dgvBorrows.AllowUserToAddRows = false;
            dgvBorrows.CellFormatting += DgvBorrows_CellFormatting;

            tab.Controls.Add(dgvBorrows);
            tab.Controls.Add(pnlButtons);
        }

        private void CreateStatisticsTab(TabPage tab)
        {
            lblStats = new Label();
            lblStats.Dock = DockStyle.Fill;
            lblStats.Font = new Font("Segoe UI", 11F);
            lblStats.Padding = new Padding(20);
            lblStats.AutoSize = false;
            lblStats.TextAlign = ContentAlignment.TopLeft;

            Button btnRefreshStats = new Button();
            btnRefreshStats.Text = "Refresh Statistics";
            btnRefreshStats.Size = new Size(150, 35);
            btnRefreshStats.Location = new Point(10, 10);
            btnRefreshStats.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRefreshStats.BackColor = Color.FromArgb(0, 120, 215);
            btnRefreshStats.ForeColor = Color.White;
            btnRefreshStats.FlatStyle = FlatStyle.Flat;
            btnRefreshStats.FlatAppearance.BorderSize = 0;
            btnRefreshStats.Click += async (s, e) => await LoadStatistics();

            Panel pnlStats = new Panel();
            pnlStats.Dock = DockStyle.Fill;
            pnlStats.Padding = new Padding(10);
            pnlStats.Controls.Add(lblStats);
            pnlStats.Controls.Add(btnRefreshStats);

            tab.Controls.Add(pnlStats);
        }

        private void InitializeDatabase()
        {
            _context = new LibraryDbContext();
            _context.Database.EnsureCreated();
        }

        private void InitializeServices()
        {
            _bookService = new BookService(_context);
        }

        private async void LoadData()
        {
            await LoadBooks();
            await LoadBorrows();
            await LoadStatistics();
        }

        private async Task LoadBooks(bool availableOnly = false)
        {
            try
            {
                var books = availableOnly 
                    ? await _bookService.GetAvailableBooksAsync()
                    : await _bookService.GetAllBooksAsync();
                    
                dgvBooks.DataSource = books.Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Author,
                    b.Year,
                    Status = b.IsAvailable ? "Available" : "Borrowed",
                    b.IsAvailable,
                    DateAdded = b.DateAdded?.ToString("yyyy-MM-dd") ?? "N/A"
                }).ToList();

                dgvBooks.Columns["Id"].Visible = false;
                dgvBooks.Columns["IsAvailable"].Visible = false;
                dgvBooks.Columns["Title"].HeaderText = "Title";
                dgvBooks.Columns["Author"].HeaderText = "Author";
                dgvBooks.Columns["Year"].HeaderText = "Year";
                dgvBooks.Columns["Status"].HeaderText = "Status";
                dgvBooks.Columns["DateAdded"].HeaderText = "Date Added";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadBorrows()
        {
            try
            {
                var borrows = await _bookService.GetAllBorrowRecordsAsync();
                dgvBorrows.DataSource = borrows.Select(b => new
                {
                    b.Id,
                    BookTitle = b.Book.Title,
                    BookAuthor = b.Book.Author,
                    BorrowerName = b.BorrowerName,
                    BorrowerEmail = b.BorrowerEmail ?? "N/A",
                    BorrowerPhone = b.BorrowerPhone ?? "N/A",
                    BorrowDate = b.BorrowDate.ToString("yyyy-MM-dd"),
                    DueDate = b.DueDate?.ToString("yyyy-MM-dd") ?? "N/A",
                    ReturnDate = b.ReturnDate?.ToString("yyyy-MM-dd") ?? "Not Returned",
                    Status = b.IsReturned ? "Returned" : (b.DueDate < DateTime.Now ? "Overdue" : "Active"),
                    b.IsReturned
                }).ToList();

                dgvBorrows.Columns["Id"].Visible = false;
                dgvBorrows.Columns["IsReturned"].Visible = false;
                dgvBorrows.Columns["BookTitle"].HeaderText = "Book Title";
                dgvBorrows.Columns["BookAuthor"].HeaderText = "Author";
                dgvBorrows.Columns["BorrowerName"].HeaderText = "Borrower";
                dgvBorrows.Columns["BorrowerEmail"].HeaderText = "Email";
                dgvBorrows.Columns["BorrowerPhone"].HeaderText = "Phone";
                dgvBorrows.Columns["BorrowDate"].HeaderText = "Borrow Date";
                dgvBorrows.Columns["DueDate"].HeaderText = "Due Date";
                dgvBorrows.Columns["ReturnDate"].HeaderText = "Return Date";
                dgvBorrows.Columns["Status"].HeaderText = "Status";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading borrows: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadStatistics()
        {
            try
            {
                var stats = await _bookService.GetStatisticsAsync();
                var sb = new StringBuilder();
                sb.AppendLine("═══════════════════════════════════════════════════");
                sb.AppendLine("           LIBRARY STATISTICS");
                sb.AppendLine("═══════════════════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Total Books:              {stats["TotalBooks"]}");
                sb.AppendLine($"Available Books:          {stats["AvailableBooks"]}");
                sb.AppendLine($"Borrowed Books:           {stats["BorrowedBooks"]}");
                sb.AppendLine($"Total Borrow Records:     {stats["TotalBorrows"]}");
                sb.AppendLine($"Active Borrows:           {stats["ActiveBorrows"]}");
                sb.AppendLine($"Overdue Borrows:          {stats["OverdueBorrows"]}");
                sb.AppendLine();
                sb.AppendLine("═══════════════════════════════════════════════════");

                lblStats.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvBooks_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvBooks.Columns[e.ColumnIndex].Name == "Status")
            {
                var row = dgvBooks.Rows[e.RowIndex];
                var isAvailable = (bool)row.Cells["IsAvailable"].Value;
                e.Value = isAvailable ? "Available" : "Borrowed";
                e.CellStyle.ForeColor = isAvailable ? Color.Green : Color.Red;
                e.CellStyle.Font = new Font(dgvBooks.Font, FontStyle.Bold);
            }
        }

        private void DgvBorrows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvBorrows.Columns[e.ColumnIndex].Name == "Status")
            {
                var row = dgvBorrows.Rows[e.RowIndex];
                var isReturned = (bool)row.Cells["IsReturned"].Value;
                var status = e.Value?.ToString() ?? "";
                
                if (status == "Returned")
                {
                    e.CellStyle.ForeColor = Color.Green;
                }
                else if (status == "Overdue")
                {
                    e.CellStyle.ForeColor = Color.Red;
                }
                else
                {
                    e.CellStyle.ForeColor = Color.Orange;
                }
                e.CellStyle.Font = new Font(dgvBorrows.Font, FontStyle.Bold);
            }
        }

        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Prevent recursive calls when updating from recommendations
            if (_isUpdatingSearch)
                return;

            var searchText = txtSearch.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                lstRecommendations.Visible = false;
                await LoadBooks(chkShowAvailableOnly.Checked);
                return;
            }

            try
            {
                // Get recommendations
                var recommendations = await _bookService.GetSearchRecommendationsAsync(searchText);
                if (recommendations.Any() && searchText.Length > 0)
                {
                    lstRecommendations.DataSource = recommendations;
                    lstRecommendations.Visible = true;
                    lstRecommendations.BringToFront();
                }
                else
                {
                    lstRecommendations.Visible = false;
                }

                // Perform search
                var books = await _bookService.SearchBooksAsync(searchText);
                
                // Apply available filter if checked
                if (chkShowAvailableOnly.Checked)
                {
                    books = books.Where(b => b.IsAvailable).ToList();
                }
                
                dgvBooks.DataSource = books.Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Author,
                    b.Year,
                    Status = b.IsAvailable ? "Available" : "Borrowed",
                    b.IsAvailable,
                    DateAdded = b.DateAdded?.ToString("yyyy-MM-dd") ?? "N/A"
                }).ToList();

                dgvBooks.Columns["Id"].Visible = false;
                dgvBooks.Columns["IsAvailable"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error performing search: {ex.Message}", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void LstRecommendations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRecommendations.SelectedItem != null)
            {
                _isUpdatingSearch = true;
                try
                {
                    var selectedText = lstRecommendations.SelectedItem.ToString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(selectedText))
                        return;
                        
                    txtSearch.Text = selectedText;
                    lstRecommendations.Visible = false;
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                    
                    // Perform search directly without triggering TextChanged
                    var books = await _bookService.SearchBooksAsync(selectedText);
                    
                    // Apply available filter if checked
                    if (chkShowAvailableOnly.Checked)
                    {
                        books = books.Where(b => b.IsAvailable).ToList();
                    }
                    
                    dgvBooks.DataSource = books.Select(b => new
                    {
                        b.Id,
                        b.Title,
                        b.Author,
                        b.Year,
                        Status = b.IsAvailable ? "Available" : "Borrowed",
                        b.IsAvailable,
                        DateAdded = b.DateAdded?.ToString("yyyy-MM-dd") ?? "N/A"
                    }).ToList();

                    dgvBooks.Columns["Id"].Visible = false;
                    dgvBooks.Columns["IsAvailable"].Visible = false;
                }
                finally
                {
                    _isUpdatingSearch = false;
                }
            }
        }

        private void BtnAddBook_Click(object sender, EventArgs e)
        {
            using (var form = new BookForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private async void BtnEditBook_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var bookId = (int)dgvBooks.SelectedRows[0].Cells["Id"].Value;
            var book = await _bookService.GetBookByIdAsync(bookId);
            if (book != null)
            {
                using (var form = new BookForm(book))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
        }

        private async void BtnDeleteBook_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var bookId = (int)dgvBooks.SelectedRows[0].Cells["Id"].Value;
            var book = await _bookService.GetBookByIdAsync(bookId);
            
            if (book != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete '{book.Title}'?", "Confirm Delete", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var result = await _bookService.DeleteBookAsync(bookId);
                    if (result)
                    {
                        MessageBox.Show("Book deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Error deleting book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void BtnBorrowBook_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book to borrow.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var bookId = (int)dgvBooks.SelectedRows[0].Cells["Id"].Value;
            var book = await _bookService.GetBookByIdAsync(bookId);
            
            if (book == null)
            {
                MessageBox.Show("Book not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!book.IsAvailable)
            {
                MessageBox.Show("This book is currently borrowed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new BorrowForm(book))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private async void BtnReturnBook_Click(object sender, EventArgs e)
        {
            if (dgvBorrows.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a borrow record to return.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var recordId = (int)dgvBorrows.SelectedRows[0].Cells["Id"].Value;
            var isReturned = (bool)dgvBorrows.SelectedRows[0].Cells["IsReturned"].Value;

            if (isReturned)
            {
                MessageBox.Show("This book has already been returned.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to return this book?", "Confirm Return", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var result = await _bookService.ReturnBookAsync(recordId);
                if (result)
                {
                    MessageBox.Show("Book returned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Error returning book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void BtnStatistics_Click(object sender, EventArgs e)
        {
            tabControl.SelectedIndex = 2;
        }

        private async void ChkShowAvailableOnly_CheckedChanged(object sender, EventArgs e)
        {
            await LoadBooks(chkShowAvailableOnly.Checked);
        }

        private async void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    saveDialog.FileName = $"LibraryData_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        var basePath = Path.GetDirectoryName(saveDialog.FileName);
                        var baseName = Path.GetFileNameWithoutExtension(saveDialog.FileName);
                        var extension = Path.GetExtension(saveDialog.FileName);
                        
                        // Export Books
                        var booksPath = Path.Combine(basePath, $"{baseName}_Books{extension}");
                        var books = await _bookService.GetAllBooksAsync();
                        var booksSb = new StringBuilder();
                        booksSb.AppendLine("Title,Author,Year");
                        foreach (var book in books)
                        {
                            booksSb.AppendLine($"\"{book.Title}\",\"{book.Author}\",{book.Year}");
                        }
                        File.WriteAllText(booksPath, booksSb.ToString());
                        
                        // Export Borrow Records
                        var borrowsPath = Path.Combine(basePath, $"{baseName}_Borrows{extension}");
                        var borrows = await _bookService.GetAllBorrowRecordsAsync();
                        var borrowsSb = new StringBuilder();
                        borrowsSb.AppendLine("BookTitle,BorrowerName,Email,Phone,BorrowDate,DueDate,ReturnDate,IsReturned,Notes");
                        foreach (var borrow in borrows)
                        {
                            borrowsSb.AppendLine($"\"{borrow.Book.Title}\",\"{borrow.BorrowerName}\",\"{borrow.BorrowerEmail ?? ""}\",\"{borrow.BorrowerPhone ?? ""}\",\"{borrow.BorrowDate:yyyy-MM-dd}\",\"{borrow.DueDate:yyyy-MM-dd}\",\"{borrow.ReturnDate?.ToString("yyyy-MM-dd") ?? ""}\",\"{(borrow.IsReturned ? "Yes" : "No")}\",\"{borrow.Notes ?? ""}\"");
                        }
                        File.WriteAllText(borrowsPath, borrowsSb.ToString());
                        
                        MessageBox.Show($"Data exported successfully!\n\nBooks: {booksPath}\nBorrow Records: {borrowsPath}", 
                            "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openDialog = new OpenFileDialog())
                {
                    openDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    openDialog.Title = "Select CSV file to import (Books, Borrows, or Returns)";
                    openDialog.Multiselect = false;
                    
                    if (openDialog.ShowDialog() == DialogResult.OK)
                    {
                        var result = await _bookService.ImportFromCsvAsync(openDialog.FileName);
                        
                        var message = new StringBuilder();
                        message.AppendLine("Import completed!");
                        message.AppendLine($"Books imported: {result.booksImported}");
                        message.AppendLine($"Borrow records imported: {result.borrowsImported}");
                        message.AppendLine($"Returns processed: {result.returnsProcessed}");
                        
                        if (result.errors.Any())
                        {
                            message.AppendLine();
                            message.AppendLine($"Errors ({result.errors.Count}):");
                            foreach (var error in result.errors.Take(10))
                            {
                                message.AppendLine($"  - {error}");
                            }
                            if (result.errors.Count > 10)
                            {
                                message.AppendLine($"  ... and {result.errors.Count - 10} more errors");
                            }
                            
                            MessageBox.Show(message.ToString(), "Import Results", MessageBoxButtons.OK, 
                                result.errors.Count > result.booksImported + result.borrowsImported + result.returnsProcessed
                                    ? MessageBoxIcon.Warning 
                                    : MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(message.ToString(), "Import Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _context?.Dispose();
            base.OnFormClosing(e);
        }
    }
}



