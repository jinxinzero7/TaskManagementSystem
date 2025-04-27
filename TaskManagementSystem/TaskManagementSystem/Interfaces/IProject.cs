using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementSystem.Interfaces
{
    public interface IProject
    {
        /* Свойства интерфейса IProject:
         - Id
         - Name
         - Tasks: Включает в себя все задачи, назначенные на этот проект */

        int Id { get; }
        string Name { get; set; }
        public List<ITask> Tasks { get; set; }

        // Метод "Добавить задачу" для добавления задачи в проект
        void AddTask(ITask task);

        // Метод "Удалить задачу" для удаления задачи из проекта
        void DeleteTask(ITask task);

        // Метод "Найти задачу по Id" для поиска задачи по ее уникальному идентификатору
        ITask? FindTaskOnId(int id);
    }
}
