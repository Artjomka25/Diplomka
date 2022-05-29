using System.Collections.Generic;

namespace Diplomka.Models
{
    public enum RoleEnum : int
    {
        Администратор = 1,
        Планировщик,
        Заказчик
    }
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
}
