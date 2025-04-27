using System.Collections.Generic;
using System.Xml.Linq;
using TaskManagementSystem.Interfaces;

// Enum в которой описаны все состояния статуса задач
public enum TaskStatus
{
    ToDo,
    InProgress,
    Overdue,
    Completed
}

// Обьявление класса задач
public class Task : ITask
{
    // Объявление переменной для уникального Id
    private static int _nextTaskId = 1;

    public int Id { get; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IUser? AssignedTo { get; set; }

    public TaskStatus Status { get; set; }
    public DateTime DueDate { get; set; }

    public Task(string title, string description, DateTime dueDate)
    {
        // Генерация уникального идентификатора с помощью инкремента
        Id = _nextTaskId++;
        Title = title;
        Description = description;
        DueDate = dueDate;
        
        // Проверка на просроченность задачи
        if (dueDate < DateTime.Now)
            Status = TaskStatus.Overdue;
        else
            Status = TaskStatus.ToDo;
    }

    public void MarkAsCompleted()
    {
        Status = TaskStatus.Completed;
    }

    public void MarkAsInProgress()
    {
        Status = TaskStatus.InProgress;
    }

    // Переопределение метода ToString() для корректного вывода
    public override string ToString()
    {
        return $"Id: {Id} Название: {Title} Описание: {Description} Назначена пользователю: {AssignedTo}, Статус: {Status}, Дедлайн: {DueDate:yyyy-MM-dd}";
    }
}

// Объявление класса пользователей
public class User : IUser
{
    // Объявление уникального идентификатора аналогично для пользователя
    private static int _nextUserId = 1;
    public int Id { get; }
    public string Name { get; set; }
    public string Email { get; set; }


    public User(string name, string email)
    {
        Id = _nextUserId++;
        Name = name;
        Email = email;
    }

    public override string ToString()
    {
        return $"Имя: {Name} Почта: {Email}";
    }
}

// Объявление класса проектов
public class Project : IProject
{
    private static int _nextProjectId = 1;
    public int Id { get; }
    public string Name { get; set; }
    public List<ITask> Tasks { get; set; }

    public Project(string name)
    {
        Id = _nextProjectId++;
        Name = name;
        Tasks = new List<ITask>();
    }

    public void AddTask(ITask task)
    {
        Tasks.Add(task);
    }

    public void DeleteTask(ITask task)
    {
        // Проверка на существование задачи
        if (!Tasks.Contains(task))
            throw new ArgumentException("Задача не найдена.");
        Tasks.Remove(task);
    }

    public ITask? FindTaskOnId(int id)
    {
        return Tasks.FirstOrDefault(task => task.Id == id);
    }

}

// Объявление класса Менеджера Задач для работы с проектами, пользователями и задачами
public class TaskManager
{
    // Свойста: список пользователей, список проектов
    public List<IUser> Users { get; set; }
    public List<IProject> Projects { get; set; }

    public TaskManager()
    {
        Users = new List<IUser>();
        Projects = new List<IProject>();
    }

    // Метод создания пользователя
    public IUser CreateUser(string name, string email)
    {
        User user = new User(name, email);
        Users.Add(user);
        return user;
    }

    // Метод создания проекта
    public IProject CreateProject(string name)
    {
        Project project = new Project(name);
        Projects.Add(project);
        return project;
    }

    // Метод создания задачи
    public ITask CreateTask(IProject Project, string title, string description, DateTime DueDate)
    {
        if (title == null)
        {
            throw new ArgumentNullException(nameof(title), "Название не может быть null.");
        }
        if (description == null)
        {
            throw new ArgumentNullException(nameof(description), "Описание не может быть null.");
        }
        Task task = new Task(title, description, DueDate);
        Project.AddTask(task);
        return task;
    }

    // Метод назначения задачи пользователю
    public void AssignTask(ITask task, IUser user)
    {
        // Проверка на null значение
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task), "Задача не может быть null.");
        }
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
        }
        task.AssignedTo = user;
    }

    // Метод возвращающий задачи с указанным статусом
    public List<ITask> GetTasksByStatus(TaskStatus status)
    {
        List<ITask> tasks = new List<ITask>();
        foreach (IProject proj in Projects)
        {
            tasks.AddRange(from task in proj.Tasks where task.Status == status select task);
        }
        return tasks;
    }

    // Метод возвращающий задачи назначенные указанному пользователю
    public List<ITask> GetTasksAssignedToUser(IUser user)
    {
        List<ITask> tasks = new List<ITask>();
        foreach (IProject proj in Projects)
        {
            tasks.AddRange(from task in proj.Tasks where task.AssignedTo == user select task);
        }
        return tasks;
    }

    // Метод возвращающий все задачи указанного проекта
    public List<ITask> GetTasksForProject(IProject project)
    {
        return project.Tasks;
    }

    // Метод возвращающий задачи, срок выполнения которых вышел
    public List<ITask> GetTasksOverDue()
    {
        List<ITask> tasks = new List<ITask>();
        foreach (IProject proj in Projects)
        {
            tasks.AddRange(from task in proj.Tasks where task.DueDate < DateTime.Now && task.Status != TaskStatus.Completed select task);
        }
        return tasks;
    }

}

