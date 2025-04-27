using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementSystem.Interfaces
{
    public interface IUser
    {
        /* Свойства интерфейса IUser:
        - Id
        - Name
        - Email */

        int Id { get; }
        string Name { get; set; }
        string Email { get; set; }
        
    }
}
