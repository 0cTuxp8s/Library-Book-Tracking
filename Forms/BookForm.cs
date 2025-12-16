using System;
using System.Windows.Forms;
using LibraryBookTracking.Models;
using LibraryBookTracking.Services;
using LibraryBookTracking.Data;

namespace LibraryBookTracking.Forms
{
    public partial class BookForm : Form
    {
        private TextBox txtTitle;
        private TextBox txtAuthor;
        private NumericUpDown numYear;
        private Button btnSave;
        private Button btnCancel;
        private Book? _book;
        private BookService _bookService;
        private LibraryDbContext _context;

        public BookForm(Book? book = null)
        {
            _book = book;
            _context = new LibraryDbContext();
            _bookService = new BookService(_context);
            InitializeComponent();
            
            if (_book != null)
            {
                LoadBookData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = _book == null ? "Add New Book" : "Edit Book";
            this.Size = new Size(500, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "Title:";
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(100, 23);
            lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            txtTitle = new TextBox();
            txtTitle.Location = new Point(130, 18);
            txtTitle.Size = new Size(330, 25);
            txtTitle.Font = new Font("Segoe UI", 10F);

            // Author
            Label lblAuthor = new Label();
            lblAuthor.Text = "Author:";
            lblAuthor.Location = new Point(20, 60);
            lblAuthor.Size = new Size(100, 23);
            lblAuthor.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            txtAuthor = new TextBox();
            txtAuthor.Location = new Point(130, 58);
            txtAuthor.Size = new Size(330, 25);
            txtAuthor.Font = new Font("Segoe UI", 10F);

            // Year
            Label lblYear = new Label();
            lblYear.Text = "Year:";
            lblYear.Location = new Point(20, 100);
            lblYear.Size = new Size(100, 23);
            lblYear.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            numYear = new NumericUpDown();
            numYear.Location = new Point(130, 98);
            numYear.Size = new Size(150, 25);
            numYear.Minimum = 1000;
            numYear.Maximum = DateTime.Now.Year;
            numYear.Value = DateTime.Now.Year;
            numYear.Font = new Font("Segoe UI", 10F);

            // Buttons
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new Point(280, 150);
            btnSave.Size = new Size(85, 35);
            btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(0, 120, 215);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(375, 150);
            btnCancel.Size = new Size(85, 35);
            btnCancel.Font = new Font("Segoe UI", 9F);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] {
                lblTitle, txtTitle,
                lblAuthor, txtAuthor,
                lblYear, numYear,
                btnSave, btnCancel
            });

            this.ResumeLayout(false);
        }

        private void LoadBookData()
        {
            if (_book != null)
            {
                txtTitle.Text = _book.Title;
                txtAuthor.Text = _book.Author;
                numYear.Value = _book.Year;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Please enter an author.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAuthor.Focus();
                return;
            }

            try
            {
                bool result;
                if (_book == null)
                {
                    // Add new book
                    var newBook = new Book
                    {
                        Title = txtTitle.Text.Trim(),
                        Author = txtAuthor.Text.Trim(),
                        Year = (int)numYear.Value,
                        IsAvailable = true,
                        DateAdded = DateTime.Now
                    };
                    result = await _bookService.AddBookAsync(newBook);
                }
                else
                {
                    // Update existing book
                    _book.Title = txtTitle.Text.Trim();
                    _book.Author = txtAuthor.Text.Trim();
                    _book.Year = (int)numYear.Value;
                    result = await _bookService.UpdateBookAsync(_book);
                }

                if (result)
                {
                    MessageBox.Show(_book == null ? "Book added successfully!" : "Book updated successfully!", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error saving book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _context?.Dispose();
            base.OnFormClosing(e);
        }
    }
}

