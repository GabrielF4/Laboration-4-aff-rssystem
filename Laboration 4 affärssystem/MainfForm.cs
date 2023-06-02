using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboration_4_affärssystem
{
    public partial class MainForm : Form
    {
        const int BOOKS = 0;
        const int MOVIES = 1;
        const int GAMES = 2;

        BindingList<Book> BookList;
        BindingList<Movie> MovieList;
        BindingList<Game> GameList;

        //List for displaying cart
        BindingList<CartItem> CartList;

        //List for storing productinformation in cart
        List<Product> CartBuffert;

        int priceCartBuffer = 0;
        int productIDCounter = FileHandler.GetProductIDCounter();
        int selectedRowIndex;

        //Constructor
        public MainForm()
        {
            InitializeComponent();

            BookList = new BindingList<Book>();
            MovieList = new BindingList<Movie>();
            GameList = new BindingList<Game>();
            CartList = new BindingList<CartItem>();

            CartBuffert = new List<Product>();

            bookGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            bookGridView.MultiSelect = false;
            bookGridView.AllowUserToAddRows = false;
            bookGridView.AllowUserToDeleteRows = false;
            bookGridView.ReadOnly = true;
            bookGridView.DataSource = BookList;

            movieGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            movieGridView.MultiSelect = false;
            movieGridView.AllowUserToAddRows = false;
            movieGridView.AllowUserToDeleteRows = false;
            movieGridView.ReadOnly = true;
            movieGridView.DataSource = MovieList;

            gameGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gameGridView.MultiSelect = false;
            gameGridView.AllowUserToAddRows = false;
            gameGridView.AllowUserToDeleteRows = false;
            gameGridView.DataSource = GameList;
            gameGridView.ReadOnly = true;

            dataGridViewCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewCart.MultiSelect = false;
            dataGridViewCart.DataSource = CartList;

        }

        //Storage handling -----------------------------------------------------------------------------------------------------------------------

        //Adding items to database with the indata panel--------------------------
        private void addBookBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1B.Text) || string.IsNullOrEmpty(textBox2B.Text))
            {
                MessageBox.Show("Fyll i obligatorisk indata!");
                return;
            }

            if (!int.TryParse(textBox2B.Text, out int price) || price < 0)
            {
                MessageBox.Show("Ange giltigt pris!");
                return;
            }

            //Increase stock if Items alread exist
            foreach(Book book in BookList)
            {
                if(book.Name == textBox1B.Text)
                {
                    MessageBox.Show("Finns redan bok med följande namn! (Ökar antalet i lager)");
                    book.Stock += (int)numericB.Value;
                    bookGridView.Refresh();
                    return;
                }
            }

            BookList.Add(new Book()
            {
                Name = textBox1B.Text,
                Price = int.Parse(textBox2B.Text),
                Author = textBox3B.Text,
                Genre = textBox4B.Text,
                Format = textBox5B.Text,
                Language = textBox6B.Text,
                ID = ++productIDCounter,
                Stock = (int)numericB.Value

            });
        }
        private void addMovieBtn_Click(object sender, EventArgs e)
        {
            //Abort if name or price field is empty
            if (string.IsNullOrEmpty(textBox1F.Text) || string.IsNullOrEmpty(textBox2F.Text))
            {
                MessageBox.Show("Fyll i obligatorisk indata!");
                return;
            }

            //Abort if incorrect input
            if (!int.TryParse(textBox2F.Text, out int price) || price < 0)
            {
                MessageBox.Show("Ange giltigt pris!");
                return;
            }

            if (!int.TryParse(textBox4F.Text, out int movieLength) || movieLength <= 0)
            {
                MessageBox.Show("Ange giltig speltid för film!");
                return;
            }

            //Increase stock if Items alread exist
            foreach (Movie movie in MovieList)
            {
                if (movie.Name == textBox1F.Text)
                {
                    MessageBox.Show("Finns redan film med följande namn! (Ökar antalet i lager)");
                    movie.Stock += (int)numericF.Value;
                    movieGridView.Refresh();
                    return;
                }
            }

            MovieList.Add(new Movie()
            {
                Name = textBox1F.Text,
                Price = price,
                Format = textBox3F.Text,
                Playtime = movieLength + " min",
                ID = ++productIDCounter,
                Stock = (int)numericF.Value

            });
        }
        private void addGameBtn_Click(object sender, EventArgs e)
        {
            //Abort if name or price field is empty
            if (string.IsNullOrEmpty(textBox1G.Text) || string.IsNullOrEmpty(textBox2G.Text))
            {
                MessageBox.Show("Fyll i obligatorisk indata!");
                return;
            }

            //Abort if incorrect input
            if (!int.TryParse(textBox2G.Text, out int price) || price < 0)
            {
                MessageBox.Show("Ange giltigt pris!");
                return;
            }

            //Increase stock if Items alread exist
            foreach (Game game in GameList)
            {
                if (game.Name == textBox1G.Text)
                {
                    MessageBox.Show("Finns redan film med följande namn! (Ökar antalet i lager)");
                    game.Stock += (int)numericG.Value;
                    gameGridView.Refresh();
                    return;
                }
            }

            GameList.Add(new Game()
            {
                Name = textBox1G.Text,
                Price = int.Parse(textBox2G.Text),
                Platform = textBox3G.Text,
                ID = ++productIDCounter,
                Stock = (int)numericG.Value

            });
        }
        //------------------------------------------------------------------------

        //Increase stock of Item
        private void increaseStockBtn_Click(object sender, EventArgs e)
        {
            if (addItemTabControl.SelectedIndex == BOOKS && bookGridView.SelectedRows.Count > 0)
            {
                int rowIndex = bookGridView.SelectedRows[0].Index;
                BookList[rowIndex].Stock++;
                bookGridView.Refresh();

            }
            else if (addItemTabControl.SelectedIndex == MOVIES && movieGridView.SelectedRows.Count > 0)
            {
                int rowIndex = movieGridView.SelectedRows[0].Index;
                MovieList[rowIndex].Stock++;
                movieGridView.Refresh();

            }
            else if (addItemTabControl.SelectedIndex == GAMES && gameGridView.SelectedRows.Count > 0)
            {
                int rowIndex = gameGridView.SelectedRows[0].Index;
                GameList[rowIndex].Stock++;
                gameGridView.Refresh();
            }
        }
        //Decrease stock of Item
        private void decreaseStockBtn_Click(object sender, EventArgs e)
        {

            if (addItemTabControl.SelectedIndex == BOOKS && bookGridView.SelectedRows.Count > 0)
            {
                int rowIndex = bookGridView.SelectedRows[0].Index;

                if (BookList[rowIndex].Stock > 1)
                {
                    BookList[rowIndex].Stock--;
                    bookGridView.Refresh();
                }
                else
                {
                    DialogResult result = MessageBox.Show("Är du säker på att du vill ta bort varan?", "Säkerhetskontroll", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }

                    BookList.RemoveAt(rowIndex);
                }

            }
            else if (addItemTabControl.SelectedIndex == MOVIES && movieGridView.SelectedRows.Count > 0)
            {
                int rowIndex = movieGridView.SelectedRows[0].Index;

                if (MovieList[rowIndex].Stock > 1)
                {
                    MovieList[rowIndex].Stock--;
                    movieGridView.Refresh();
                }
                else
                {
                    DialogResult result = MessageBox.Show("Är du säker på att du vill ta bort varan?", "Säkerhetskontroll", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }

                    MovieList.RemoveAt(rowIndex);
                }

            }
            else if (addItemTabControl.SelectedIndex == GAMES && gameGridView.SelectedRows.Count > 0)
            {
                int rowIndex = gameGridView.SelectedRows[0].Index;

                if (GameList[rowIndex].Stock > 1)
                {
                    GameList[rowIndex].Stock--;
                    gameGridView.Refresh();

                }
                else
                {
                    DialogResult result = MessageBox.Show("Är du säker på att du vill ta bort varan?", "Säkerhetskontroll", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }

                    GameList.RemoveAt(rowIndex);
                }
            }
        }

        //Cart handeling -----------------------------------------------------------------------------------------------------------------------
        private void addToCartBtn_Click(object sender, EventArgs e)
        {
            bool productItemExist = false;
            CartItem currentItem = null;
            Product currentProduct = null;

            if (addItemTabControl.SelectedIndex == BOOKS && bookGridView.SelectedRows.Count > 0)
            {
                selectedRowIndex = bookGridView.SelectedRows[0].Index;

                //Update checkout price

                priceCartBuffer += BookList[selectedRowIndex].Price;
                priceLabel.Text = priceCartBuffer.ToString();

                //Selecting product if product alread exist in cart

                foreach (Product product in CartBuffert)
                {
                    if (product.Name == BookList[selectedRowIndex].Name)
                    {
                        productItemExist = true;
                        currentProduct = product;
                        currentItem = CartList[CartBuffert.IndexOf(product)];
                        break;
                    }
                }

                if (!productItemExist)
                {
                    //Add product to cart

                    CartBuffert.Add(new Product()
                    {
                        Type = "Book",
                        Name = BookList[selectedRowIndex].Name,
                        Price = BookList[selectedRowIndex].Price,
                        Author = BookList[selectedRowIndex].Author,
                        Genre = BookList[selectedRowIndex].Genre,
                        Format = BookList[selectedRowIndex].Format,
                        Language = BookList[selectedRowIndex].Language,
                        ID = BookList[selectedRowIndex].ID,
                        Stock = 1

                    });

                    CartList.Add(new CartItem()
                    {
                        Name = BookList[selectedRowIndex].Name,
                        Price = BookList[selectedRowIndex].Price,
                        ID = BookList[selectedRowIndex].ID,
                        Stock = 1
                    });

                    //Decrease stock or remove items from storage

                    BookList[selectedRowIndex].Stock--;

                    if(BookList[selectedRowIndex].Stock < 1)
                    {
                        BookList.RemoveAt(selectedRowIndex);
                    }
                }
                else
                {

                    currentItem.Stock++;
                    currentProduct.Stock++;

                    //Decrease stock or remove items from storage

                    BookList[selectedRowIndex].Stock--;

                    if (BookList[selectedRowIndex].Stock < 1)
                    {
                        BookList.RemoveAt(selectedRowIndex);
                    }
                }

                bookGridView.Refresh();

            }else if (addItemTabControl.SelectedIndex == MOVIES && movieGridView.SelectedRows.Count > 0)
            {
                selectedRowIndex = movieGridView.SelectedRows[0].Index;

                //Update checkout price

                priceCartBuffer += MovieList[selectedRowIndex].Price;
                priceLabel.Text = priceCartBuffer.ToString();

                //Selecting product if product alread exist in cart

                foreach (Product product in CartBuffert)
                {
                    if (product.Name == MovieList[selectedRowIndex].Name)
                    {
                        productItemExist = true;
                        currentProduct = product;
                        currentItem = CartList[CartBuffert.IndexOf(product)];
                        break;
                    }
                }

                if (!productItemExist)
                {
                    //Add product to cart

                    CartBuffert.Add(new Product()
                    {
                        Type = "Movie",
                        Name = MovieList[selectedRowIndex].Name,
                        Price = MovieList[selectedRowIndex].Price,
                        Format = MovieList[selectedRowIndex].Format,
                        Playtime = MovieList[selectedRowIndex].Playtime,
                        ID = MovieList[selectedRowIndex].ID,
                        Stock = 1

                    });

                    CartList.Add(new CartItem()
                    {
                        Name = MovieList[selectedRowIndex].Name,
                        Price = MovieList[selectedRowIndex].Price,
                        ID = MovieList[selectedRowIndex].ID,
                        Stock = 1
                    });

                    //Decrease stock or remove items from storage

                    MovieList[selectedRowIndex].Stock--;

                    if (MovieList[selectedRowIndex].Stock < 1)
                    {
                        MovieList.RemoveAt(selectedRowIndex);
                    }
                }
                else
                {

                    currentItem.Stock++;
                    currentProduct.Stock++;

                    //Decrease stock or remove items from storage

                    MovieList[selectedRowIndex].Stock--;

                    if (MovieList[selectedRowIndex].Stock < 1)
                    {
                        MovieList.RemoveAt(selectedRowIndex);
                    }
                }

                movieGridView.Refresh();

            }
            else if (addItemTabControl.SelectedIndex == GAMES && gameGridView.SelectedRows.Count > 0)
            {
                selectedRowIndex = gameGridView.SelectedRows[0].Index;

                //Update checkout price

                priceCartBuffer += GameList[selectedRowIndex].Price;
                priceLabel.Text = priceCartBuffer.ToString();

                //Selecting product if product alread exist in cart

                foreach (Product product in CartBuffert)
                {
                    if (product.Name == GameList[selectedRowIndex].Name)
                    {
                        productItemExist = true;
                        currentProduct = product;
                        currentItem = CartList[CartBuffert.IndexOf(product)];
                        break;
                    }
                }

                if (!productItemExist)
                {
                    //Add product to cart

                    CartBuffert.Add(new Product()
                    {
                        Type = "Game",
                        Name = GameList[selectedRowIndex].Name,
                        Price = GameList[selectedRowIndex].Price,
                        Platform = GameList[selectedRowIndex].Platform,
                        ID = GameList[selectedRowIndex].ID,
                        Stock = 1

                    });

                    CartList.Add(new CartItem()
                    {
                        Name = GameList[selectedRowIndex].Name,
                        Price = GameList[selectedRowIndex].Price,
                        ID = GameList[selectedRowIndex].ID,
                        Stock = 1
                    });

                    //Decrease stock or remove items from storage

                    GameList[selectedRowIndex].Stock--;

                    if (GameList[selectedRowIndex].Stock < 1)
                    {
                        GameList.RemoveAt(selectedRowIndex);
                    }
                }
                else
                {

                    currentItem.Stock++;
                    currentProduct.Stock++;

                    //Decrease stock or remove items from storage

                    GameList[selectedRowIndex].Stock--;

                    if (GameList[selectedRowIndex].Stock < 1)
                    {
                        GameList.RemoveAt(selectedRowIndex);
                    }
                }

                gameGridView.Refresh();
            }

            dataGridViewCart.Refresh();
        }
        private void removeFromCartBtn_Click(object sender, EventArgs e)
        {
            if (dataGridViewCart.SelectedRows.Count == 0) { return; }

            selectedRowIndex = dataGridViewCart.SelectedRows[0].Index;

            bool productItemExist = false;
            Product currentProduct = CartBuffert[selectedRowIndex];
            CartItem currentItem = CartList[selectedRowIndex];

            //Update checkout price
            priceCartBuffer -= CartBuffert[selectedRowIndex].Price;
            priceLabel.Text = priceCartBuffer.ToString();

            if (CartBuffert[selectedRowIndex].Type == "Book")
            {
                Book currentBook = null;

                //Selecting product if storage isn't out of stock
                foreach (Book book in BookList)
                {
                    if (book.Name == CartBuffert[selectedRowIndex].Name)
                    {
                        productItemExist = true;
                        currentBook = book;
                        break;
                    }
                }

                if (productItemExist)
                {
                    currentBook.Stock++;

                    //Remove or decrease stock
                    if (currentItem.Stock <= 1)
                    {
                        CartBuffert.RemoveAt(selectedRowIndex);
                        CartList.RemoveAt(selectedRowIndex);
                    }
                    else
                    {
                        currentProduct.Stock--;
                        currentItem.Stock--;
                    }
                }
                else
                {
                    //Add product back into storage if element was removed
                    BookList.Add(new Book()
                    {
                        Name = CartBuffert[selectedRowIndex].Name,
                        Price = CartBuffert[selectedRowIndex].Price,
                        Author = CartBuffert[selectedRowIndex].Author,
                        Genre = CartBuffert[selectedRowIndex].Genre,
                        Format = CartBuffert[selectedRowIndex].Format,
                        Language = CartBuffert[selectedRowIndex].Language,
                        ID = CartBuffert[selectedRowIndex].ID,
                        Stock = 1

                    });

                    //Remove or decrease stock
                    if (currentItem.Stock <= 1)
                    {
                        CartBuffert.RemoveAt(selectedRowIndex);
                        CartList.RemoveAt(selectedRowIndex);
                    }
                    else
                    {
                        currentProduct.Stock--;
                        currentItem.Stock--;
                    }
                }

                bookGridView.Refresh();

            }
            else if (CartBuffert[selectedRowIndex].Type == "Movie")
            {
                Movie currentMovie = null;

                //Selecting product if storage isn't out of stock
                foreach (Movie movie in MovieList)
                {
                    if (movie.Name == CartBuffert[selectedRowIndex].Name)
                    {
                        productItemExist = true;
                        currentMovie = movie;
                        break;
                    }
                }

                if (productItemExist)
                {
                    currentMovie.Stock++;

                    //Remove or decrease stock
                    if (currentItem.Stock <= 1)
                    {
                        CartBuffert.RemoveAt(selectedRowIndex);
                        CartList.RemoveAt(selectedRowIndex);
                    }
                    else
                    {
                        currentProduct.Stock--;
                        currentItem.Stock--;
                    }
                }
                else
                {
                    //Add product back into storage if element was removed
                    MovieList.Add(new Movie()
                    {
                        Name = CartBuffert[selectedRowIndex].Name,
                        Price = CartBuffert[selectedRowIndex].Price,
                        Format = CartBuffert[selectedRowIndex].Format,
                        Playtime = CartBuffert[selectedRowIndex].Playtime,
                        ID = CartBuffert[selectedRowIndex].ID,
                        Stock = 1

                    });

                    //Remove or decrease stock
                    if (currentItem.Stock <= 1)
                    {
                        CartBuffert.RemoveAt(selectedRowIndex);
                        CartList.RemoveAt(selectedRowIndex);
                    }
                    else
                    {
                        currentProduct.Stock--;
                        currentItem.Stock--;
                    }
                }

                movieGridView.Refresh();


            }
            else if (CartBuffert[selectedRowIndex].Type == "Game")
            {
                Game currentGame = null;

                //Selecting product if storage isn't out of stock
                foreach (Game game in GameList)
                {
                    if (game.Name == CartBuffert[selectedRowIndex].Name)
                    {
                        productItemExist = true;
                        currentGame = game;
                        break;
                    }
                }

                if (productItemExist)
                {
                    currentGame.Stock++;

                    //Remove or decrease stock
                    if (currentItem.Stock <= 1)
                    {
                        CartBuffert.RemoveAt(selectedRowIndex);
                        CartList.RemoveAt(selectedRowIndex);
                    }
                    else
                    {
                        currentProduct.Stock--;
                        currentItem.Stock--;
                    }
                }
                else
                {
                    //Add product back into storage if element was removed
                    GameList.Add(new Game()
                    {
                        Name = CartBuffert[selectedRowIndex].Name,
                        Price = CartBuffert[selectedRowIndex].Price,
                        Platform = CartBuffert[selectedRowIndex].Platform,
                        ID = CartBuffert[selectedRowIndex].ID,
                        Stock = 1

                    });

                    //Remove or decrease stock
                    if (currentItem.Stock <= 1)
                    {
                        CartBuffert.RemoveAt(selectedRowIndex);
                        CartList.RemoveAt(selectedRowIndex);
                    }
                    else
                    {
                        currentProduct.Stock--;
                        currentItem.Stock--;
                    }
                }

                gameGridView.Refresh();

            }

            dataGridViewCart.Refresh();

        }
        private void buyBtn_Click(object sender, EventArgs e)
        {
            if (CartList.Count == 0) { return; }

            MessageBox.Show($"Total Cost: {priceCartBuffer} kr\n\nThe purchase was successful!");

            CartList.Clear();
            CartBuffert.Clear();
            priceCartBuffer = 0;
            priceLabel.Text = "-";

        }
        private void resetCartBtn_Click(object sender, EventArgs e)
        {
            resetCart();
            bookGridView.Refresh();
            movieGridView.Refresh();
            gameGridView.Refresh();
        }

        //Tabcontrol ---------------------------------------------------------------------------------------------------------------------------
        private void addItemTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (addItemTabControl.SelectedIndex == BOOKS)
            {
                theTabControl.SelectedIndex = BOOKS;

            }
            else if (addItemTabControl.SelectedIndex == MOVIES)
            {
                theTabControl.SelectedIndex = MOVIES;

            }
            else if (addItemTabControl.SelectedIndex == GAMES)
            {
                theTabControl.SelectedIndex = GAMES;

            }
        }
        private void theTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (theTabControl.SelectedIndex == BOOKS)
            {
                addItemTabControl.SelectedIndex = BOOKS;

            }
            else if (theTabControl.SelectedIndex == MOVIES)
            {
                addItemTabControl.SelectedIndex = MOVIES;

            }
            else if (theTabControl.SelectedIndex == GAMES)
            {
                addItemTabControl.SelectedIndex = GAMES;

            }
        }

        //File Handling ------------------------------------------------------------------------------------------------------------------------
        private void saveBtn_Click(object sender, EventArgs e)
        {
            FileHandler.SaveToFile(BookList, MovieList, GameList, productIDCounter);
            MessageBox.Show("The Tables Have Been Saved To The Database! :)");
        }
        private void loadBtn_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Är du säker du vill återställa från fil utan att spara?", "?!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if(result == DialogResult.Yes)
            {
                FileHandler.LoadBooks(BookList);
                FileHandler.LoadMovies(MovieList);
                FileHandler.LoadGames(GameList);

                clearCart();
            }
        }
        private void EditTableForm_Load(object sender, EventArgs e)
        {
            FileHandler.LoadBooks(BookList);
            FileHandler.LoadMovies(MovieList);
            FileHandler.LoadGames(GameList);
        }
        private void EditTableForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            DialogResult result = MessageBox.Show("Vill du spara?", "Spara?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                resetCart();
                FileHandler.SaveToFile(BookList, MovieList, GameList, productIDCounter);

            }
            else if(result == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
        }


        //Custom Functions --------------------------------------------------------------------------------------------------------------------
        public void resetCart()
        {
            //Clear Cart
            if (CartBuffert.Count != 0)
            {
                foreach (var item in CartBuffert)
                {
                    bool productItemExist = false;

                    if (item.Type == "Book")
                    {
                        Book currentBook = null;

                        //Checking for similar element in storage

                        foreach(Book book in BookList)
                        {
                            if(book.Name == item.Name)
                            {
                                productItemExist = true;
                                currentBook = book;
                                break;
                            }
                        }

                        //Adding up stock or adding back product to storage

                        if (productItemExist)
                        {
                            currentBook.Stock += item.Stock;
                        }
                        else
                        {
                            BookList.Add(new Book()
                            {
                                Name = item.Name,
                                Price = item.Price,
                                Author = item.Author,
                                Genre = item.Genre,
                                Format = item.Format,
                                Language = item.Language,
                                ID = item.ID,
                                Stock = item.Stock,

                            });
                        }
                    }
                    else if (item.Type == "Movie")
                    {
                        Movie currentMovie = null;

                        //Checking for similar element in storage

                        foreach (Movie movie in MovieList)
                        {
                            if (movie.Name == item.Name)
                            {
                                productItemExist = true;
                                currentMovie = movie;
                                break;
                            }
                        }

                        //Adding up stock or adding back product to storage

                        if (productItemExist)
                        {
                            currentMovie.Stock += item.Stock;
                        }
                        else
                        {
                            MovieList.Add(new Movie()
                            {
                                Name = item.Name,
                                Price = item.Price,
                                Format = item.Format,
                                Playtime = item.Playtime,
                                ID = item.ID,
                                Stock = item.Stock,

                            });
                        }

                    }
                    else if (item.Type == "Game")
                    {
                        Game currentGame = null;

                        //Checking for similar element in storage

                        foreach (Game game in GameList)
                        {
                            if (game.Name == item.Name)
                            {
                                productItemExist = true;
                                currentGame = game;
                                break;
                            }
                        }

                        //Adding up stock or adding back product to storage

                        if (productItemExist)
                        {
                            currentGame.Stock += item.Stock;
                        }
                        else
                        {
                            GameList.Add(new Game()
                            {
                                Name = item.Name,
                                Price = item.Price,
                                Platform = item.Platform,
                                ID = item.ID,
                                Stock = item.Stock,

                            });
                        }
                    }
                }

                clearCart();
            }
        }

        public void clearCart()
        {
            CartBuffert.Clear();
            CartList.Clear();
            priceCartBuffer = 0;
            priceLabel.Text = "-";
        }

    }
}