partial class Program
{
    static void Main(string[] args)
    {
        // Инициализация Менеджера Задач
        TaskManager taskManager = new TaskManager();

        // Инициализация пользователей
        IUser user1 = taskManager.CreateUser("Михаил", "michael@gmail.com");
        IUser user2 = taskManager.CreateUser("Алиса", "alice@gmail.com");
        IUser user3 = taskManager.CreateUser("Даниил", "daniel@gmail.com");

        // Инициализация проектов
        IProject project1 = taskManager.CreateProject("Редизайн сайта");
        IProject project2 = taskManager.CreateProject("Разработка приложения");
        IProject project3 = taskManager.CreateProject("Разработка сайта");

        // Инициализация задач
        DateTime date1 = new DateTime(2025, 4, 15);
        ITask task1 = taskManager.CreateTask(project1, "Переработка лого", "Создание нового логотипа по требованиям заказчика.", date1);
        DateTime date2 = new DateTime(2025, 4, 30);
        ITask task2 = taskManager.CreateTask(project1, "Изменение палитры сайта", "Сверка цветов с заказчиком. Реализация.", date2);

        DateTime date3 = new DateTime(2025, 6, 21);
        ITask task3 = taskManager.CreateTask(project2, "Разработка дизайна приложения", "Создание макета в Figma.", date3);
        
        DateTime date4 = new DateTime(2025, 4, 18);
        ITask task4 = taskManager.CreateTask(project3, "Создание макета", "Набросать идеи. Реализовать, согласовав с заказчиком", date4);
        DateTime date5 = new DateTime(2025, 4, 24);
        ITask task5 = taskManager.CreateTask(project3, "Верстка сайта", "Согласно макету, сверстать сайт.", date5);
        DateTime date6 = new DateTime(2025, 5, 2);
        ITask task6 = taskManager.CreateTask(project3, "Обсуждение с заказчиком", "Корректировка требований заказчика.", date6);

        // Назначение задач пользователям 1, 2 и 3
        taskManager.AssignTask(task1, user1);
        taskManager.AssignTask(task2, user2);
        taskManager.AssignTask(task3, user1);
        taskManager.AssignTask(task4, user1);
        taskManager.AssignTask(task5, user3);
        taskManager.AssignTask(task6, user2);

        // Выставление статусов задач
        task1.MarkAsCompleted();

        task4.MarkAsInProgress();
        task2.MarkAsInProgress();

        // Вывод всех задач по статусу

        Console.WriteLine("Задачи со статусом 'ToDo':");
        foreach (ITask task in taskManager.GetTasksByStatus(TaskStatus.ToDo))
        {
            Console.WriteLine(task);
        }

        Console.WriteLine("\nЗадачи со статусом 'In Progress':");
        foreach (ITask task in taskManager.GetTasksByStatus(TaskStatus.InProgress))
        {
            Console.WriteLine(task);
        }

        Console.WriteLine("\nЗадачи со статусом 'Completed':");
        foreach (ITask task in taskManager.GetTasksByStatus(TaskStatus.Completed))
        {
            Console.WriteLine(task);
        }

        Console.WriteLine("\nЗадачи со статусом 'Overdue':");
        foreach (ITask task in taskManager.GetTasksByStatus(TaskStatus.Overdue))
        {
            Console.WriteLine(task);
        }

        // Вывод всех задач по пользователям

        Console.WriteLine("\nЗадачи, назначенные user1:");
        foreach (ITask task in taskManager.GetTasksAssignedToUser(user1))
        {
            Console.WriteLine(task);
        }

        Console.WriteLine("\nЗадачи, назначенные user2:");
        foreach (ITask task in taskManager.GetTasksAssignedToUser(user2))
        {
            Console.WriteLine(task);
        }

        Console.WriteLine("\nЗадачи, назначенные user3:");
        foreach (ITask task in taskManager.GetTasksAssignedToUser(user3))
        {
            Console.WriteLine(task);
        }

        // Вывод всех задач по проектам

        Console.WriteLine("\nЗадачи для проекта project1:");
        foreach (ITask task in taskManager.GetTasksForProject(project1))
        {
            Console.WriteLine(task);
        }

        Console.WriteLine("\nЗадачи для проекта project2:");
        foreach (ITask task in taskManager.GetTasksForProject(project2))
        {
            Console.WriteLine(task);
        }

        Console.WriteLine("\nЗадачи для проекта project3:");
        foreach (ITask task in taskManager.GetTasksForProject(project3))
        {
            Console.WriteLine(task);
        }

        Console.ReadKey();

    }
}




