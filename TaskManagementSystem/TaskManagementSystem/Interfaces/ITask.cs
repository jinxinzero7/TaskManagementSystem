using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementSystem.Interfaces
{
    public interface ITask
    {
        /* Свойства интерфейса ITask:
        - Id
        - Title
        - Description
        - AssignedTo: Включает в себя пользователя, которому назначена данная задача
        - Status: Статус выполнения задачи, все состояния описаны в enum TaskStatus
        - DueDate: Дата, по окончанию которой задача будет считаться просроченной */

        int Id { get; }
        string Title { get; set; }
        string Description { get; set; }
        IUser? AssignedTo { get; set; }
        TaskStatus Status { get; set; }
        DateTime DueDate { get; set; }

        // Метод для изменения статуса задачи на "Выполнена"
        void MarkAsCompleted();

        // Метод для изменения статуса задачи на "В прогрессе"
        public void MarkAsInProgress();

    }
}
