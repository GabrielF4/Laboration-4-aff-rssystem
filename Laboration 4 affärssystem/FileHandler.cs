using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboration_4_affärssystem
{
    public static class FileHandler
    {

        public static void LoadBooks(BindingList<Book> books)
        {
            using (var reader = new StreamReader("database.csv"))
            {

                books.Clear();

                // Read header row (if any)
                var header = reader.ReadLine();

                // Read data rows
                while (!reader.EndOfStream)
                {

                    var line = reader.ReadLine();

                    if (line == "Filmer,,,,,") return;

                    var values = line.Split(',');

                    if(values.Length == 8)
                    {
                        var book = new Book
                        {
                            Name = values[0],
                            Price = int.Parse(values[1]),
                            Author = values[2],
                            Genre = values[3],
                            Format = values[4],
                            Language = values[5],
                            ID = int.Parse(values[6]),
                            Stock= int.Parse(values[7])
                        };

                        books.Add(book);
                    }
                }
            }
        }

        public static void LoadMovies(BindingList<Movie> movies)
        {
            using (var reader = new StreamReader("database.csv"))
            {

                movies.Clear();

                //Goto section "Filmer" in csv file
                while (reader.ReadLine() != "Filmer,,,,," && !reader.EndOfStream);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    //Section "Filmer" has ended
                    if (line == "Dataspel,,,,,") return;

                    var values = line.Split(',');

                    if(values.Length == 6)
                    {
                        var movie = new Movie
                        {
                            Name = values[0],
                            Price = int.Parse(values[1]),
                            Format = values[2],
                            Playtime = values[3],
                            ID = int.Parse(values[4]),
                            Stock = int.Parse(values[5])
                        };

                        movies.Add(movie);
                    }
                }
            }
        }

        public static void LoadGames(BindingList<Game> games)
        {
            using (var reader = new StreamReader("database.csv"))
            {

                games.Clear();

                //Goto section "Dataspel" in csv file
                while (reader.ReadLine() != "Dataspel,,,,," && !reader.EndOfStream);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    //Section "Dataspel" has ended
                    if (line == "ProduktIDRäknare,,,,,") return;

                    var values = line.Split(',');

                    if(values.Length == 5)
                    {
                        var game = new Game
                        {
                            Name = values[0],
                            Price = int.Parse(values[1]),
                            Platform = values[2],
                            ID = int.Parse(values[3]),
                            Stock = int.Parse(values[4])
                        };

                        games.Add(game);
                    }
                }
            }
        }

        public static int GetProductIDCounter()
        {
            using (var reader = new StreamReader("database.csv"))
            {
                //Goto section "Dataspel" in csv file
                while (reader.ReadLine() != "ProduktIDRäknare,,,,," && !reader.EndOfStream) ;

                int.TryParse(reader.ReadLine(), out int theID);

                return theID;
            } 
        }

        public static void SaveToFile(
            BindingList<Book> BookList,
            BindingList<Movie> MovieList,
            BindingList<Game> GameList,
            int IDCounter) {

            using (var writer = new StreamWriter("database.csv"))
            {

                writer.WriteLine("Böcker,,,,,");

                // Write data rows
                foreach (var book in BookList)
                {
                    writer.WriteLine($"{book.Name},{book.Price},{book.Author},{book.Genre},{book.Format},{book.Language},{book.ID}, {book.Stock}");
                }

                writer.WriteLine("Filmer,,,,,");

                foreach (var movie in MovieList)
                {
                    writer.WriteLine($"{movie.Name},{movie.Price},{movie.Format},{movie.Playtime},{movie.ID}, {movie.Stock}");
                }

                writer.WriteLine("Dataspel,,,,,");

                foreach (var game in GameList)
                {
                    writer.WriteLine($"{game.Name},{game.Price},{game.Platform}, {game.ID}, {game.Stock}");
                }

                writer.WriteLine("ProduktIDRäknare,,,,,");
                writer.WriteLine($"{IDCounter}");
            }
        }

    }
}
