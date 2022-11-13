using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using ToDo.Models;
using ToDo.Models.VieWModel;

namespace ToDo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var toDoViewModel = GetAllToDos();
        return View(toDoViewModel);
    }

    /// <summary>
    /// Fill in the form
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public JsonResult PopulateForm(int id)
    {
        var todo = GetById(id);
        return Json(todo);
    }

    /// <summary>
    /// getting all items for drawing all tasks on home page 
    /// </summary>
    /// <returns></returns>
    internal ToDoViewModel GetAllToDos()
    {
        List<ToDoItem> toDoItems = new List<ToDoItem>();

        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = "SELECT * FROM todo";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            toDoItems.Add(
                                new ToDoItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                        }
                    }
                    else
                    {
                        return new ToDoViewModel
                        {
                            TodoList = toDoItems
                        };
                    }
                };
            }
        }
        return new ToDoViewModel
        {
            TodoList = toDoItems
        };
    }

    /// <summary>
    /// getting items by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    internal ToDoItem GetById(int id)
    {
        ToDoItem todo = new();

        using (var connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"SELECT * FROM todo Where Id = '{id}'";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        todo.Id = reader.GetInt32(0);
                        todo.Name = reader.GetString(1);
                    }
                    else
                    {
                        return todo;
                    }
                };
            }
        }

        return todo;
    }
    public RedirectResult Insert(ToDoItem todo)
    {
        using (SqliteConnection connection =
                new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"INSERT INTO todo (name) VALUES ('{todo.Name}')";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("http://localhost:5009");
    }

    [HttpPost]
    public JsonResult Delete(int id)
    {
        using (SqliteConnection connection =
                new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"DELETE from todo WHERE Id = '{id}'";
                tableCmd.ExecuteNonQuery();
            }
        }

        return Json(new { });
    }

    public RedirectResult Update(ToDoItem todo)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"UPDATE todo SET name = '{todo.Name}' WHERE Id = '{todo.Id}'";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        return Redirect("http://localhost:5009");
    }
}
