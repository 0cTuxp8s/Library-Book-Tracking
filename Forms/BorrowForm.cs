using System;
using System.Windows.Forms;
using LibraryBookTracking.Models;
using LibraryBookTracking.Services;
using LibraryBookTracking.Data;

namespace LibraryBookTracking.Forms
{
    public partial class BorrowForm : Form
    {
        private Label lblBookInfo;
        private TextBox txtBorrowerName;
        private TextBox txtBorrowerEmail;
        private TextBox txtBorrowerPhone;
        private DateTimePicker dtpDueDate;
        private TextBox txtNotes;
        private Button btnSave;
        private Button btnCancel;
        private Book _book;
        private BookService _bookService;
        private LibraryDbContext _context;

        public BorrowForm(Book book)
        {
            _book = book;
            _context = new LibraryDbContext();
            _bookService = new BookService(_context);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Borrow Book";
            this.Size = new Size(500, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Book Info
            lblBookInfo = new Label();
            lblBookInfo.Text = $"Book: {_book.Title}\nAuthor: {_book.Author}\nYear: {_book.Year}";
            lblBookInfo.Location = new Point(20, 20);
            lblBookInfo.Size = new Size(450, 60);
            lblBookInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBookInfo.BackColor = Color.FromArgb(240, 240, 240);
            lblBookInfo.Padding = new Padding(10);
            lblBookInfo.BorderStyle = BorderStyle.FixedSingle;

            // Borrower Name
            Label lblBorrowerName = new Label();
            lblBorrowerName.Text = "Borrower Name:";
            lblBorrowerName.Location = new Point(20, 100);
            lblBorrowerName.Size = new Size(120, 23);
            lblBorrowerName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            txtBorrowerName = new TextBox();
            txtBorrowerName.Location = new Point(150, 98);
            txtBorrowerName.Size = new Size(320, 25);
            txtBorrowerName.Font = new Font("Segoe UI", 10F);

            // Borrower Email
            Label lblBorrowerEmail = new Label();
            lblBorrowerEmail.Text = "Email:";
            lblBorrowerEmail.Location = new Point(20, 140);
            lblBorrowerEmail.Size = new Size(120, 23);
            lblBorrowerEmail.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            txtBorrowerEmail = new TextBox();
            txtBorrowerEmail.Location = new Point(150, 138);
            txtBorrowerEmail.Size = new Size(320, 25);
            txtBorrowerEmail.Font = new Font("Segoe UI", 10F);

            // Borrower Phone
            Label lblBorrowerPhone = new Label();
            lblBorrowerPhone.Text = "Phone:";
            lblBorrowerPhone.Location = new Point(20, 180);
            lblBorrowerPhone.Size = new Size(120, 23);
            lblBorrowerPhone.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            txtBorrowerPhone = new TextBox();
            txtBorrowerPhone.Location = new Point(150, 178);
            txtBorrowerPhone.Size = new Size(320, 25);
            txtBorrowerPhone.Font = new Font("Segoe UI", 10F);

            // Due Date
            Label lblDueDate = new Label();
            lblDueDate.Text = "Due Date:";
            lblDueDate.Location = new Point(20, 220);
            lblDueDate.Size = new Size(120, 23);
            lblDueDate.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            dtpDueDate = new DateTimePicker();
            dtpDueDate.Location = new Point(150, 218);
            dtpDueDate.Size = new Size(200, 25);
            dtpDueDate.Format = DateTimePickerFormat.Short;
            dtpDueDate.Value = DateTime.Now.AddDays(14);
            dtpDueDate.Font = new Font("Segoe UI", 10F);

            // Notes
            Label lblNotes = new Label();
            lblNotes.Text = "Notes:";
            lblNotes.Location = new Point(20, 260);
            lblNotes.Size = new Size(120, 23);
            lblNotes.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            txtNotes = new TextBox();
            txtNotes.Location = new Point(150, 258);
            txtNotes.Size = new Size(320, 25);
            txtNotes.Font = new Font("Segoe UI", 10F);
            txtNotes.Multiline = false;

            // Buttons
            btnSave = new Button();
            btnSave.Text = "Borrow";
            btnSave.Location = new Point(280, 300);
            btnSave.Size = new Size(85, 35);
            btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(0, 153, 51);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(375, 300);
            btnCancel.Size = new Size(85, 35);
            btnCancel.Font = new Font("Segoe UI", 9F);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] {
                lblBookInfo,
                lblBorrowerName, txtBorrowerName,
                lblBorrowerEmail, txtBorrowerEmail,
                lblBorrowerPhone, txtBorrowerPhone,
                lblDueDate, dtpDueDate,
                lblNotes, txtNotes,
                btnSave, btnCancel
            });

            this.ResumeLayout(false);
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBorrowerName.Text))
            {
                MessageBox.Show("Please enter borrower name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBorrowerName.Focus();
                return;
            }

            try
            {
                var result = await _bookService.BorrowBookAsync(
                    _book.Id,
                    txtBorrowerName.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtBorrowerEmail.Text) ? null : txtBorrowerEmail.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtBorrowerPhone.Text) ? null : txtBorrowerPhone.Text.Trim(),
                    dtpDueDate.Value,
                    string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim()
                );

                if (result)
                {
                    MessageBox.Show("Book borrowed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error borrowing book. The book may no longer be available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

