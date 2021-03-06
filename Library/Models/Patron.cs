using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

  namespace Library.Models
{
  public class Patron
  {
    private string _patronname;
    private int _id;


    public Patron(string patronname,int id = 0)

    {
      _patronname = patronname;
      _id = id;
    }

    public override bool Equals(System.Object otherPatron)
    {
      if (!(otherPatron is Patron))
      {
        return false;
      }
      else
      {
        Patron newPatron = (Patron) otherPatron;
        return this.GetId().Equals(newPatron.GetId());
      }
    }
    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }
    public string GetPatron()
    {
      return _patronname;
    }
    public int GetId()
    {
      return _id;
    }
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO patrons (patronname) VALUES (@patronname);";

      MySqlParameter patronname = new MySqlParameter();
      patronname.ParameterName = "@patronname";
      patronname.Value = this._patronname;
      cmd.Parameters.Add(patronname);


      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Patron> GetAll()
    {
      List<Patron> allPatron = new List<Patron> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM patrons;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int PatronId = rdr.GetInt32(0);
        string PatronName = rdr.GetString(1);
        Patron newPatron = new Patron(PatronName, PatronId);
        allPatron.Add(newPatron);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allPatron;
    }

    public static Patron Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM patrons WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int PatronId = 0;
      string PatronName = "";


      while(rdr.Read())
      {
        PatronId = rdr.GetInt32(0);
        PatronName = rdr.GetString(1);

      }
      Patron newPatron = new Patron(PatronName,PatronId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newPatron;
    }

    public void AddBook(Book newBook)
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"INSERT INTO patrons_books (patron_id, book_id) VALUES (@PatronId, @BookId);";

        MySqlParameter patron_id = new MySqlParameter();
        patron_id.ParameterName = "@PatronId";
        patron_id.Value = _id;
        cmd.Parameters.Add(patron_id);

        MySqlParameter book_id = new MySqlParameter();
        book_id.ParameterName = "@BookId";
        book_id.Value = newBook.GetId();
        cmd.Parameters.Add(book_id);

        cmd.ExecuteNonQuery();
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
    }

    public List<Book> GetBooks()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT books.* FROM patrons JOIN patrons_books ON (patrons.id = patrons_books.patron_id) JOIN books ON (patrons_books.book_id = books.id) WHERE patrons.id = @PatronId;";


      MySqlParameter PatronId = new MySqlParameter();
      PatronId.ParameterName = "@PatronId";
      PatronId.Value = _id;
      cmd.Parameters.Add(PatronId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<Book> books = new List<Book>{};

      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookTitlename = rdr.GetString(1);
        int bookcopies = rdr.GetInt32(2);
        Book newBook = new Book(bookTitlename, bookcopies, bookId);
        books.Add(newBook);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return books;
    }

    public void AddCopy(Copy newCopy)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO patrons_copies (patron_id, copy_id) VALUES (@PatronId, @CopyId);";


            MySqlParameter copy_id = new MySqlParameter();
            copy_id.ParameterName = "@CopyId";
            copy_id.Value = newCopy.GetId();
            cmd.Parameters.Add(copy_id);

            MySqlParameter patron_id = new MySqlParameter();
            patron_id.ParameterName = "@PatronId";
            patron_id.Value = _id;
            cmd.Parameters.Add(patron_id);

            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public List<Patron> GetPatrons()
        {
          MySqlConnection conn = DB.Connection();
          conn.Open();
          var cmd = conn.CreateCommand() as MySqlCommand;
          cmd.CommandText = @"SELECT copies.* FROM patrons JOIN patrons_copies ON (patrons.id = patrons_copies.patron_id) JOIN copies ON (patrons_copies.copy_id = copies.id) WHERE patrons.id = @PatronId;";

          MySqlParameter PatronId = new MySqlParameter();
          PatronId.ParameterName = "@PatronId";
          PatronId.Value = _id;
          cmd.Parameters.Add(PatronId);

          var rdr = cmd.ExecuteReader() as MySqlDataReader;
          List<Patron> patrons = new List<Patron>{};

          while(rdr.Read())
          {
            int patronId = rdr.GetInt32(0);
            string patronname = rdr.GetString(1);

            Patron newPatron = new Patron(patronname,patronId);
            patrons.Add(newPatron);
          }
          conn.Close();
          if (conn != null)
          {
            conn.Dispose();
          }
          return patrons;
        }


    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM patrons;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void DeletePatron()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM patrons WHERE id = @PatronId; DELETE FROM patrons_books WHERE patron_id = @PatronId;", conn);
      MySqlParameter patronsIdParameter = new MySqlParameter();
      patronsIdParameter.ParameterName = "@PatronId";
      patronsIdParameter.Value = this.GetId();

      cmd.Parameters.Add(patronsIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }


  }

}
